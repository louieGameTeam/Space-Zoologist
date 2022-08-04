﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles checking and display of food quality VFXObjects
/// </summary>
public class FoodQualityVFXHandler : MonoBehaviour
{
    public static FoodQualityVFXHandler Instance = null;

    public struct EatingData
    {
        public bool IsEating;
        public ItemID FoodBeingEaten;

        public EatingData(bool isEating, ItemID foodBeingEaten)
        {
            this.IsEating = isEating;
            this.FoodBeingEaten = foodBeingEaten;
        }
    }

    #region Private Fields
    private Dictionary<ItemID, bool> EdibleFoods;
    private List<AnimalSpecies> SpeciesList;
    private int NextSpeciesToDisplayIndex;
    private GameObject SelectedAnimal = null;
    private AnimalSpecies SelectedSpecies = null;
    private bool CanDisplayFoodQualityFX = true;
    #endregion

    #region Constants
    private int FOOD_QUALITY_VFX_INTERVAL = 15; // Interval of time between checks for displaying food quality VFX
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance != null && VFXManager.Instance != null)
        {
            if (SelectedAnimal == null)
            {
                SearchForAnimal();
            }

            else
            {
                DisplayFoodFX();
            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Initialization function for use by GameManager, to guarantee presence of EventManager.
    /// Event subscription should occur here
    /// </summary>
    public void Initialize()
    {
        // When population goes extinct, SelectedAnimal should be deselected to prevent getting locked in no animation playing
        EventManager.Instance.SubscribeToEvent(EventType.PopulationExtinct, () => DeselectAnimalSelection());

        // Same scenario for when population decreases, in case the SelectedAnimal has died
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountChange, (eventData) => HandlePopulationDecrease(eventData));
    }

    /// <summary>
    /// Updates SpeciesSet to contain all species in the current level
    /// </summary>
    public void UpdateSpeciesList()
    {
        SpeciesList = new List<AnimalSpecies>();
        foreach (AnimalSpecies species in GameManager.Instance.AnimalSpecies.Values)
        {
            SpeciesList.Add(species);
        }

        if (SpeciesList.Count > 0)
        {
            NextSpeciesToDisplayIndex = 0;
        }

        else
        {
            Debug.LogError("FoodQualityVFXHandler: SpeciesList of current level empty");
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Displays relevant effects for food that animal is currently eating
    /// </summary>
    private void DisplayFoodFX()
    {
        if (CanDisplayFoodQualityFX)
        {
            EatingData eatingData = GetEatingData();
            if (eatingData.IsEating)
            {
                EdibleFoods = GetEdibleFoods(SelectedSpecies.Needs.FindFoodNeeds());
                Debug.Log($"FOOD CONSUMED: {eatingData.FoodBeingEaten}");
                // If food being eaten is edible for the selected species
                if (EdibleFoods.ContainsKey(eatingData.FoodBeingEaten))
                {
                    // If food being eaten is preferred
                    if (EdibleFoods[eatingData.FoodBeingEaten] == true)
                        VFXManager.Instance.DisplayVFX(SelectedAnimal.transform.position, VFXType.GoodFood);

                    // If food being eaten is not preferred
                    else
                        VFXManager.Instance.DisplayVFX(SelectedAnimal.transform.position, VFXType.NeutralFood);
                }

                // If the food being eaten is not edible for the selected species
                else
                    VFXManager.Instance.DisplayVFX(SelectedAnimal.transform.position, VFXType.BadFood);

                // Once VFX has been played, start cooldown before next VFX can be played
                StartCoroutine(FoodQualityFXCooldown(FOOD_QUALITY_VFX_INTERVAL));
            }
        }
    }

    /// <summary>
    /// Enforces cooldown between instances of displaying food FX
    /// </summary>
    /// <param name="checkInterval"> Number of seconds to wait between checks for displaying food quality FX </param>
    /// <returns></returns>
    private IEnumerator FoodQualityFXCooldown(int checkInterval)
    {
        CanDisplayFoodQualityFX = false;

        DeselectAnimalSelection();

        yield return new WaitForSeconds(checkInterval);
        CanDisplayFoodQualityFX = true;
    }

    /// <summary>
    /// Searches for next animal to display food VFX for
    /// </summary>
    private void SearchForAnimal()
    {
        // Get a list of all populations of a species
        SelectedSpecies = SpeciesList[NextSpeciesToDisplayIndex];
        List<Population> populationList = GameManager.Instance.m_populationManager.GetPopulationsBySpecies(SelectedSpecies);

        if (populationList.Count > 0)
        {
            // Select random population from the species
            int selectedPopIndex = UnityEngine.Random.Range(0, populationList.Count);
            Population selectedPop = populationList[selectedPopIndex];

            // Selects random animal from the selected population
            SelectedAnimal = selectedPop.AnimalPopulation[UnityEngine.Random.Range(0, selectedPop.AnimalPopulation.Count)];
            SelectedSpecies = SpeciesList[NextSpeciesToDisplayIndex];

            // Cycle through species to display FoodVFX for
            NextSpeciesToDisplayIndex = (NextSpeciesToDisplayIndex + 1) % SpeciesList.Count;
        }

        // Reset SelectedSpecies to null if populationList is empty
        else
            SelectedSpecies = null;
    }

    /// <summary>
    /// Determine whether animal is currently eating as well as what food they are eating
    /// </summary>
    /// <returns></returns>
    private EatingData GetEatingData()
    {
        BehaviorPattern temp = SelectedAnimal.GetComponent<AnimalBehaviorManager>().activeBehaviorPattern;
        if (temp is EatingPattern)
            return new EatingData(true, ((EatingPattern)temp).FoodID);

        return new EatingData(false, ItemID.Invalid);
    }
    
    /// <summary>
    /// Converts a species' food needs into a dictionary for comparison with food currently being eaten
    /// </summary>
    /// <param name="foodNeeds"></param>
    /// <returns></returns>
    private Dictionary<ItemID, bool> GetEdibleFoods(NeedData[] foodNeeds)
    {
        Dictionary<ItemID, bool> edibleFoods = new Dictionary<ItemID, bool>();
        foreach (NeedData need in foodNeeds)
        {
            edibleFoods.Add(need.ID, need.Preferred);
        }

        return edibleFoods;
    }

    /// <summary>
    /// Deselects animal and species, allowing for new animal to be selected and avoiding
    /// the possibility that selected animal no longer exists.
    /// </summary>
    private void DeselectAnimalSelection()
    {
        SelectedAnimal = null;
        SelectedSpecies = null;
    }

    /// <summary>
    /// Handles deselection of animal and species when population count decreases, allowing
    /// for new animal to be selected and avoiding the possibility that a selected animal no
    /// longer exists.
    /// </summary>
    /// <param name="eventData"> Tuple data passed by event invocation </param>
    private void HandlePopulationDecrease(object eventData)
    {
        ValueTuple<Population, bool> eventTuple = (ValueTuple<Population, bool>)eventData;
        try
        {
            if (eventTuple.Item2 == false)
            {
                // Deselect animal and species
                DeselectAnimalSelection();
            }
        }
            
        catch
        {
            Debug.LogError("FoodQualityVFXHandler.DeselectAnimal: Cast from eventData to tuple failed");
        }
    }
    #endregion
}
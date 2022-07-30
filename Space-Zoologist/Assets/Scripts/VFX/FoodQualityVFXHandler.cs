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
                if (CanDisplayFoodQualityFX)
                {
                    EatingData eatingData = GetEatingData();
                    if (eatingData.IsEating)
                    {
                        foreach (NeedData need in SelectedSpecies.Needs.FindFoodNeeds())
                        {
                            // Food is edible
                            if (eatingData.FoodBeingEaten == need.ID)
                            {
                                // If food eaten is a preferred food
                                if (need.Preferred)
                                {
                                    VFXManager.Instance.DisplayVFX(SelectedAnimal.transform.position, VFXType.GoodFood);
                                }

                                else
                                {
                                    VFXManager.Instance.DisplayVFX(SelectedAnimal.transform.position, VFXType.NeutralFood);
                                }

                                break;
                            }

                            // Food is inedible
                            else
                            {
                                VFXManager.Instance.DisplayVFX(SelectedAnimal.transform.position, VFXType.BadFood);
                                break;
                            }
                        }

                        // Once VFX has been played, start cooldown before next VFX can be played
                        StartCoroutine(FoodQualityFXCooldown(FOOD_QUALITY_VFX_INTERVAL));
                    }
                }
            }
        }
    }
    #endregion

    #region Public Methods
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
    /// Enforces cooldown between instances of displaying food FX
    /// </summary>
    /// <param name="checkInterval"> Number of seconds to wait between checks for displaying food quality FX </param>
    /// <returns></returns>
    private IEnumerator FoodQualityFXCooldown(int checkInterval)
    {
        CanDisplayFoodQualityFX = false;

        // Deselect animal and species
        SelectedAnimal = null;
        SelectedSpecies = null;

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
            int selectedPopIndex = Random.Range(0, populationList.Count);
            Population selectedPop = populationList[selectedPopIndex];

            // Selects random animal from the selected population
            SelectedAnimal = selectedPop.AnimalPopulation[Random.Range(0, selectedPop.AnimalPopulation.Count)];
            SelectedSpecies = SpeciesList[NextSpeciesToDisplayIndex];

            // Cycle through species to display FoodVFX for
            NextSpeciesToDisplayIndex = (NextSpeciesToDisplayIndex + 1) % SpeciesList.Count;
        }

        else
        {
            SelectedSpecies = null;
        }
    }

    private EatingData GetEatingData()
    {
        BehaviorPattern temp = SelectedAnimal.GetComponent<AnimalBehaviorManager>().activeBehaviorPattern;
        if (temp is EatingPattern)
        {
            EatingPattern tempEatingPattern = (EatingPattern)temp;
            return new EatingData(true, tempEatingPattern.FoodID);
        }

        return new EatingData(false, ItemID.Invalid);
    }
    #endregion
}
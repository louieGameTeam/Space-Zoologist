using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles checking and display of food quality VFXObjects
/// </summary>
public class FoodQualityVFXHandler : MonoBehaviour
{
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

    #region Public Fields
    public HashSet<ItemID> FoodItems => foodItems;
    #endregion

    #region Private Fields
    private HashSet<ItemID> foodItems; // Set of all possible foods
    private Dictionary<ItemID, bool> EdibleFoods; // Contains food IDs and whether those foods are preferred by the selected species
    private HashSet<ItemID> PreferredFoods;       // Contains food IDs of all preferred foods of selected species
    private List<AnimalSpecies> SpeciesList;
    private int NextSpeciesToDisplayIndex;
    [SerializeField][EditorReadOnly] private GameObject SelectedAnimal = null;
    [SerializeField][EditorReadOnly] private AnimalSpecies SelectedSpecies = null;
    [SerializeField][EditorReadOnly] private bool CanDisplayFoodQualityFX = true;

    [Tooltip("Time in seconds between food VFX being displayed")]
    [SerializeField] private int FoodQualityVFXInterval; // Interval of time between checks for displaying food quality VFX
    #endregion

    #region Monobehaviour Callbacks

    private void Update()
    {
        if (GameManager.Instance != null && 
            VFXManager.Instance != null && 
            !GameManager.Instance.IsPaused)
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
        foodItems = new HashSet<ItemID> (ItemRegistry.GetItemIDsWithCategory (ItemRegistry.Category.Food));

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
                // EdibleFoods = GetEdibleFoods(SelectedSpecies.Needs.FindFoodNeeds());
#if UNITY_EDITOR
                Debug.Log($"FOOD CONSUMED: {eatingData.FoodBeingEaten}");
#endif 
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
                
                // Play the animal-specific audio
                if(!SelectedSpecies.EatingSFX)
                    Debug.LogError("Species is missing eating SFX, please assign in editor", SelectedSpecies);
                    
                AudioManager.instance.PlayOneShot(SelectedSpecies.EatingSFX);

                // Once VFX has been played, start cooldown before next VFX can be played
                StartCoroutine(FoodQualityFXCooldown(FoodQualityVFXInterval));
            }
        }
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
            Population selectedPop = populationList[UnityEngine.Random.Range(0, populationList.Count)];

            if (SelectedSpecies != null)
            {
                // Grab edible foods for this species
                GetEdibleFoods(SelectedSpecies.Needs.FindFoodNeeds());
            }

            else
                return;
            
            // Check for the existence of NeedRating for the selected population
            NeedRatingCache needRatingCache = GameManager.Instance.Needs.Ratings;
            if (needRatingCache.HasRating(selectedPop))
            {
                NeedRating needRating = needRatingCache.GetRating(selectedPop);

                // If food needs are met, prioritize finding animals that are pathing to preferred/neutral foods
                if (needRating.FoodNeedIsMet)
                {
                    // Attempt to prioritize animals pathing to preferred foods, return if animal found
                    if (SearchForAnimalPathingToPreferred(selectedPop))
                        return;

                    // Attempt to prioritize animals pathing to neutral foods, return if animal found
                    if (SearchForAnimalPathingToNeutral(selectedPop))
                        return;

                    // Otherwise, do not select animal and allow search to continue
                    //SelectedAnimal = selectedPop.AnimalPopulation[UnityEngine.Random.Range(0, selectedPop.AnimalPopulation.Count)];
                    DeselectAnimalSelection();
                    return;
                }
                
                // Otherwise, if food needs are not met, prioritize finding animals that are heading to bad quality foods
                else
                {
                    // Attempt to prioritize animals pathing to bad foods, return if animal found
                    if (SearchForAnimalPathingToBad(selectedPop))
                        return;

                    // Attempt to prioritize animals pathing to neutral foods, return if animal found
                    if (SearchForAnimalPathingToNeutral(selectedPop))
                        return;

                    // Otherwise, do not select animal and allow search to continue
                    // SelectedAnimal = selectedPop.AnimalPopulation[UnityEngine.Random.Range(0, selectedPop.AnimalPopulation.Count)];
                    DeselectAnimalSelection();
                    return;
                }
            }

            // If no NeedRating exists yet, do not select animal and allow search to continue
            else
            {
                //SelectedAnimal = selectedPop.AnimalPopulation[UnityEngine.Random.Range(0, selectedPop.AnimalPopulation.Count)];
                DeselectAnimalSelection();
                return;
            }
        }

        // Reset SelectedSpecies to null and advance to next species to display if populationList is empty
        else
        {
            // Debug.Log($"FoodQualityVFXHandler: Could not find population of species: {SelectedSpecies}");
            DeselectAnimalSelection();
            NextSpeciesToDisplayIndex = (NextSpeciesToDisplayIndex + 1) % SpeciesList.Count;
        }
    }

    /// <summary>
    /// Attempts to find an animal in the currently selected population that's already pathing to
    /// a preferred food for the currently selected species. Selected species should be up to date
    /// before calling this function. Directly sets the SelectedAnimal field and increments the
    /// NextSpeciesToDisplayIndex if animal is found
    /// </summary>
    /// <param name="selectedPop"> Selected population of the currently selected species of animal </param>
    /// <returns> Returns true if animal is found, false otherwise </returns>
    private bool SearchForAnimalPathingToPreferred(Population selectedPop)
    {
        foreach (ItemID preferredFood in PreferredFoods)
        {
            foreach (GameObject animal in selectedPop.AnimalPopulation)
            {
                Animal curAnimal = animal.GetComponent<Animal>();

                // If the food is preferred and an animal is already pathing to it, select that animal
                if (curAnimal.FoodTargetSpeciesName == preferredFood.Data.Name.Get(ItemName.Type.Serialized))
                {
                    SelectedAnimal = animal;

                    // Cycle through species to display FoodVFX for
                    NextSpeciesToDisplayIndex = (NextSpeciesToDisplayIndex + 1) % SpeciesList.Count;
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to find an animal in the currently selected population that's already pathing to
    /// a neutral food for the currently selected species. Selected species should be up to date
    /// before calling this function. Directly sets the SelectedAnimal field and increments the
    /// NextSpeciesToDisplayIndex if animal is found
    /// </summary>
    /// <param name="selectedPop"> Selected population of the currently selected species of animal </param>
    /// <returns> Returns true if animal is found, false otherwise </returns>
    private bool SearchForAnimalPathingToNeutral(Population selectedPop)
    {
        foreach (ItemID food in EdibleFoods.Keys)
        {
            // Skip foods that are preferred, these have already been checked
            if (EdibleFoods[food] == false)
            {
                foreach (GameObject animal in selectedPop.AnimalPopulation)
                {
                    Animal curAnimal = animal.GetComponent<Animal>();

                    // If food is neutral and an animal is already pathing to it, select that animal
                    if (curAnimal.FoodTargetSpeciesName == food.Data.Name.Get(ItemName.Type.Serialized))
                    {
                        SelectedAnimal = animal;

                        // Cycle through species to display FoodVFX for
                        NextSpeciesToDisplayIndex = (NextSpeciesToDisplayIndex + 1) % SpeciesList.Count;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to find an animal in the currently selected population that's already pathing to
    /// a bad quality food for the currently selected species. Selected species should be up to date
    /// before calling this function. Directly sets the SelectedAnimal field and increments the
    /// NextSpeciesToDisplayIndex if animal is found
    /// </summary>
    /// <param name="selectedPop"> Selected population of the currently selected species of animal </param>
    /// <returns> Returns true if animal is found, false otherwise </returns>
    private bool SearchForAnimalPathingToBad(Population selectedPop)
    {
        foreach (ItemID food in foodItems)
        {
            // Filter for bad quality foods from all available foods
            if (!EdibleFoods.ContainsKey(food))
            {
                foreach (GameObject animal in selectedPop.AnimalPopulation)
                {
                    Animal curAnimal = animal.GetComponent<Animal>();
                    
                    // If food is bad and an animal is already pathing to it, select that animal
                    if (curAnimal.FoodTargetSpeciesName == food.Data.Name.Get(ItemName.Type.Serialized))
                    {
                        SelectedAnimal = animal;

                        // Cycle through species to display FoodVFX for
                        NextSpeciesToDisplayIndex = (NextSpeciesToDisplayIndex + 1) % SpeciesList.Count;
                        return true;
                    }
                }
            }
        }

        return false;
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
    /// Converts a species' food needs into a dictionary for comparison with food currently being eaten and
    /// if a food need is preferred, adds to PreferredFoods HashSet
    /// </summary>
    /// <param name="foodNeeds"></param>
    private void GetEdibleFoods(NeedData[] foodNeeds)
    {
        Dictionary<ItemID, bool> tempEdibleFoods = new Dictionary<ItemID, bool>();
        HashSet<ItemID> tempPreferredFoods = new HashSet<ItemID>();

        foreach (NeedData need in foodNeeds)
        {
            tempEdibleFoods.Add(need.ID, need.Preferred);

            if (need.Preferred)
                tempPreferredFoods.Add(need.ID);
        }

        EdibleFoods = tempEdibleFoods;
        PreferredFoods = tempPreferredFoods;
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
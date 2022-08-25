using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPathfinding : GeneralPathfinding
{
    [Header("Same as FoodSourceSpecies.SpeciesName, case sensitive.")]
    [SerializeField][EditorReadOnly] private string foodSpeciesName = default;
    public string FoodSpeciesName => foodSpeciesName;

    #region Private Fields
    private Dictionary<ItemID, float> FoodProbabilities;
    [Header("Food Probabilities (Weights)")]
    [SerializeField] private float GoodFoodProbability = default;
    [SerializeField] private float NeutralFoodProbability = default;
    [SerializeField] private float BadFoodProbability = default;
    #endregion

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        Animal animal = gameObject.GetComponent<Animal>();

        // Assign weights to each food ItemID based on species of current animal
        FoodProbabilities = AssignFoodProbabilities(animal.PopulationInfo.Species.Needs.FindFoodNeeds(), animal);

        // Select random accessible food species to path to, accounting for weights of each food type
        SelectRandomFood(animalData);

        Vector3Int[] destinations = GameManager.Instance.m_foodSourceManager.GetFoodSourcesLocationWithSpecies(foodSpeciesName);
        Vector3Int destination = base.TileDataController.WorldToCell(gameObject.transform.position);
        if (destinations != null)
        {
            // Shuffle destinations
            Vector3Int temp;
            for (int i = 0; i < destinations.Length; i++)
            {
                int random = Random.Range(i, destinations.Length);
                temp = destinations[i];
                destinations[i] = destinations[random];
                destinations[random] = destinations[i];
            }

            // Get a valid destination
            foreach (Vector3Int potentialDestination in destinations)
            {
                if (animalData.animal.PopulationInfo.AccessibleLocations.Contains(potentialDestination))
                {
                    destination = potentialDestination;
                    break;
                }
            }

            if (destination.Equals(new Vector3Int(-1, -1, -1)))
            {
                int locationIndex = animalData.animal.PopulationInfo.random.Next(0, animalData.animal.PopulationInfo.AccessibleLocations.Count);
                if (animalData.animal.PopulationInfo.AccessibleLocationsExist)
                    destination = animalData.animal.PopulationInfo.AccessibleLocations[locationIndex];
            }

            animal.SetFoodTargetSpeciesName(foodSpeciesName);
            AnimalPathfinding.PathRequestManager.RequestPath(base.TileDataController.WorldToCell(gameObject.transform.position), destination, animalData.animal.MovementController.AssignPath, animalData.animal.PopulationInfo.Grid);
        }

        else
        {
            // If the edible food doesn't exist, just go to a random food (or whatever ItemType destination is set to)
            base.EnterPattern(gameObject, animalData);
        }
    }

    #region Private Methods
    /// <summary>
    /// Assigns the correct food weights for this species to all possible foods
    /// </summary>
    /// <param name="foodNeeds"></param>
    /// <returns></returns>
    private Dictionary<ItemID, float> AssignFoodProbabilities(NeedData[] foodNeeds, Animal animal)
    {
        Dictionary<ItemID, float> foodProbabilities = new Dictionary<ItemID, float>();
        NeedRatingCache needRatingCache = GameManager.Instance.Needs.Ratings;

        // If NeedRatingCache has been built, use need ratings to determine food probabilities
        if (needRatingCache.HasRating(animal.PopulationInfo))
        {
            NeedRating needRating = needRatingCache.GetRating(animal.PopulationInfo);

            foreach (NeedData need in foodNeeds)
            {
                if (need.Preferred)
                {
                    // If food needs are met and food is good, add good probability
                    if (needRating.FoodNeedIsMet)
                        foodProbabilities.Add(need.ID, GoodFoodProbability);

                    // If food needs are not met and food is good, add bad probability
                    else
                        foodProbabilities.Add(need.ID, BadFoodProbability);
                }

                // If food is not preferred, must be neutral, which is always assigned neutral probability
                else
                {
                    foodProbabilities.Add(need.ID, NeutralFoodProbability);
                }
            }

            foreach (ItemID foodItem in GameManager.Instance.m_foodQualityVFXManager.FoodItems)
            {
                if (!foodProbabilities.ContainsKey(foodItem))
                {
                    // If food needs are met, normal probability assigned
                    if (needRating.FoodNeedIsMet)
                        foodProbabilities.Add(foodItem, BadFoodProbability);

                    // If food needs are not met, good food probability is assigned to bad food
                    else
                        foodProbabilities.Add(foodItem, GoodFoodProbability);
                }
            }
        }

        // If NeedRatingCache has not been built, default to basic probability assignment
        else
        {
            // A species' foodNeeds only contain foods that are edible for them
            // Assign food weights for good and neutral foods, any food ID not in
            // the foodWeights dict should default to the bad food weight.
            foreach (NeedData need in foodNeeds)
            {
                if (need.Preferred)
                    foodProbabilities.Add(need.ID, GoodFoodProbability);

                else
                    foodProbabilities.Add(need.ID, NeutralFoodProbability);
            }

            foreach (ItemID foodItem in GameManager.Instance.m_foodQualityVFXManager.FoodItems)
            {
                if (!foodProbabilities.ContainsKey(foodItem))
                    foodProbabilities.Add(foodItem, BadFoodProbability);
            }
        }

        return foodProbabilities;
    }

    /// <summary>
    /// Updates FoodSpeciesName to path to, based on food quality weights
    /// </summary>
    /// <param name="animalData"></param>
    private void SelectRandomFood(AnimalData animalData)
    {
        // Set of all reachable food species to the current animal
        HashSet<ItemID> reachableFoodSpecies = new HashSet<ItemID>();
        float universalProbability = 0;
        foreach (ItemID foodItem in GameManager.Instance.m_foodQualityVFXManager.FoodItems)
        {
            Vector3Int[] foodItemLocations = GameManager.Instance.m_foodSourceManager.GetFoodSourcesLocationWithSpecies(foodItem.Data.Name.Get(ItemName.Type.Serialized));
            if (foodItemLocations != null)
            {
                foreach (Vector3Int foodLocation in foodItemLocations)
                {
                    // If animal can reach at least one of this foodItem, no longer need to scan through others
                    if (animalData.animal.PopulationInfo.AccessibleLocations.Contains(foodLocation))
                    {
                        reachableFoodSpecies.Add(foodItem);
                        universalProbability += FoodProbabilities[foodItem];
                        break;
                    }
                }
            }
        }

        float randomThreshold = Random.Range(0, universalProbability);
        float cumulativeProbability = 0;
        foreach (ItemID reachableFood in reachableFoodSpecies)
        {
            if (randomThreshold <= (cumulativeProbability += FoodProbabilities[reachableFood]))
            {
                foodSpeciesName = reachableFood.Data.Name.Get(ItemName.Type.Serialized);
                break;
            }
        }
    }
    #endregion
}
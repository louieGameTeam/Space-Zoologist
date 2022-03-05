using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPathfinding : GeneralPathfinding
{
    [Header("Same as FoodSourceSpecies.SpeciesName, case sensitive.")]
    [SerializeField] string FoodSpeciesName = default;

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        List<Vector3Int> destinations = new List<Vector3Int>();
        //Searches for edible foods based on animal's food needs
        var needs = animalData.animal.PopulationInfo.GetNeedValues();
        foreach(var need in needs)
        {
            var location = GameManager.Instance.m_foodSourceManager.GetFoodSourcesLocationWithSpecies(need.Key);
            if(location != null) destinations.AddRange(location);
        }
        Vector3Int destination = new Vector3Int(-1,-1,-1);
        if (destinations.Count != 0)
        {
            // Shuffle destinations
            Vector3Int temp;
            for (int i = 0; i < destinations.Count; i++)
            {
                int random = Random.Range(i, destinations.Count);
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
                destination = animalData.animal.PopulationInfo.AccessibleLocations[locationIndex];
            }
            AnimalPathfinding.PathRequestManager.RequestPath(base.TileDataController.WorldToCell(gameObject.transform.position), destination, animalData.animal.MovementController.AssignPath, animalData.animal.PopulationInfo.Grid);
        }
        else {
            // If the edible food doesn't exist, just go to a random food (or whatever ItemType destination is set to)
            base.EnterPattern(gameObject,animalData);
        }
    }
}

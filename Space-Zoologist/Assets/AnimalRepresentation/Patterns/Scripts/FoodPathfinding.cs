using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPathfinding : GeneralPathfinding
{
    [Header("Same as FoodSourceSpecies.SpeciesName, case sensitive.")]
    [SerializeField] string FoodSpeciesName = default;

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        Vector3Int[] destinations = FoodSourceManager.ins.GetFoodSourcesLocationWithSpecies(FoodSpeciesName);
        Vector3Int destination = new Vector3Int(-1,-1,-1);
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
                // TODO figure out how to exit pattern since condition not satisfied
                int locationIndex = animalData.animal.PopulationInfo.random.Next(0, animalData.animal.PopulationInfo.AccessibleLocations.Count);
                destination = animalData.animal.PopulationInfo.AccessibleLocations[locationIndex];
            }

            AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(gameObject.transform.position), destination, animalData.animal.MovementController.AssignPath, animalData.animal.PopulationInfo.Grid);
        }
        else {
            // If the edible food doesn't exist, just go to a random food (or whatever ItemType destination is set to)
            base.EnterPattern(gameObject,animalData);
        }
    }
}

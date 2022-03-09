using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            List<FoodSource> foodSources = GameManager.Instance.m_foodSourceManager.FoodSources;
            string message = "Food source ratings (experimental):";

            // Go through each food source in the manager
            foreach (FoodSource food in foodSources)
            {
                // Get food source need availability
                NeedRating rating = NeedRatingFactory.Build(food);
                string ratingJson = JsonUtility.ToJson(rating, true);

                message += $"\n\t{food.Species} at {food.GetCellPosition()}: \n{ratingJson}";
            }

            message += "\nPopulation ratings (experimental):";

            // Build need distribution for each population
            Dictionary<Population, NeedAvailability> distribution = NeedAvailabilityFactory.BuildDistribution();

            // Go through all entries in the dictionary
            foreach (KeyValuePair<Population, NeedAvailability> kvp in distribution)
            {
                // Build need rating for this population
                NeedRating rating = NeedRatingFactory.Build(kvp.Key, kvp.Value);
                string ratingJson = JsonUtility.ToJson(rating, true);

                message += $"\n\t{kvp.Key.Species} at {kvp.Key.GetPosition()}: \n{ratingJson}";
            }

            Debug.Log(message);
        }
    }

    private void OnDrawGizmos()
    {
        if (GameManager.Instance)
        {
            TileDataController tileDataController = GameManager.Instance.m_tileDataController;

            if (tileDataController)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int mouseGridPosition = GameManager.Instance.m_tileDataController.WorldToCell(mouseWorldPosition);
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(mouseGridPosition, 0.5f);
            }
        }
    }
}

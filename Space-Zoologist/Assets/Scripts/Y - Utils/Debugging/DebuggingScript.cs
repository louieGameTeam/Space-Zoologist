using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingScript : MonoBehaviour
{
    public TerrainDominance terrainDominance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            NeedAvailabilityCache availabilityCache = new NeedAvailabilityCache();
            NeedRatingCache ratingCache = new NeedRatingCache(availabilityCache);

            // Report the availabilities of all populations
            string message = "Population need availabilities:";
            foreach (KeyValuePair<Population, NeedAvailability> kvp in availabilityCache.PopulationNeedAvailabilities)
            {
                string json = JsonUtility.ToJson(kvp.Value, true);
                message += $"\n\t{kvp.Key.Species} at {kvp.Key.GetPosition()}: \n{json}";
            }

            // Report the ratings of all populations
            message += "\nPopulation need ratings:";
            foreach (KeyValuePair<Population, NeedRating> kvp in ratingCache.PopulationRatings)
            {
                string json = JsonUtility.ToJson(kvp.Value, true);
                message += $"\n\t{kvp.Key.Species} at {kvp.Key.GetPosition()}: \n{json}";
            }

            // Report the availabilities of all food sources
            message += "\nFood source need availabilities:";
            foreach(KeyValuePair<FoodSource, NeedAvailability> kvp in availabilityCache.FoodSourceNeedAvailabilities)
            {
                string json = JsonUtility.ToJson(kvp.Value, true);
                message += $"\n\t{kvp.Key.Species} at {kvp.Key.GetPosition()}: \n{json}";
            }

            // Report the ratings of all food sources
            message += "\nFood source need ratings:";
            foreach (KeyValuePair<FoodSource, NeedRating> kvp in ratingCache.FoodSourceRatings)
            {
                string json = JsonUtility.ToJson(kvp.Value, true);
                message += $"\n\t{kvp.Key.Species} at {kvp.Key.GetPosition()}: \n{json}";
            }

            // Log the big message
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

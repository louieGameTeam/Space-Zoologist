using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Parses and inputs data from a csv file into the LevelData object
/// </summary>
[CreateAssetMenu(fileName="LevelDataLoader", menuName="Scene Data/Level Data Loader")]
public class LevelDataLoader : ScriptableObject
{
    [Header("Click to reload, checkmark will not appear")]
    [SerializeField] private bool ReloadData = false;
    [Header("Change to load different level data")]
    [SerializeField] int Level = 1;
    [Expandable] public LevelData levelData = default;

    public void OnValidate()
    {
        if (this.ReloadData)
        {
            Debug.Log("Species Data reloaded");
            this.ReloadData = false;
            // Parse through all level data
            TextAsset speciesData = Resources.Load<TextAsset>(Path.Combine("LevelData", "Level" + Level));
            string[] data = speciesData.text.Trim().Split(new char[] { '\n'});
            int currentSpeciesIndex = 0, currentFoodSourceIndex = 0, currentItemIndex = 0;
            string[] row = data[0].Trim().Split(new char[] { ',' });
            int temp;
            int.TryParse(row[5], out temp);
            levelData.startingBalance.RuntimeValue = temp;
            for (int i=1; i<data.Length; i++)
            {
                row = data[i].Trim().Split(new char[] { ',' });
                // Create new needType object and parse the needs
                if (row[0].Equals("FoodSource", StringComparison.OrdinalIgnoreCase) && currentFoodSourceIndex < levelData.foodSources.Count)
                {
                    currentFoodSourceIndex = this.ParseFoodSources(data, row, i, currentFoodSourceIndex);
                    // Small runtime improvement
                    i+=3;
                }
                if (row[0].Equals("Species", StringComparison.OrdinalIgnoreCase) && currentSpeciesIndex < levelData.animalSpecies.Count)
                {
                    currentSpeciesIndex = this.ParseSpecies(data, row, i, currentSpeciesIndex);
                    i+=3;
                }
                if (row[0].Equals("Item", StringComparison.OrdinalIgnoreCase) && currentItemIndex < levelData.items.Count)
                {
                    currentItemIndex = this.ParseItems(row, data[i + 1].Trim().Split(new char[] { ',' }), currentItemIndex);
                }
            }
        }
    }

    private int ParseFoodSources(string[] data, string[] row, int i, int currentFoodSourceIndex)
    {
        string name = row[1];
        int rootRadius;
        int.TryParse(row[3], out rootRadius);
        int output;
        int.TryParse(row[5], out output);
        levelData.foodSources[currentFoodSourceIndex].SetupData(name, rootRadius, output, LoadNeedsData(data, i+3));
        currentFoodSourceIndex++;
        return currentFoodSourceIndex;
    }

    private int ParseItems(string[] row1, string[] row2, int currentItemIndex)
    {
        int price;
        int.TryParse(row2[1], out price);
        levelData.items[currentItemIndex].SetupData(row1[2], row1[4], row1[6], price);
        currentItemIndex++;
        return currentItemIndex;
    }

    // Manually parse fields and then parse all terrain/needs
    // TODO determine how behaviors will be parsed
    private int ParseSpecies(string[] data, string[] row, int i, int currentSpeciesIndex)
    {
        // Manually parsing fields
        string name = row[1];
        int dominance;
        int.TryParse(row[3], out dominance);
        float growthFactor;
        float.TryParse(row[5], out growthFactor);
        // Terrain
        List<String> terrain = new List<string>();
        row = data[i + 2].Trim().Split(new char[] { ',' });
        for (int j = 1;j <row.Length; j++)
        {
            if (!row[j].Equals(""))
            {
                terrain.Add(row[j]);
            }
        }
        // Needs
        levelData.animalSpecies[currentSpeciesIndex].SetupData(name, dominance, growthFactor, terrain, LoadNeedsData(data, i+3));
        currentSpeciesIndex++;
        return currentSpeciesIndex;
    }

    // Parses through all need types and their needs.
    private List<NeedTypeConstructData> LoadNeedsData(string[] data, int startIndex)
    {
        List<NeedTypeConstructData> speciesNeedData = new List<NeedTypeConstructData>();
        for (int i=startIndex; i<data.Length; i++)
        {
            string[] row = data[i].Trim().Split(new char[] { ',' });
            if (row[0].Equals("species", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            if (row[0].Equals("type", StringComparison.OrdinalIgnoreCase))
            {
                // Create new needType object and parse the needs
                speciesNeedData.Add(new NeedTypeConstructData(row[1]));
                i++;
                // Parse through all of the needs for the given type, creating all the needs
                while (i < data.Length)
                {
                    row = data[i].Trim().Split(new char[] { ',' });
                    if (row[0].Equals("type", StringComparison.OrdinalIgnoreCase) || row[0].Equals("species", StringComparison.OrdinalIgnoreCase))
                    {
                        // i-- so that type/species is still on the same line on the next iteration
                        i--;
                        break;
                    }
                    if (row[0].Equals("Name", StringComparison.OrdinalIgnoreCase))
                    {
                        speciesNeedData[speciesNeedData.Count - 1].needs.Add(ConstructNeed(data, row, i));
                    }
                    i++;
                }
            }
        }
        return speciesNeedData;
    }

    // Parse through all of the fields for the need
    private NeedConstructData ConstructNeed(string[] data, string[] row, int i)
    {
        string name = row[1];
        int severity;
        List<string> conditions = new List<string>();
        List<float> thresholds = new List<float>();
        // Parse conditions first and then parse thresholds for conditions.Count - 1
        int.TryParse(row[3], out severity);
        i++;
        row = data[i].Trim().Split(new char[] { ',' });
        for (int j=0; j<row.Length; j++)
        {
            if (row[j].Equals("bad", StringComparison.OrdinalIgnoreCase) || row[j].Equals("neutral", StringComparison.OrdinalIgnoreCase) 
            || row[j].Equals("good", StringComparison.OrdinalIgnoreCase))
            {
                conditions.Add(row[j]);
            }
        }
        row = data[i + 1].Trim().Split(new char[] { ',' });
        for (int j=1; j<conditions.Count; j++)
        {
            int threshold;
            int.TryParse(row[j], out threshold);
            thresholds.Add(threshold);
        }
        return new NeedConstructData(name, severity, conditions, thresholds);
    }
}

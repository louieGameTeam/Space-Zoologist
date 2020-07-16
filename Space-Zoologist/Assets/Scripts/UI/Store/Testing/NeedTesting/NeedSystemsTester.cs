using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedSystemsTester : MonoBehaviour
{
    [System.Serializable]
    private class TestPop
    {
        [Range(1, 50)]
        public int count = 1;
        public AnimalSpecies species = default;
    }

    [SerializeField] private Text populationStats = default;
    [SerializeField] private Text foodSourceStats = default;

    [SerializeField] private List<AnimalSpecies> availableSpecies = null;
    [SerializeField] private List<FoodSourceSpecies> availableFoodSourceSpecies = null;

    private int selectedAnimalSpecies = 0;
    private int selectedFoodSpecies = 0;

    void Start()
    {
        //rpm.UpdateAccessMap();
        //foodSourceManager.UpdateFoodSources();
    }

    private void Update()
    {
        //if (Input.GetMouseButtonUp(0))
        //{
        //    populationManager.AddAnimals(availableSpecies[selectedAnimalSpecies], 1, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0));
        //}

        //if (Input.GetMouseButtonUp(1))
        //{
        //    foodSourceManager.CreateFoodSource(availableFoodSourceSpecies[selectedFoodSpecies], new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0));
        //}

        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    needSystemManager.UpdateSystems();
        //}

        //if (Input.GetKeyUp(KeyCode.UpArrow))
        //{
        //    selectedAnimalSpecies = (selectedAnimalSpecies + 1) % availableSpecies.Count;
        //    selectedFoodSpecies = (selectedFoodSpecies + 1) % availableFoodSourceSpecies.Count;
        //}

        string populationStatsText = "";
        foreach (Population population in PopulationManager.ins.Populations)
        {
            populationStatsText += $"***{ population.Species.SpeciesName } {population.GetInstanceID()}; Count: {population.Count}; Dominance: {population.Dominance}***\n";
            foreach (KeyValuePair<string, float> needValue in population.NeedsValues)
            {
                populationStatsText += $"- {needValue.Key}: { needValue.Value } -- Condition: {population.Species.Needs[needValue.Key].GetCondition(needValue.Value)}\n";
            }
            populationStatsText += "\n";
        }
        populationStats.text = populationStatsText;

        string foodSourceStatsText = "";
        foreach (FoodSource foodSource in FoodSourceManager.ins.FoodSources)
        {
            foodSourceStatsText += $"***{ foodSource.Species.SpeciesName }; Food output: {foodSource.FoodOutput}***\n";
            foreach (KeyValuePair<string, float> needValue in foodSource.NeedsValues)
            {
                foodSourceStatsText += $"- {needValue.Key}: { needValue.Value } -- Condition: {foodSource.Species.Needs[needValue.Key].GetCondition(needValue.Value)}\n";
            }
            foodSourceStatsText += "\n";
        }
        foodSourceStats.text = foodSourceStatsText;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSystem : MonoBehaviour, INeedSystem
{

    private List<Population> Populations = new List<Population>();
    [SerializeField] private NeedType need = default;
    public NeedType Need { get => need; }
    [SerializeField] private Text populationsText = default;

    NeedType INeedSystem.Need => Need;

    public void RegisterPopulation(Population population)
    {
        Populations.Add(population);
    }

    public void UnregisterPopulation(Population population)
    {
        Populations.Remove(population);
    }

    public void UpdatePopulations()
    {
        foreach (Population population in Populations)
        {
            population.UpdateNeed(Need, CalculateNeedValue(population));
        }
    }

    private float CalculateNeedValue(Population population)
    {
        return Random.value;
    }

    private void Update()
    {
        string text = $"{gameObject.name} Listeners\n";
        foreach (Population population in Populations)
        {
            text += $"{population.SpeciesName}: {population.GetNeedValue(need)}";
            text += "\n";
        }
        populationsText.text = text;
    }
}

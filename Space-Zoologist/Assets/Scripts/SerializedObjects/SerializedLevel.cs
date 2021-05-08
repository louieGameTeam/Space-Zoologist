using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedLevel
{
    public SerializedPopulation[] serializedPopulations;
    public SerializedPlot serializedPlot;
    public void SetPopulations(PopulationManager populationManager)
    {
        this.serializedPopulations = new SerializedPopulation[populationManager.transform.childCount];
        for (int i = 0; i < populationManager.transform.childCount; i++)
        {
            GameObject populationGO = populationManager.transform.GetChild(i).gameObject;
            Vector3[] animals = new Vector3[populationGO.transform.childCount];
            for (int j = 0; j < populationGO.transform.childCount; j++)
            {
                animals[i] = populationGO.transform.GetChild(j).position;
            }
            this.serializedPopulations[i] = new SerializedPopulation(populationGO.GetComponent<Population>().species, animals);
        }
    }
    public void SetPlot(SerializedPlot serializedPlot)
    {
        this.serializedPlot = serializedPlot;
    }
}

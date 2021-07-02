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
        this.serializedPopulations = new SerializedPopulation[populationManager.gameObject.transform.childCount];
        for (int i = 0; i < populationManager.transform.childCount; i++)
        {
            GameObject populationGO = populationManager.transform.GetChild(i).gameObject;

            List<GameObject> activeChildren = new List<GameObject>();
            for (int j = 0; j < populationGO.transform.childCount; j++)
            {
                GameObject animalGO = populationGO.transform.GetChild(j).gameObject;
                if (animalGO.activeSelf)
                {
                    activeChildren.Add(animalGO);
                }
            }
            Vector3[] animalPos = new Vector3[activeChildren.Count];
            for (int k = 0; k < activeChildren.Count; k++)
            {
                animalPos[k] = activeChildren[k].transform.position;
            }
            this.serializedPopulations[i] = new SerializedPopulation(populationGO.GetComponent<Population>().species, animalPos);
        }
    }
    public void SetPlot(SerializedPlot serializedPlot)
    {
        this.serializedPlot = serializedPlot;
    }
}

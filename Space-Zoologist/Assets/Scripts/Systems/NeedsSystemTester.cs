using UnityEngine;
using System.Collections;

public class NeedsSystemTester : MonoBehaviour
{
    public float WaterValue = default;
    // Use this for initialization
    void Start()
    {
        AnimalPopulationsManager test = FindObjectOfType<AnimalPopulationsManager>().GetComponent<AnimalPopulationsManager>();
        NeedSystemManager.AddSystem(test);
    }

    public void TriggerInitialize()
    {
        EventManager.TriggerEvent("Initialize");
    }

    public void TriggerDistributeFood()
    {
        GameObject.Find("StrotPopulation").GetComponent<AnimalPopulation>().UpdateNeed("Water", this.WaterValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

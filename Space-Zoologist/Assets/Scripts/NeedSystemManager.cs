using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedSystemManager : MonoBehaviour
{

    private static Dictionary<string, INeedSystem> systems = new Dictionary<string, INeedSystem>();

    private static NeedSystemManager instance;
    public static NeedSystemManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = (new GameObject("NeedSystemManager")).AddComponent<NeedSystemManager>();
            }
            return instance;
        }
    }

    public void RegisterPopulation(Population population, string need)
    {
        systems[need].RegisterPopulation(population);
    }
    public void UnregisterPopulation(Population population, string need)
    {
        systems[need].UnregisterPopulation(population);
    }

    public static void AddSystem(INeedSystem needSystem)
    {
        systems.Add(needSystem.NeedName, needSystem);
    }
}

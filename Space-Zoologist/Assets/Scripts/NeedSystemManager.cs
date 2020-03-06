using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedSystemManager : MonoBehaviour
{

    private static Dictionary<string, INeedSystem> systems = default;

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

    public static void RegisterPopulation(Population population, string need)
    {
        systems[need].RegisterPopulation(population);
    }
    public static void UnregisterPopulation(Population population, string need)
    {
        systems[need].UnregisterPopulation(population);
    }
}

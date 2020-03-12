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

    private static void Initialize()
    {
        NeedSystemManager i = NeedSystemManager.Instance;
    }

    public static void RegisterPopulation(Population population, string need)
    {
        Instance.systems[need].RegisterPopulation(population);
    }
    public static void UnregisterPopulation(Population population, string need)
    {
        Initialize();
        systems[need].UnregisterPopulation(population);
    }

    public static void AddSystem(INeedSystem needSystem)
    {
        Initialize();
        systems.Add(needSystem.NeedName, needSystem);
    }
}

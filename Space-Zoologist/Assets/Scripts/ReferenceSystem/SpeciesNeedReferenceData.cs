using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesNeedReferenceData : MonoBehaviour
{
    [SerializeField] private List<SpeciesNeed> AddAllNeeds = new List<SpeciesNeed>();
    public Dictionary<string, SpeciesNeed> AllNeeds = new Dictionary<string, SpeciesNeed>();

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public SpeciesNeed FindNeed(string need)
    {
        if (this.AllNeeds.ContainsKey(need))
        {
            return this.AllNeeds[need];
        }
        else
        {
            return null;
        }
    }

    // Ensure the Needs are all indexed by their name
    public void OnValidate()
    {
        foreach(SpeciesNeed need in this.AddAllNeeds)
        {
            if (!this.AllNeeds.ContainsKey(need.Name.ToString()))
            {
                this.AllNeeds.Add(need.Name.ToString(), need);
            }
        }
    }
}

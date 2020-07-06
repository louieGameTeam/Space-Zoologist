using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeciesNeedReference", menuName = "ReferenceData/SpeciesNeedReference")]
public class SpeciesNeedReferenceData : ScriptableObject
{
    [SerializeField] private List<Need> AddAllNeeds = new List<Need>();
    public Dictionary<string, Need> AllNeeds = new Dictionary<string, Need>();

    public Need FindNeed(string need)
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
        foreach(Need need in this.AddAllNeeds)
        {
            if (!this.AllNeeds.ContainsKey(need.NeedName))
            {
                this.AllNeeds.Add(need.NeedName, need);
            }
        }
    }
}

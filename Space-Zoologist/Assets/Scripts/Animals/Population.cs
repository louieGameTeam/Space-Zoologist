using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    private Species species = default;
    public Species Species { get => species; private set => species = value; }
    public string SpeciesName { get => species.SpeciesName; }
    private Dictionary<string, float> Needs = new Dictionary<string, float>();
    public int Count { get; private set; }
    private Sprite sprite;
    public Sprite Sprite { get { return species.Sprite; } private set => sprite = value; }
    private Vector2Int origin = Vector2Int.zero;

    public void InitializeFromSpecies(Species species, Vector2Int origin)
    {
        this.Species = species;
        this.origin = origin;

        this.transform.position = Utils.Vector2IntToVector3Int(origin);
        this.sprite = species.Sprite;

        foreach(SpeciesNeed need in Species.Needs)
        {
            Needs.Add(need.NeedName, 0);
            NeedSystemManager.RegisterPopulation(this, need.NeedName);
        }
    }

    // Called whenever an event triggers a system to update its value
    // or when a system calls this delegated method
    public void UpdateNeed(string NeedName, float value)
    {
        if (Needs.ContainsKey(NeedName))
        {
            Needs[NeedName] = value;
            UpdatePopulationGrowthConditions();
        }
        else
        {
            Debug.Log("Need not found");
        }
    }

    /// <summary>
    /// Gets need conditions for each need based on the current values and sends them along with the need's severity to the growth formula system.
    /// </summary>
    public void UpdatePopulationGrowthConditions()
    {
        Dictionary<NeedCondition, float> needCondtions = new Dictionary<NeedCondition, float>();
        foreach(KeyValuePair<string, float> needStatus in Needs)
        {
            NeedCondition condition = species.GetNeedCondition(needStatus.Key, needStatus.Value);
            needCondtions.Add(condition, species.GetNeedSeverity(needStatus.Key));
        }
    }
}


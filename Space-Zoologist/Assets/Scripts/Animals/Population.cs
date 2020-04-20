using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A runtime instance of a species.
/// </summary>
public class Population : MonoBehaviour
{
    [SerializeField] private Species species = default;
    public Species Species => species;
    private Dictionary<NeedType, float> NeedValues = new Dictionary<NeedType, float>();
    private int count = 0;
    public int Count => count;
    private Vector2Int origin = Vector2Int.zero;

    private void Awake()
    {
        this.Initialize(species, Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.y)));
    }

    /// <summary>
    /// Initialize the population as the given species at the given origin.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin of the population</param>
    /// <param name="needSystemManager"></param>
    public void Initialize(Species species, Vector2Int origin)
    {
        this.species = species;
        this.origin = origin;

        this.transform.position = GridUtils.Vector2IntToVector3Int(origin);
        foreach(SpeciesNeed need in Species.Needs)
        {
            NeedValues.Add(need.Type, 0);
        }
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(NeedType need, float value)
    {
        Debug.Assert(NeedValues.ContainsKey(need));
        NeedValues[need] = value;
        // UpdateGrowthConditions();
    }

    /// <summary>
    /// Get the value of the given need.
    /// </summary>
    /// <param name="need">The need to get the value of</param>
    /// <returns></returns>
    public float GetNeedValue(NeedType need)
    {
        Debug.Assert(NeedValues.ContainsKey(need));

        return NeedValues[need];
    }

    // TODO: Implement
    /// <summary>
    /// Gets need conditions for each need based on the current values and sends them along with the need's severity to the growth formula system.
    /// </summary>
    public void UpdateGrowthConditions()
    {
        throw new System.NotImplementedException();
    }
}


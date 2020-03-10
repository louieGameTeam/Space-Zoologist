using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "Species", menuName = "AnimalPopulations/NewSpecies")]
public class Species : ScriptableObject
{
    [SerializeField] public string _speciesType = default;
    [SerializeField] private Sprite _sprite = default;
    [Expandable] public AnimalPopulationGrowth PopGrowth;
    [Expandable] public List<Need> Needs = default;
    [SerializeField] public int PopulationSize;
    [SerializeField] public float DominanceScore;

    public AnimalPopulationGrowth GetPopGrowth()
    {
        return this.PopGrowth;
    }

    public List<Need> GetNeeds()
    {
        return Needs;
    }

    public int GetPopulationSize()
    {
        return this.PopulationSize;
    }

    public float GetDominanceScore()
    {
        return this.DominanceScore;
    }

    public Sprite sprite
    {
        get { return _sprite; }
        private set { _sprite = value; }
    }
}

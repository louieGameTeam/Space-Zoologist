using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    private Species species = default;
    public Species Species { get => species; private set => species = value; }
    public string SpeciesName { get => species.Name; }
    private Dictionary<string, float> Needs = new Dictionary<string, float>();
    public int count { get; private set; }
    private Sprite sprite;
    public Sprite Sprite { get { return species.sprite; } private set => sprite = value; }

    public void InitializeFromSpecies(Species species)
    {
        Species = species;
        foreach(Need need in Species.Needs)
        {
            Needs.Add(need.Name, 0);
            NeedSystemManager.RegisterPopulation(this, need.Name);
        }
    }

    public void UpdateNeed(string need, float value)
    {
        Needs[need] = value;
    }
}


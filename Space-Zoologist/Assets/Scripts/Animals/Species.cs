using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Species : ScriptableObject
{
    enum SpeciesType { }
    [SerializeField] private SpeciesType _speciesType = default;
    [SerializeField] private List<Need> needs = default;
    [SerializeField] private Sprite _sprite = default;
    public Sprite sprite
    {
        get { return _sprite; }
        private set { _sprite = value; }
    }
}

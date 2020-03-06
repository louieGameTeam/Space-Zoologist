using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Species : ScriptableObject
{
    [SerializeField] private string speciesName = default;
    public string Name { get { return speciesName; } private set { speciesName = value; } }
    [SerializeField] private List<Need> needs = default;
    public List<Need> Needs { get => needs; private set => needs = value; }
    [SerializeField] private Sprite _sprite = default;
    public Sprite sprite
    {
        get { return _sprite; }
        private set { _sprite = value; }
    }   
}

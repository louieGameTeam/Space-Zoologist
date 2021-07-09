using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchCategory
{
    public ResearchCategoryType Type { get; private set; }
    public string Name => name;

    [SerializeField]
    private string name;

    public ResearchCategory(ResearchCategoryType type, string name)
    {
        Type = type;
        this.name = name;
    }

    public void Setup(ResearchCategoryType type)
    {
        Type = type;
    }
}

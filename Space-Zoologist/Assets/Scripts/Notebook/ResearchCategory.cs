using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: this MUST be a struct because other code needs to compare this by value, NOT by reference
// If this has to change to a class for some reason, we need to override the Equals() method to compare
// by value and not by reference
[System.Serializable]
public struct ResearchCategory
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

    public static bool operator==(ResearchCategory a, ResearchCategory b)
    {
        return a.Type == b.Type && a.name == b.name;
    }
    public static bool operator!=(ResearchCategory a, ResearchCategory b)
    {
        return !(a == b);
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(ResearchCategory))
        {
            return this == (ResearchCategory)obj;
        }
        else return false;
    }
    public override int GetHashCode()
    {
        return name.GetHashCode() + Type.GetHashCode();
    }
    public override string ToString()
    {
        return "ResearchCategory { " + Type + ", " + name + "}";
    }
}

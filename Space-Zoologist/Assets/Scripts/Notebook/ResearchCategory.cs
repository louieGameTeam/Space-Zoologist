using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: Check if this struct has not been initialized 
// by checking if the name is null
[System.Serializable]
public struct ResearchCategory
{
    public ResearchCategoryType Type => type;
    public string Name => name;

    [SerializeField]
    [Tooltip("Type for the research category")]
    private ResearchCategoryType type;
    [SerializeField]
    [Tooltip("Name associated with this research category")]
    private string name;

    public ResearchCategory(ResearchCategoryType type, string name)
    {
        this.type = type;
        this.name = name;
    }

    public void Setup(ResearchCategoryType type)
    {
        this.type = type;
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
        // If the other object is null, it cannot be equal to this struct
        if (obj == null) return false;
        // If the types are equal, use the operator
        else if (obj.GetType() == typeof(ResearchCategory)) return this == (ResearchCategory)obj;
        // If the types are unequal, it cannot be equal to this object
        else return false;
    }
    public override int GetHashCode()
    {
        if (name == null) Debug.Log("Research category name is apparently null");
        return name.GetHashCode() + Type.GetHashCode();
    }
    public override string ToString()
    {
        return "ResearchCategory { " + Type + ", " + name + "}";
    }
}

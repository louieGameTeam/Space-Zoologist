using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The identity of a single enclosed area.
/// Holds the name of the scene it is in, as well as the id of the area
/// </summary>
[System.Serializable]
public struct EnclosureID
{
    [SerializeField]
    [Tooltip("Name of the scene the enclosure was found in")]
    private string levelName;
    [SerializeField]
    [Tooltip("ID number of the enclosure")]
    private byte id;

    public static bool operator==(EnclosureID a, EnclosureID b)
    {
        return a.levelName == b.levelName && a.id == b.id;
    }
    public static bool operator!=(EnclosureID a, EnclosureID b)
    {
        return !(a == b);
    }
    public override bool Equals(object other)
    {
        if (other == null) return false;
        else if (other.GetType() == typeof(EnclosureID)) return this == (EnclosureID)other;
        else return false;
    }
    public override int GetHashCode()
    {
        return levelName.GetHashCode() + id.GetHashCode();
    }
    public override string ToString()
    {
        return "Enclosure ID: { " + levelName + ", " + id + " }";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemName
{
    #region Public Typedefs
    public enum Type 
    { 
        English, 
        Colloquial, 
        Science,

        // The name of the item as it will appear
        // in the JSON file used by the GameManager
        // to load up an instance of type "SerializedLevel"
        Serialized
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of names for the item - parallel to the internal 'Type' enum")]
    private string[] names = { "Goat", "Zeig", "CapraX Zeigrun", "Goat" };
    #endregion

    #region Constructors
    public ItemName()
    {
        int totalNames = System.Enum.GetNames(typeof(Type)).Length;
        names = new string[totalNames];
    }
    #endregion

    #region Data Access Methods
    public string Get(int index)
    {
        if (index >= 0 && index < names.Length) return names[index];
        else throw new IndexOutOfRangeException(
            $"No name specified at index {index}. " +
            $"Total names: {names.Length}");
    }
    public string Get(Type type) => Get((int)type);
    public void Set(int index, string name)
    {
        if (index >= 0 && index < names.Length)
        {
            names[index] = name;
        }
        else throw new IndexOutOfRangeException(
            $"No name specified at index {index}. " +
            $"Total names: {names.Length}");
    }
    public void Set(Type type, string name) => Set((int)type, name);
    #endregion

    #region Search Methods
    public bool HasName(string name)
    {
        return Contains(n => n.Equals(name));
    }
    public bool HasName(string name, StringComparison comparison)
    {
        return Contains(n => n.Equals(name, comparison));
    }
    public bool AnyNameContains(string name)
    {
        return Contains(n => n.Contains(name));
    }
    public bool Contains(Predicate<string> predicate)
    {
        // Find the index of the item matching the predicate
        int index = Array.FindIndex(names, predicate);

        // Return true if index is in range of the array
        return index >= 0 && index < names.Length;
    }
    /// <summary>
    /// Returns the name in format "scientific(colloqial)", 
    /// or "scientific" if both forms are identical
    /// </summary>
    /// <returns></returns>
    public string GetCombinedName()
    {
        // avoid duplicates if identicala
        string scientific = Get(Type.Science), colloq = Get(Type.Colloquial);
        if(scientific.CompareTo(colloq) == 0)
            return scientific;
        return scientific + $" ({colloq})";
    }
    /// <summary>
    /// Returns the item name in requested format
    /// </summary>
    /// <param name="nameType"></param>
    /// <returns></returns>
    public string GetName(Type nameType)
    {
        return Get(nameType);
    }
    #endregion

    #region Overrides
    public override string ToString()
    {
        if (names != null) return string.Join(", ", names);
        else return "(no name)";
    }
    #endregion
}

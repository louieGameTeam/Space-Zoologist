using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemName
{
    #region Public Typedefs
    public enum Type { English, Colloquial, Science }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of names for the item - parallel to the internal 'Type' enum")]
    private string[] names = { "Goat", "Zeig", "CapraX Zeigrun" };
    #endregion

    #region Constructors
    public ItemName()
    {
        int totalNames = System.Enum.GetNames(typeof(Type)).Length;
        names = new string[totalNames];
    }
    #endregion

    #region Public Methods
    public string Get(Type type) => names[(int)type];
    public string Set(Type type, string name) => names[(int)type] = name;
    #endregion

    #region Overrides
    public override string ToString()
    {
        if (names != null) return string.Join(", ", names);
        else return "(no name)";
    }
    #endregion
}

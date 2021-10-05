using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The identity of a single enclosed area.
/// Holds the name of the scene it is in, as well as the id of the area
/// </summary>
[System.Serializable]
public struct LevelID : System.IComparable<LevelID>
{
    #region Public Properties
    public int LevelNumber => levelNumber;
    public int EnclosureNumber => enclosureNumber;
    #endregion

    #region Private Editor Data
    [SerializeField]
    [Tooltip("Name of the scene the enclosure was found in")]
    private int levelNumber;
    [SerializeField]
    [Tooltip("ID number of the enclosure")]
    private int enclosureNumber;
    #endregion

    #region Constructors
    public LevelID(int levelNumber, int enclosureNumber)
    {
        this.levelNumber = levelNumber;
        this.enclosureNumber = enclosureNumber;
    }
    #endregion

    #region Factory Methods
    public static LevelID FromSceneName(string name)
    {
        string levelPrefix = "Level";
        string enclosurePrefix = "E";

        // Search for the place in the string where the level number is
        int levelIndex = name.IndexOf(levelPrefix);
        if (levelIndex >= 0) levelIndex += levelPrefix.Length;
        // Search for the place in the string where the enclosure is identified
        int enclosureIndexSearchStart = levelIndex >= 0 && levelIndex < name.Length ? levelIndex : 0;
        int enclosureIndex = name.IndexOf(enclosurePrefix, enclosureIndexSearchStart);
        if (enclosureIndex >= 0) enclosureIndex += enclosurePrefix.Length;

        // Get the substrings where the numbers are
        string levelNumberString = string.Empty;
        string enclosureNumberString = string.Empty;

        if (levelIndex >= 0 && levelIndex < name.Length)
        {
            if (enclosureIndex >= 0 && enclosureIndex < name.Length)
            {
                levelNumberString = name.Substring(levelIndex, enclosureIndex - levelIndex - 1);
                enclosureNumberString = name.Substring(enclosureIndex);
            }
            else levelNumberString = name.Substring(levelIndex);
        }

        // Try to parse the level number string and enclosure number string
        if (!int.TryParse(levelNumberString, out int levelNumber))
        {
            levelNumber = -1;
        }
        if (!int.TryParse(enclosureNumberString, out int enclosureNumber))
        {
            enclosureNumber = 1;
        }

        // Return the enclosure ID
        return new LevelID(levelNumber, enclosureNumber);
    }

    // Parse the scene name to get the enclosure ID
    public static LevelID FromCurrentSceneName()
    {
        GameManager instance = GameManager.Instance;

        if(instance)
        {
            return FromSceneName(instance.LevelData.Level.SceneName);
        }
        else return new LevelID(-1, -1);
    }
    #endregion

    #region Operators
    public static bool operator ==(LevelID a, LevelID b) => a.CompareTo(b) == 0;
    public static bool operator !=(LevelID a, LevelID b) => !(a == b);
    public static bool operator <(LevelID a, LevelID b) => a.CompareTo(b) < 0;
    public static bool operator >=(LevelID a, LevelID b) => !(a < b);
    public static bool operator >(LevelID a, LevelID b) => a.CompareTo(b) > 0;
    public static bool operator <=(LevelID a, LevelID b) => !(a > b);
    #endregion

    #region Object Overrides
    public override bool Equals(object other)
    {
        if (other == null) return false;
        else if (other.GetType() == typeof(LevelID)) return CompareTo((LevelID)other) == 0;
        else return false;
    }
    public override int GetHashCode()
    {
        return levelNumber.GetHashCode() + enclosureNumber.GetHashCode();
    }
    public override string ToString()
    {
        return "Enclosure ID: { " + levelNumber + ", " + enclosureNumber + " }";
    }
    #endregion

    #region Interface Implementation
    public int CompareTo(LevelID other)
    {
        int levelCompare = levelNumber.CompareTo(other.levelNumber);

        // If levels are equal, compare the enclosures
        if (levelCompare == 0) return enclosureNumber.CompareTo(other.enclosureNumber);
        // Otherwise return the result of the level compare
        else return levelCompare;
    }
    #endregion
}

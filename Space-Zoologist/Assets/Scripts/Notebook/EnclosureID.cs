using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The identity of a single enclosed area.
/// Holds the name of the scene it is in, as well as the id of the area
/// </summary>
[System.Serializable]
public struct EnclosureID
{
    public int LevelNumber => levelNumber;
    public int EnclosureNumber => enclosureNumber;

    [SerializeField]
    [Tooltip("Name of the scene the enclosure was found in")]
    private int levelNumber;
    [SerializeField]
    [Tooltip("ID number of the enclosure")]
    private int enclosureNumber;

    public EnclosureID(int levelNumber, int enclosureNumber)
    {
        this.levelNumber = levelNumber;
        this.enclosureNumber = enclosureNumber;
    }

    // Parse the scene name to get the enclosure ID
    public static EnclosureID FromCurrentSceneName()
    {
        // Get the name of the active scene
        string name = SceneManager.GetActiveScene().name;
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

        if(levelIndex >= 0 && levelIndex < name.Length)
        {
            if (enclosureIndex >= 0 && enclosureIndex < name.Length)
            {
                levelNumberString = name.Substring(levelIndex, enclosureIndex - levelIndex);
                enclosureNumberString = name.Substring(enclosureIndex);
            }
            else levelNumberString = name.Substring(levelIndex);
        }

        // Try to parse the level number string and enclosure number string
        if(!int.TryParse(levelNumberString, out int levelNumber))
        {
            levelNumber = -1;
        }
        if(!int.TryParse(enclosureNumberString, out int enclosureNumber))
        {
            enclosureNumber = 1;
        }

        // Return the enclosure ID
        return new EnclosureID(levelNumber, enclosureNumber);
    }

    public static bool operator==(EnclosureID a, EnclosureID b)
    {
        return a.levelNumber == b.levelNumber && a.enclosureNumber == b.enclosureNumber;
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
        return levelNumber.GetHashCode() + enclosureNumber.GetHashCode();
    }
    public override string ToString()
    {
        return "Enclosure ID: { " + levelNumber + ", " + enclosureNumber + " }";
    }
}

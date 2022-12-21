using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    #region Public Properties
    public static LevelID LatestLevelQualified => Instance.latestLevelQualified;
    public static LevelID LatestLevelIntroFinished => Instance.latestLevelIntroFinished;
    #endregion

    #region Private Properties
    private static SaveData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Load();
            }
            return instance;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("ID of the latest level that the player is qualified to attempt")]
    private LevelID latestLevelQualified = new LevelID(0, 1);
    [SerializeField]
    [Tooltip("ID of the latest level that the player has finished the intro for")]
    private LevelID latestLevelIntroFinished = new LevelID(0, 0);
    [SerializeField]
    [Tooltip("List of completed levels with their ratings")]
    private List<LevelRating> levelRatings = new List<LevelRating>();
    #endregion

    #region Public Fields
    public static readonly string fileName = "spacez.save";
    public static readonly string filePath = Path.Combine(Application.persistentDataPath, fileName);
    #endregion

    #region Private Fields
    private static readonly BinaryFormatter formatter = new BinaryFormatter();
    private static SaveData instance;
    #endregion

    #region Public Methods
    public static void QualifyForLevel(LevelID level)
    {
        if (level > Instance.latestLevelQualified)
        {
            Instance.latestLevelQualified = level;
        }
    }
    public static int GetLevelRating(LevelID level)
    {
        // Find a rating for the given level
        if(Instance.levelRatings == null)
        {
            Instance.levelRatings = new List<LevelRating>();
        }
        int index = Instance.levelRatings.FindIndex(x => x.ID == level);

        // If the level was found then return the rating
        if (index >= 0) return Instance.levelRatings[index].Rating;
        // If the level was not found then return an invalid rating
        else return -1;
    }
    public static void SetLevelRating(LevelID level, int rating)
    {
        // Search for a rating with the given id
        int index = Instance.levelRatings.FindIndex(x => x.ID == level);

        // Check if a rating with the id was found
        if (index >= 0)
        {
            // Get the rating
            LevelRating levelRating = Instance.levelRatings[index];

            // If the current rating is less than the new one, then update the current rating
            if (levelRating.Rating < rating)
            {
                Instance.levelRatings[index] = new LevelRating(level, rating);
            }
        }
        // If no rating exists for the current id then add a new rating
        else Instance.levelRatings.Add(new LevelRating(level, rating));
    }

    public static void TrySetLatestLevelIntro(LevelID id)
    {
        //If the latest level Intro finished is less, than update
        if (Instance.latestLevelIntroFinished < id)
        {
            Instance.latestLevelIntroFinished = id;
        }
    }
    #endregion

    #region File Manipulation Methods
    public static SaveData Load()
    {
        // If save file exists then load the contents
        if (SaveFileExists())
        {
            using (FileStream file = File.OpenRead(filePath))
            {
                //Debug.Log (Application.persistentDataPath);
                SaveData data = (SaveData)formatter.Deserialize(file);
                return data;
            }
        }
        // If save file does not exist then return a brand new save data
        else return new SaveData();
    }
    public static void Save()
    {
        // Instance property might open the file to load, 
        // so we store the reference first to prevent a sharing violation
        SaveData data = Instance;

        using (FileStream file = File.OpenWrite(filePath))
        {
            formatter.Serialize(file, data);
        }
    }
    public static void Delete()
    {
        if (SaveFileExists())
        {
            File.Delete(filePath);
            instance = null;
        }
    }
    public static bool SaveFileExists() => File.Exists(filePath);
    #endregion
}

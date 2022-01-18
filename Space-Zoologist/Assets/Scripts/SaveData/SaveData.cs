using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    #region Public Properties
    public static LevelID LatestLevelQualified => Instance.latestLevelQualified;
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
    private LevelID latestLevelQualified = new LevelID(2, 4);
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
            Save();
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

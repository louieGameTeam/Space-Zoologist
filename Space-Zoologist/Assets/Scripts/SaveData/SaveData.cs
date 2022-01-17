using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    #region Public Properties
    public static LevelID LatestLevelCompleted => Instance.latestLevelCompleted;
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
    [Tooltip("Name of the latest level completed by the player")]
    private LevelID latestLevelCompleted = new LevelID(0, 1);
    #endregion

    #region Public Fields
    public static readonly string fileName = "sz.save";
    public static readonly string filePath = Path.Combine(Application.persistentDataPath, fileName);
    #endregion

    #region Private Fields
    private static readonly BinaryFormatter formatter = new BinaryFormatter();
    private static SaveData instance;
    #endregion

    #region Public Methods
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

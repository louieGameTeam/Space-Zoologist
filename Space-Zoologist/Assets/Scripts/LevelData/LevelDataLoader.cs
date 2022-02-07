using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDataLoader : MonoBehaviour
{
    #region Public Properties
    public static string CurrentLevel => currentLevel;
    #endregion

    #region Editor Fields
    [Header("Used when playing level scene directly")]
    [SerializeField] string LevelOnPlay = "Level1E1";
    #endregion

    #region Private Fields
    private static string currentLevel = "";
    #endregion

    #region Monobehaviour Messages
    private void Awake()
    {
        // If the current level is empty then use the level on play
        if (currentLevel == "")
        {
            currentLevel = LevelOnPlay;
        }
        LevelDataReference.instance.LevelData = GetLevelData(currentLevel);
    }
    #endregion

    #region Public Methods
    public static void LoadLatestQualifiedLevel() => LoadLevel(SaveData.LatestLevelQualified);
    public static void LoadLevel(LevelData level) => LoadLevel(level.Level.SceneName);
    public static void LoadLevel(LevelID levelToLoad) => LoadLevel(levelToLoad.LevelName);
    public static void LoadLevel(string levelToLoad)
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.HandleExitLevel();
        }
        // Set the current level to the level we are about to load
        currentLevel = levelToLoad;
        SceneManager.LoadScene("MainLevel"); 
    }
    public static void ReloadLevel() => LoadLevel(currentLevel);
    public static void LoadNextLevel()
    {
        LevelID nextLevel = GameManager.Instance.LevelData.Ending.GetNextLevelID();

        // If we got the next level then load it
        if (nextLevel != LevelID.Invalid)
        {
            LoadLevel(nextLevel);
        }
        else Debug.Log($"Next level name could not be identified. " +
            $"Make sure that the player has finished taking the end of level quiz " +
            $"before trying to load the next level");
    }
    public static LevelData GetLevelData(LevelID levelID)
    {
        // Setup the path to use the enclosure number only if it is not level 0
        string path = $"LevelData/Level{levelID.LevelNumber}/L{levelID.LevelNumber}";
        if (levelID.LevelNumber != 0) path += $"E{levelID.EnclosureNumber}";
        path += "Data";

        // Load the data at the computed path
        LevelData data = Resources.Load<LevelData>(path);

        // If you got level data then return it
        if (data) return data;
        // If you did not get level data then throw an exception
        else throw new MissingReferenceException($"{nameof(LevelDataLoader)}: " +
            $"Failed to load level {levelID.LevelName} from resource path {path}");
    }
    public static LevelData GetLevelData(string levelToLoad) => GetLevelData(LevelID.FromSceneName(levelToLoad));
    public static LevelData[] GetAllLevelEnclosures(int levelNumber)
    {
        string path = $"LevelData/Level{levelNumber}/";
        return Resources.LoadAll<LevelData>(path);
    }
    public static LevelData[] GetAllLevelData() => Resources.LoadAll<LevelData>("");
    public static int MaxLevel()
    {
        LevelData[] levels = GetAllLevelData();
        return levels
            .Select(level => LevelID.FromSceneName(level.Level.SceneName).LevelNumber)
            .Max();
    }
    #endregion

}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDataLoader : MonoBehaviour
{
    #region Public Properties
    public static LevelData CurrentLevel => currentLevel;
    #endregion

    #region Editor Fields
    [Header("Used when playing level scene directly")]
    [SerializeField] LevelData levelOnPlay = null;
    #endregion

    #region Private Fields
    private static LevelData currentLevel;
    #endregion

    #region Monobehaviour Messages
    private void Awake()
    {
        // If the current level is empty then use the level on play
        if (currentLevel == null)
        {
            currentLevel = levelOnPlay;
        }
        LevelDataReference.instance.LevelData = currentLevel;
    }
    private void OnDrawGizmos()
    {
        if (currentLevel)
        {
            Gizmos.color = Color.green;
            foreach (Vector3Int start in currentLevel.StartinPositions)
            {
                Gizmos.DrawSphere(start, 0.5f);
            }
        }
    }
    #endregion

    #region Public Methods
    public static void LoadLatestQualifiedLevel() => LoadLevel(SaveData.LatestLevelQualified);
    public static void LoadLevel(string levelToLoad) => LoadLevel(GetLevelData(levelToLoad));
    public static void LoadLevel(LevelID levelToLoad) => LoadLevel(GetLevelData(levelToLoad));
    public static void LoadLevel(LevelData level)
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.HandleExitLevel();
        }
        // Set the current level to the level we are about to load
        currentLevel = level;
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
    public static LevelData GetLevelData(string levelToLoad)
    {
        LevelID id = LevelID.FromSceneName(levelToLoad);

        if (id.IsValid)
        {
            return GetLevelData(id);
        }
        else throw new System.ArgumentException($"Level '{levelToLoad}' " +
            $"could not be parsed into a valid LevelID object");
    }
    public static LevelData[] GetAllLevelEnclosures(int levelNumber)
    {
        string path = $"LevelData/Level{levelNumber}/";
        return Resources.LoadAll<LevelData>(path);
    }
    // TODO: Load this data once and cache it, rather than loading it multiple times
    // See GetAllLevelData, GetAllLevelEnclosures, and GetLevelData, for starters
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

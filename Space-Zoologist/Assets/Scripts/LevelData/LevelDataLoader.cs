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
    [Expandable] public List<LevelData> levelDatas = new List<LevelData>();
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
        LevelDataReference.instance.LevelData = GetLevelData(LevelOnPlay);
    }
    #endregion

    #region Public Methods
    public static void LoadLevel(LevelID levelToLoad) => LoadLevel(levelToLoad.LevelName);
    public static void LoadLevel(string levelToLoad)
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.HandleExitLevel();
        }
        // Set the current level to the level we are about to load
        currentLevel = levelToLoad;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
    public static void ReloadLevel() => LoadLevel(currentLevel);

    public void LoadNextLevel()
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
    public LevelData GetLevelData(LevelID levelID) => GetLevelData(levelID.LevelName);
    #endregion

    #region Private Methods
    private LevelData GetLevelData(string levelToLoad)
    {
        currentLevel = levelToLoad;
        foreach (LevelData levelData in levelDatas)
        {
            if (levelData)
            {
                if (levelData.Level.SceneName.Equals(levelToLoad))
                {
                    return levelData;
                }
            }
        }
        return null;
    }
    #endregion
}

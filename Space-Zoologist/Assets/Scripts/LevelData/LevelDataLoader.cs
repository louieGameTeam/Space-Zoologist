using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDataLoader : MonoBehaviour
{
    [Expandable] public List<LevelData> levelDatas = new List<LevelData>();
    [Header("Used when playing level scene directly")]
    [SerializeField] string LevelOnPlay = "Level1E1";
    string currentLevel = "Level1E1";

    private void Awake()
    {
        int LevelDataLoader = FindObjectsOfType<LevelDataLoader>().Length;
        if (LevelDataLoader != 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            LevelMenuSelector selectedLevel = FindObjectOfType<LevelMenuSelector>();
            if (selectedLevel != null)
            {
                LevelDataReference.instance.LevelData = GetLevelData(selectedLevel.levelName);
                Destroy(selectedLevel.gameObject);
            }
            else
            {
                LevelDataReference.instance.LevelData = GetLevelData(LevelOnPlay);
            }
            DontDestroyOnLoad(this);
        }
    }

    public void LoadLevel(LevelID levelToLoad) => LoadLevel(levelToLoad.LevelName);
    public void LoadLevel(string levelToLoad)
    {
        GameManager.Instance?.HandleExitLevel();
        LevelDataReference.instance.LevelData = GetLevelData(levelToLoad);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

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

    public void ReloadLevel()
    {
        GameManager.Instance?.HandleExitLevel();
        LevelDataReference.instance.LevelData = GetLevelData(currentLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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

    public LevelData GetLevelData(LevelID levelID) => GetLevelData(levelID.LevelName);
}

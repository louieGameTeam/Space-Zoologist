using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDataLoader : MonoBehaviour
{
    [Expandable] public List<LevelData> levelDatas = new List<LevelData>();
    [Header("Used when playing level scene directly")]
    [SerializeField] string LevelOnPlay = "Level1E1";
    private LevelDataReference levelDataReference = default;
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
            CurrentLevel level = FindObjectOfType<CurrentLevel>();
            levelDataReference = FindObjectOfType<LevelDataReference>();
            if (level != null)
            {
                UpdateLevelData(level.levelName);
                Destroy(level.gameObject);
            }
            else
            {
                UpdateLevelData(LevelOnPlay);
            }
            DontDestroyOnLoad(this);
        }
    }

    public void LoadLevel(string levelToLoad)
    {
        Debug.Log("Loading: " + levelToLoad);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UpdateLevelData(levelToLoad);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (levelDataReference == null)
        {
            return;
        }
        UpdateLevelData(currentLevel);
    }

    private void UpdateLevelData(string levelToLoad)
    {
        currentLevel = levelToLoad;
        foreach (LevelData levelData in levelDatas)
        {
            if (levelData.Level.SceneName.Equals(levelToLoad))
            {
                Debug.Log("Updated level data reference: " + levelToLoad);
                levelDataReference.LevelData = levelData;
                break;
            }
        }
    }
}

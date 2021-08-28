using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName="Scene Navigator", menuName="Scene Data/Scene Navigator")]
public class SceneNavigator : ScriptableObject
{
    [Expandable] public List<Level> Levels = default;
    public string RecentlyLoadedLevel { get; private set; }

    public void LoadLevel(string levelName)
    {
        this.UpdateRecentlyLoadedLevel(levelName);
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public void RestartLevel() {
        LoadLevel(RecentlyLoadedLevel);
    }

    public void LoadNextLevel() {
        for (int i = 0; i < Levels.Count; i++) {
            if (Levels[i].SceneName == RecentlyLoadedLevel) {
                // found current level, load the next one
                LoadLevel(Levels[i + 1].SceneName);
                return;
            }
        }
        Debug.LogError("Scene Navigator: Did not find current level. Check if level name matches scene name.");
    }

    public void LoadMainMenu()
    {
        LoadLevel("MainMenu");
    }

    public void LoadLevelMenu()
    {
        LoadLevel("LevelMenu");
    }

    private void UpdateRecentlyLoadedLevel(string levelName)
    {
        this.RecentlyLoadedLevel = levelName;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

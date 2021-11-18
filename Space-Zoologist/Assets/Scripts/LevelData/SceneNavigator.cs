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
        GameManager.Instance?.HandleExitLevel();
        this.UpdateRecentlyLoadedLevel(levelName);
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public void RestartLevel()
    {
        LoadLevel("MainLevel");
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

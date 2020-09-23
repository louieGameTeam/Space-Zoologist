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

    public void LoadMainMenu()
    {
        this.UpdateRecentlyLoadedLevel("MainMenu");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void LoadLevelMenu()
    {
        this.UpdateRecentlyLoadedLevel("LevelMenu");
        SceneManager.LoadScene("LevelMenu", LoadSceneMode.Single);
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

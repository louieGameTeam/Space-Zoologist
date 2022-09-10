using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName="Scene Navigator", menuName="Scene Data/Scene Navigator")]
public class SceneNavigator : ScriptableObject
{
    [Expandable] public List<Level> Levels = default;
    public static string RecentlyLoadedLevel { get; private set; }

    public static void LoadLevel(string levelName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (GameManager.Instance)
        {
            Debug.Log("Got a game manager");
            GameManager.Instance.HandleExitLevel();
        }
        else Debug.Log("Did not get a game manager");

        UpdateRecentlyLoadedLevel(levelName);
        SceneManager.LoadScene(levelName, mode);
    }

    private static void UpdateRecentlyLoadedLevel(string levelName)
    {
        RecentlyLoadedLevel = levelName;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

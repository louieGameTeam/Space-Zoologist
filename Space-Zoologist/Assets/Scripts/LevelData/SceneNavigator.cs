using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName="Scene Navigator", menuName="Scene Data/Scene Navigator")]
public class SceneNavigator : ScriptableObject
{
    [Expandable] public List<Level> Levels = default;
    public static string RecentlyLoadedLevel { get; private set; }

    // Used by scripts
    public static void LoadScene (string levelName) {
        if (GameManager.Instance) {
            Debug.Log ("Got a game manager");
            GameManager.Instance.HandleExitLevel ();
        } else Debug.Log ("Did not get a game manager");

        UpdateRecentlyLoadedLevel (levelName);
        LevelLoadEffectsHandler.Instance.StartCoroutine (LevelLoadEffectsHandler.SceneTransition (levelName));

    }

    // Used by UI buttons in prefabs
    public void LoadLevel (string levelName)
    {
        if (GameManager.Instance)
        {
            Debug.Log("Got a game manager");
            GameManager.Instance.HandleExitLevel();
        }
        else Debug.Log("Did not get a game manager");

        UpdateRecentlyLoadedLevel(levelName);
        LevelLoadEffectsHandler.Instance.StartCoroutine (LevelLoadEffectsHandler.SceneTransition (levelName));
    }

    public void RestartLevel () {
        if (GameManager.Instance) {
            Debug.Log ("Got a game manager");
            GameManager.Instance.HandleExitLevel ();
        } else Debug.Log ("Did not get a game manager");

        LevelLoadEffectsHandler.Instance.StartCoroutine (LevelLoadEffectsHandler.SceneTransition (RecentlyLoadedLevel));
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

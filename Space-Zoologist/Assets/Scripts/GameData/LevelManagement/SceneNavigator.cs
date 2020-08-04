using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName="Scene Navigator", menuName="Scene Data/Scene Navigator")]
public class SceneNavigator : ScriptableObject
{
    [Expandable] public List<Level> Levels = default;

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void LoadLevelMenu()
    {
        SceneManager.LoadScene("LevelMenu", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

// TODO resolve scrolling issue when more levels are added beyond screen

public class LevelNavigator : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void SendCurrentLevelToWeb(string levelName);




    // Then create a function that is going to trigger
    // the imported function from our JSLib.



    [SerializeField] SceneNavigator SceneNavigator = default;
    [SerializeField] GameObject LevelUIPrefab = default;
    [SerializeField] GameObject LevelContent = default;
    public List<GameObject> DisplayedLevels = default;

    public void Start()
    {
        this.DisplayedLevels = new List<GameObject>();
        this.InitializeLevelDisplay();
    }

    private void InitializeLevelDisplay()
    {
        foreach (Level level in this.SceneNavigator.Levels)
        {
            GameObject newLevel = Instantiate(LevelUIPrefab, LevelContent.transform);
            newLevel.GetComponent<LevelUI>().InitializeLevelUI(level);
            newLevel.GetComponent<Button>().onClick.AddListener(() => {
                this.SceneNavigator.LoadLevel(level.SceneName);
                //try
                //{
                //    SendCurrentLevelToWeb(level.SceneName);
                //}
                //catch
                //{
                //    Debug.Log("Level loaded, not hooked up to React");
                //}

            });
            this.DisplayedLevels.Add(newLevel);
        }
    }
}

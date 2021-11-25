using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelNavigator : MonoBehaviour
{
    [SerializeField] SceneNavigator SceneNavigator = default;
    [SerializeField] LevelDataLoader LevelLoader = default;
    [SerializeField] GameObject LevelUIPrefab = default;
    [SerializeField] GameObject LevelContent = default;
    public List<GameObject> DisplayedLevels = default;
    private LevelMenuSelector currentLevel = default;
    int lastLvl;
    int lastEnc;

    public void Start()
    {
        string LastLevel = GameManager.LoadGame();
        lastLvl = GameManager.ExtractLevelInfo(LastLevel)[0];
        lastEnc = GameManager.ExtractLevelInfo(LastLevel)[1];
        this.DisplayedLevels = new List<GameObject>();
        this.InitializeLevelDisplay();
        currentLevel = FindObjectOfType<LevelMenuSelector>();
    }

    private void InitializeLevelDisplay()
    {
        foreach (LevelData level in this.LevelLoader.levelDatas)
        {
            GameObject newLevel = Instantiate(LevelUIPrefab, LevelContent.transform);
            newLevel.GetComponent<LevelUI>().InitializeLevelUI(level.Level);
            newLevel.GetComponent<Button>().onClick.AddListener(() => {
                currentLevel.levelName = level.Level.SceneName;
                SceneNavigator.LoadLevel("MainLevel");
            });
            int[] levelInfo = GameManager.ExtractLevelInfo(level.Level.SceneName);
            if (lastLvl < levelInfo[0] || lastLvl == levelInfo[0] && levelInfo[1] != lastEnc) {
                //if (levelInfo[0] != 0) print($"{levelInfo[0]}:{levelInfo[1]}, {lastLvl}:{lastEnc}");
                newLevel.GetComponent<Button>().interactable = false;
            }
            this.DisplayedLevels.Add(newLevel);
        }
    }
}

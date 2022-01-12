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
            int[] levelInfo = GameManager.ExtractLevelInfo(level.Level.SceneName);

            // Hidden levels
            // This makes Level2 always hidden. Shouldn't this use a save file to determine what levels the player has completed,
            // and determine the unlock state of each level in the level data that way?
            if(lastLvl == levelInfo[0] && levelInfo[1] != lastEnc || lastLvl < levelInfo[0] && levelInfo[1] != 1)
            {
                continue;
            }
            else {
                GameObject newLevel = InstantiateLevel(level);

                // Locked or autoselect level
                if (lastLvl < levelInfo[0] && levelInfo[1] == 1)
                {
                    newLevel.GetComponent<LevelUI>().SetName($"Level {levelInfo[0]}");
                    newLevel.GetComponent<Button>().interactable = false;
                }
                else if (lastLvl == levelInfo[0] && levelInfo[1] == lastEnc)
                {
                    newLevel.GetComponent<LevelUI>().SetName($"Level {levelInfo[0]}");
                }

                this.DisplayedLevels.Add(newLevel);
            }
        }
    }
    private GameObject InstantiateLevel(LevelData level)
    {
        GameObject newLevel = Instantiate(LevelUIPrefab, LevelContent.transform);
        newLevel.GetComponent<LevelUI>().InitializeLevelUI(level.Level);
        newLevel.GetComponent<Button>().onClick.AddListener(() => {
            currentLevel.levelName = level.Level.SceneName;
            SceneNavigator.LoadLevel("MainLevel");
        });
        return newLevel;
    }
}

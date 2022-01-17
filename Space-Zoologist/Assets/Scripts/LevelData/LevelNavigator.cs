using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelNavigator : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the level select UI used to select a level")]
    private LevelSelectUI levelUIPrefab;
    [SerializeField]
    [Tooltip("Layout group used to display all level ui selectors")]
    private LayoutGroup levelUIGroup;
    #endregion

    public void Start()
    {
        //string LastLevel = GameManager.LoadGame();
        //int lastLvl = GameManager.ExtractLevelInfo(LastLevel)[0];
        //int lastEnc = GameManager.ExtractLevelInfo(LastLevel)[1];

        // Create a level select UI for every level
        int maxLevel = LevelDataLoader.MaxLevel();
        for(int level = 0; level <= maxLevel; level++)
        {
            LevelSelectUI ui = Instantiate(levelUIPrefab, levelUIGroup.transform);
            ui.Setup(level);
        }
     }
}

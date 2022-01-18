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

    #region Private Fields
    private LevelSelectUI[] uis;
    #endregion

    #region Monobehaviour Messages
    public void Start()
    {
        // Create a level select UI for every level
        int maxLevel = LevelDataLoader.MaxLevel();
        uis = new LevelSelectUI[maxLevel + 1];

        for(int level = 0; level <= maxLevel; level++)
        {
            uis[level] = Instantiate(levelUIPrefab, levelUIGroup.transform);
            uis[level].Setup(level);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (uis[0].Overridden)
            {
                foreach (LevelSelectUI ui in uis)
                {
                    ui.ClearOverride();
                }
            }
            else
            {
                // Get the level just after the max level
                int maxLevel = LevelDataLoader.MaxLevel() + 1;
                LevelID maxID = new LevelID(maxLevel, 1);

                // Override all ui's to the max level
                foreach(LevelSelectUI ui in uis)
                {
                    ui.SetOverride(maxID);
                }
            }
        }
    }
    #endregion
}

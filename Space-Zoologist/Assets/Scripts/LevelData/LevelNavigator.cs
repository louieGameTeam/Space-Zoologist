using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelNavigator : MonoBehaviour
{
    #region Public Properties
    // Assume the level navigator is overridden if just one level is overridden
    public bool Overridden => uis[0].Overridden;
    #endregion

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
    #endregion

    #region Public Methods
    public void SetOverride(LevelID overrideID)
    {
        foreach (LevelSelectUI ui in uis)
        {
            ui.SetOverride(overrideID);
        }
    }
    public void ClearOverride()
    {
        foreach (LevelSelectUI ui in uis)
        {
            ui.ClearOverride();
        }
    }
    public void ToggleOverride(LevelID overrideID)
    {
        if (Overridden) ClearOverride();
        else SetOverride(overrideID);
    }
    #endregion
}

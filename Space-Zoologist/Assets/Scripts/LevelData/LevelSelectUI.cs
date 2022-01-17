using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Prefab used to select each individual enslosure for this level")]
    private LevelSelectEnclosureUI enclosureUI;
    [SerializeField]
    [Tooltip("Layout group used to display all enslosure selections")]
    private LayoutGroup enclosureUIGroup;
    [SerializeField]
    [Tooltip("Button used to pick the current enclosure that the player is on when they have not finished the level yet")]
    private Button levelSelectButton;
    #endregion

    #region Private Fields
    private LevelSelectEnclosureUI[] enclosureUIs;
    #endregion

    #region Public Methods
    public void Setup(int levelNumber)
    {
        // Get a list of all enclosures for this level
        LevelData[] enclosures = LevelDataLoader.GetAllLevelEnclosures(levelNumber);
        enclosureUIs = new LevelSelectEnclosureUI[enclosures.Length];

        // Create a UI to select each enclosure for this level
        for(int i = 0; i < enclosures.Length; i++)
        {
            enclosureUIs[i] = Instantiate(enclosureUI, enclosureUIGroup.transform);
            enclosureUIs[i].Setup(enclosures[i]);
        }

        // TODO: change so that it loads the latest incomplete enclosure
        // The button should also be disabled if all levels are complete
        levelSelectButton.onClick.AddListener(enclosureUIs[0].LoadLevel);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelSelectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Public Properties
    /*#if UNITY_EDITOR
    public bool Interactable => false;
    #else*/
    public bool Interactable => false /*LatestLevelQualified.LevelNumber == levelNumber && levelNumber <= Statics.LatestUnlockableLevel*/;
    //#endif
    public bool Overridden => overridden;
    #endregion

    #region Private Properties
    private LevelID LatestLevelQualified
    {
        get
        {
            if (overridden)
            {
                return levelOverride;
            }
            else return SaveData.LatestLevelQualified;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Prefab used to select each individual enslosure for this level")]
    private LevelSelectEnclosureUI enclosureUI = null;
    [SerializeField]
    [Tooltip("Layout group used to display all enslosure selections")]
    private LayoutGroup enclosureUIGroup = null;
    [SerializeField]
    [Tooltip("Outline that appears when the ui is hovered over")]
    private GameObject outline = null;
    [SerializeField]
    [Tooltip("Sector Number Text")]
    private TMP_Text sectorNumberText;
    #endregion

    #region Private Fields
    private int levelNumber;
    private LevelSelectEnclosureUI[] enclosureUIs;
    private bool overridden = false;
    private LevelID levelOverride = LevelID.Invalid;
    #endregion

    #region Public Methods
    public void Setup(int levelNumber)
    {
        // Set the level number state
        this.levelNumber = levelNumber;

        // Get a list of all enclosures for this level
        LevelData[] enclosures = LevelDataLoader.GetAllLevelEnclosures(levelNumber);
        enclosureUIs = new LevelSelectEnclosureUI[enclosures.Length];

        // Create a UI to select each enclosure for this level
        for(int i = 0; i < enclosures.Length; i++)
        {
            // If the level SO specifies a prefab override, use that inside
            // This is used for specially formatted enclosure UI, like the tutorial enclosure
            var overridePrefab = enclosures[i].Level.EnclosureUIPrefabOverride;
            
            if(overridePrefab)
                enclosureUIs[i] = Instantiate(overridePrefab, enclosureUIGroup.transform);
            else
                enclosureUIs[i] = Instantiate(enclosureUI, enclosureUIGroup.transform);
            
            enclosureUIs[i].Setup(
                enclosures[i]);
        }

        // Disable the outline
        outline.SetActive(false);
        outline.transform.SetAsLastSibling();

        sectorNumberText.text = levelNumber.ToString();
    }
    public void SetOverride(LevelID levelOverride)
    {
        overridden = true;
        this.levelOverride = levelOverride;

        // Disable outline in case this makes the ui not interactable anymore
        outline.SetActive(false);
        // Override the latest level qualified for all enclosure uis
        foreach(LevelSelectEnclosureUI ui in enclosureUIs)
        {
            ui.SetOverride(levelOverride);
        }
    }
    public void ClearOverride()
    {
        overridden = false;

        // Clear the overrides for all the uis
        foreach(LevelSelectEnclosureUI ui in enclosureUIs)
        {
            ui.ClearOverride();
        }
    }
    #endregion

    #region Pointer Interface Implementations
    public void OnPointerEnter(PointerEventData data)
    {
        if (Interactable)
        {
            outline.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData data)
    {
        if (Interactable)
        {
            outline.SetActive(false);
        }
    }
    public void OnPointerClick(PointerEventData data)
    {
        if (Interactable)
        {
            LevelDataLoader.LoadLatestQualifiedLevel();
        }
    }
    #endregion
}

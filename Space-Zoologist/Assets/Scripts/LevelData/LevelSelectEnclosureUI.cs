using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelSelectEnclosureUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Public Properties
    //#if UNITY_EDITOR
    //public bool Interactable => true;
    //#else
    public bool Interactable => LatestLevelQualified.LevelNumber >= enclosure.Level.ID.LevelNumber && 
                                (LatestLevelQualified.LevelNumber == enclosure.Level.ID.LevelNumber ? LatestLevelQualified.EnclosureNumber >= enclosure.Level.ID.EnclosureNumber : true);
    //#endif
    #endregion

    #region Private Properties
    private LevelID LatestLevelQualified
    {
        get
        {
            if (overridden) return levelOverride;
            else return SaveData.LatestLevelQualified;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Component used to display the name of the level")]
    private TextMeshProUGUI title = null;
    [SerializeField]
    [Tooltip("Component used to display the image of the level")]
    private Image image = null;
    [SerializeField]
    [Tooltip("Image indicating level is locked or not")]
    private GameObject levelLockedOverlay = null;
    [SerializeField]
    [Tooltip("Image indicating level is a spacer")]
    private GameObject levelSpacerOverlay = null;
    [SerializeField]
    [Tooltip("Outline object that appears when the button is hovered over")]
    private GameObject outline = null;

    [SerializeField]
    [Tooltip("Script used to display the rating for this level")]
    private LevelRatingUI ratingUI = null;
    #endregion

    #region Private Fields
    private LevelData enclosure;
    private bool overridden = false;
    private LevelID levelOverride = LevelID.Invalid;
    #endregion

    #region Public Methods
    public void Setup(LevelData enclosure)
    {
        // Set this enclosure to the one given
        this.enclosure = enclosure;

        // Set the image
        image.sprite = enclosure.Level.Sprite;

        if (enclosure.IsSpacer) {
            // Disable the panel, which is currently the parent of the text in the heirarchy
            // TODO: Get a direct reference instead
            title.transform.parent.gameObject.SetActive(false);
            outline.SetActive (false);
            ratingUI.Disable ();
            Destroy (this);
            levelSpacerOverlay.SetActive(true);
            return;
        }

        // Set the title and rating text
        title.text = enclosure.Level.ID.EnclosureNumber.ToString();

        // Disable the outline
        outline.SetActive(false);

        // Setup the rating ui with this enclosure
        ratingUI.Setup(enclosure);
        
        // Show if level is unlocked or not
        levelLockedOverlay.SetActive(!Interactable);
    }
    public void LoadLevel() => LevelDataLoader.LoadLevel(enclosure);
    public void SetOverride(LevelID levelOverride)
    {
        overridden = true;
        this.levelOverride = levelOverride;

        // Disable outline in case this makes the ui not interactable anymore
        levelLockedOverlay.SetActive(false);
        outline.SetActive(false);
    }
    public void ClearOverride()
    {
        overridden = false;

        // Disable outline in case this makes the ui not interactable anymore
        levelLockedOverlay.SetActive(false);
        outline.SetActive(false);
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
            LoadLevel();
        }
    }
    #endregion
}

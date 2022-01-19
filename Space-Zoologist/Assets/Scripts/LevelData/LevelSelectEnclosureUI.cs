using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelSelectEnclosureUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Public Properties
    public bool Interactable => LatestLevelQualified.LevelNumber > enclosure.Level.ID.LevelNumber;
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
    private TextMeshProUGUI title;
    [SerializeField]
    [Tooltip("Component used to display the image of the level")]
    private Image image;
    [SerializeField]
    [Tooltip("Outline object that appears when the button is hovered over")]
    private GameObject outline;
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

        // Set the title and image
        title.text = enclosure.Level.Name;
        image.sprite = enclosure.Level.Sprite;

        // Disable the outline
        outline.SetActive(false);
    }
    public void LoadLevel() => LevelDataLoader.LoadLevel(enclosure);
    public void SetOverride(LevelID levelOverride)
    {
        overridden = true;
        this.levelOverride = levelOverride;

        // Disable outline in case this makes the ui not interactable anymore
        outline.SetActive(false);
    }
    public void ClearOverride()
    {
        overridden = false;

        // Disable outline in case this makes the ui not interactable anymore
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

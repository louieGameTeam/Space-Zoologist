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
    private LevelID CurrentID => LevelID.FromSceneName(enclosure.Level.SceneName);
    private int Rating => SaveData.GetLevelRating(CurrentID);
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

    [SerializeField]
    [Tooltip("Flavor text describing the rating of this level")]
    private TextMeshProUGUI ratingText;
    [SerializeField]
    [Tooltip("Game object prefab to instantiate for each rating level")]
    private GameObject ratingObjectPrefab;
    [SerializeField]
    [Tooltip("Parent to instantiate the rating objects into")]
    private Transform ratingObjectParent;
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

        // Set the title, rating text, and image
        title.text = enclosure.Level.Name;
        image.sprite = enclosure.Level.Sprite;

        // Setup the rating text and rating objects
        ratingText.text = LevelRatingSystem.GetRatingText(Rating);

        // Create a rating object for each rating level
        for (int i = 0; i <= Rating; i++)
        {
            Instantiate(ratingObjectPrefab, ratingObjectParent);
        }

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

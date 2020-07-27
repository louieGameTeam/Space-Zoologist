using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the store menu's logic.
/// </summary>
public class StoreMenu : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] LevelData levelData = default;
    [SerializeField] List<StoreSection> sections = default;
    RectTransform rectTransform = default;

    Dictionary<NeedType, StoreSection> storeSections = new Dictionary<NeedType, StoreSection>();
    TilePlacementController tilePlacementController = default;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        tilePlacementController = FindObjectOfType<TilePlacementController>();
        foreach (StoreSection storeSection in sections)
        {
            storeSections.Add(storeSection.ItemType, storeSection);
            storeSection.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        // Initialize store sections with their items.
        foreach (Item item in levelData.Items)
        {
            storeSections[item.Type].gameObject.SetActive(true);
            storeSections[item.Type].AddItem(item);
        }
    }

    // Disconnect from this and the store sections and Pod menu is too simple so same logic doesn't work here.
    public void OnCursorItemClick(PointerEventData pointerEventData)
    {
        // Detects if the click is within the bounds of the main menu.
        // Ideally there'd be a way to detect when cursor item clicks are done over any menu so that you can't click through menus to place items.
        Debug.Log("On click");
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pointerEventData.position))
        {

        }
    }
}

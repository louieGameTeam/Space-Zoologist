using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoreMenu : MonoBehaviour
{
    [SerializeField] LevelData levelData = default;
    [SerializeField] GameObject storeSectionPrefab = default;
    [SerializeField] CursorItem cursorItem = default;
    [SerializeField] Transform sectionsContainer = default;
    RectTransform rectTransform = default;

    StoreItem selectedStoreItem = null;

    private Dictionary<string, StoreSection> storeSections = new Dictionary<string, StoreSection>();

    public void OnCursorItemClick(PointerEventData pointerEventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pointerEventData.position))
        {
            DeselectItem();
        }
        else
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(pointerEventData.position);
            // Do something here
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        foreach (StoreItem storeItem in levelData.StoreItems)
        {
            if (!storeSections.ContainsKey(storeItem.StoreItemCategory))
            {
                MakeSection(storeItem.StoreItemCategory);
            }
        }
    }

    private void Start()
    {
        foreach (StoreItem storeItem in levelData.StoreItems)
        {
            storeSections[storeItem.StoreItemCategory].AddItem(storeItem);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            DeselectItem();
        }
    }

    private void MakeSection(string sectionCategory)
    {
        GameObject newStoreSectionGO = Instantiate(storeSectionPrefab);
        newStoreSectionGO.transform.SetParent(sectionsContainer, false);
        StoreSection newStoreSection = newStoreSectionGO.GetComponent<StoreSection>();
        newStoreSection.Initialize(sectionCategory, OnSelectItem);
        storeSections.Add(sectionCategory, newStoreSection);
    }

    private void OnSelectItem(StoreItem item)
    {
        selectedStoreItem = item;
        cursorItem.Begin(item.Sprite, OnCursorItemClick);
    }

    private void DeselectItem()
    {
        selectedStoreItem = null;
        if (cursorItem.isActiveAndEnabled)
        {
            cursorItem.Stop(OnCursorItemClick);
        }
    }
}

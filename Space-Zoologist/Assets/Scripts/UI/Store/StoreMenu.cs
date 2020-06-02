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

    private Dictionary<string, StoreSection> storeSections = new Dictionary<string, StoreSection>();

    public void OnSelectItem(StoreItemSO item)
    {
        cursorItem.Begin(item.Sprite, OnCursorItemClick);
    }

    public void OnCursorItemClick(PointerEventData pointerEventData)
    {

    }

    private void Awake()
    {
        foreach (StoreItemSO storeItem in levelData.StoreItems)
        {
            if (!storeSections.ContainsKey(storeItem.StoreItemCategory))
            {
                MakeSection(storeItem.StoreItemCategory);
            }
        }
    }

    private void Start()
    {
        foreach (StoreItemSO storeItem in levelData.StoreItems)
        {
            storeSections[storeItem.StoreItemCategory].AddItem(storeItem);
        }
    }

    private void MakeSection(string sectionCategory)
    {
        GameObject newStoreSectionGO = Instantiate(storeSectionPrefab);
        newStoreSectionGO.transform.SetParent(sectionsContainer, false);
        StoreSection newStoreSection = newStoreSectionGO.GetComponent<StoreSection>();
        newStoreSection.Initialize(sectionCategory, this);
        storeSections.Add(sectionCategory, newStoreSection);
    }
}

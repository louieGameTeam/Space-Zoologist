using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreMenu : MonoBehaviour
{
    [SerializeField] LevelData levelData = default;
    [SerializeField] GameObject storeSectionPrefab = default;

    private Dictionary<string, StoreSection> storeSections = new Dictionary<string, StoreSection>();

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
        GameObject newStoreSectionGO = Instantiate(storeSectionPrefab, transform);
        StoreSection newStoreSection = newStoreSectionGO.GetComponent<StoreSection>();
        newStoreSection.Initialize(sectionCategory);
        storeSections.Add(sectionCategory, newStoreSection);
    }
}

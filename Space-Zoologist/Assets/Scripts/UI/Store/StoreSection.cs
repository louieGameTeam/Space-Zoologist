using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreSection : MonoBehaviour
{
    public string sectionCategory { get; private set; } = "";
    [SerializeField] Text sectionTitle = default;
    [SerializeField] GameObject itemGrid = default;
    [SerializeField] GameObject itemCellPrefab = default;

    public delegate void ItemSelectedHandler(StoreItem item);
    public event ItemSelectedHandler onItemSelected;

    public void Initialize(string sectionCategory, ItemSelectedHandler itemSelectedHandler)
    {
        this.sectionCategory = sectionCategory;
        this.sectionTitle.text = sectionCategory;
        this.onItemSelected += itemSelectedHandler;
    }

    public void AddItem(StoreItem item)
    {
        GameObject newItemCellGO = Instantiate(itemCellPrefab, itemGrid.transform);
        newItemCellGO.GetComponent<StoreItemCell>().Initialize(item, OnItemSelected);
    }

    public void OnItemSelected(StoreItem item)
    {
        onItemSelected.Invoke(item);
    }
}

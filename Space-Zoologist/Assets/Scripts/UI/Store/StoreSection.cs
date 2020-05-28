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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Initialize(string sectionCategory)
    {
        this.sectionCategory = sectionCategory;
        this.sectionTitle.text = sectionCategory;
    }

    public void AddItem(StoreItemSO item)
    {
        GameObject newItemCellGO = Instantiate(itemCellPrefab, itemGrid.transform);
        newItemCellGO.GetComponent<Image>().sprite = item.Sprite;
    }
}

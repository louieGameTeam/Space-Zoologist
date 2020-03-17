using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StoreItemSO : ScriptableObject
{
    [SerializeField] private string itemName = default;
    public string ItemName { get => itemName; set => itemName = value; }
    [SerializeField] private float itemCost = default;
    public float ItemCost { get => itemCost; set => itemCost = value; }
    [SerializeField] private string itemDescription = default;
    public string ItemDescription { get => itemDescription; set => itemDescription = value; }
    [SerializeField] private string itemCategory = default;
    public string ItemCategory { get => itemCategory; set => itemCategory = value; }
    [SerializeField] private Sprite sprite = default;
    public Sprite Sprite { get => sprite; private set => sprite = value; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// class name might be a little confusing, trying to determine if making this a parent class of the species scriptable object makes sense
[CreateAssetMenu]
public class SelectableItemSO : ScriptableObject
{
    [SerializeField] private string itemName = default;
    public string ItemName { get => itemName; set => itemName = value; }
    [SerializeField] private float itemCost = default;
    public float ItemCost { get => itemCost; set => itemCost = value; }
    [SerializeField] private string itemDescription = default;
    public string ItemDescription { get => itemDescription; set => itemDescription = value; }
    [SerializeField] private Color itemCategory = default;
    public Color ItemCategory { get => itemCategory; set => itemCategory = value; }
    [SerializeField] private Sprite sprite = default;
    public Sprite Sprite { get => sprite; set => sprite = value; }
}

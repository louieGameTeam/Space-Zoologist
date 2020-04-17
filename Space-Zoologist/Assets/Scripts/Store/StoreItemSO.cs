using UnityEngine;

// Modify as needed
[CreateAssetMenu]
public class StoreItemSO : ScriptableObject
{
    [SerializeField] private string itemName = default;
    public string ItemName { get => itemName; set => itemName = value; }
    [SerializeField] private float itemCost = default;
    public float ItemCost { get => itemCost; set => itemCost = value; }
    [SerializeField] private string itemDescription = default;
    public string StoreItemDescription { get => itemDescription; set => itemDescription = value; }
    [SerializeField] private Color itemCategory = default;
    public Color StoreItemCategory { get => itemCategory; set => itemCategory = value; }
    [SerializeField] private Sprite sprite = default;
    public Sprite Sprite { get => sprite; set => sprite = value; }
    // Using item identifier to classify items generally (e.g., all terrain tiles identified as "Terrain")
    // TODO: change to enum when enums are formalized
    [SerializeField] private string itemIdentifier;
    public string ItemIdentifier { get => itemIdentifier; set => itemIdentifier = value; }
}

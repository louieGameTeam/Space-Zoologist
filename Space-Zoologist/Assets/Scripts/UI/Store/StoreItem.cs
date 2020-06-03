using UnityEngine;

// Modify as needed
[CreateAssetMenu]
public class StoreItem : ScriptableObject
{
    public string ItemName { get => itemName; set => itemName = value; }
    public float ItemCost { get => itemCost; set => itemCost = value; }
    public string StoreItemDescription { get => itemDescription; set => itemDescription = value; }
    public string StoreItemCategory { get => itemCategory; set => itemCategory = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
    public string ItemIdentifier { get => itemIdentifier; set => itemIdentifier = value; }

    [SerializeField] private string itemName = default;
    [SerializeField] private float itemCost = default;
    [SerializeField] private string itemDescription = default;
    [SerializeField] private string itemCategory = default;
    [SerializeField] private Sprite sprite = default;
    // Identifier for internal comparison between systems (e.g., 'Sand' used to compare to 'Sand' tile type)
    [SerializeField] private string itemIdentifier;
}

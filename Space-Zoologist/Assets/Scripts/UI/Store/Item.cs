using UnityEngine;

// Modify as needed
[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string ID => id;
    public string Type => type;
    public string ItemName => itemName;
    public int Price => price;
    public string Description => description;
    public Sprite Icon => icon;

    [SerializeField] private string id = default;
    [SerializeField] private string type = default;
    [SerializeField] private string itemName = default;
    [SerializeField] private int price = default;
    [SerializeField] private string description = default;
    [SerializeField] private Sprite icon = default;
}

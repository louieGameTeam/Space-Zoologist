using System;
using UnityEngine;

public enum ItemType {Food, Terrain, Machine, Pod}
// Modify as needed
[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string ID => id;
    public ItemType Type => type;
    public string ItemName => itemName;
    public int Price => price;
    public string Description => description;
    public Sprite Icon => icon;
    public int buildTime => BuildTime;

    [SerializeField] private string id = default;
    [SerializeField] private ItemType type = default;
    [SerializeField] private string itemName = default;
    [SerializeField] private int price = default;
    [SerializeField] private string description = default;
    [SerializeField] private Sprite icon = default;
    [SerializeField] private int BuildTime = default;

    public void SetupData(string id, string type, string name, int price)
    {
        this.id = id;
        if (type.Equals("Terrain", StringComparison.OrdinalIgnoreCase))
        {
            this.type = ItemType.Terrain;
        }
        if (type.Equals("Atmosphere", StringComparison.OrdinalIgnoreCase))
        {
            this.type = ItemType.Machine;
        }
        if (type.Equals("Liquid", StringComparison.OrdinalIgnoreCase))
        {
            this.type = ItemType.Machine;
        }
        if (type.Equals("Food", StringComparison.OrdinalIgnoreCase))
        {
            this.type = ItemType.Food;
        }
        this.itemName = name;
        this.price = price;
    }
}

using System;
using UnityEngine;

// Modify as needed
[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string ID => id;
    public NeedType Type => type;
    public string ItemName => itemName;
    public int Price => price;
    public string Description => description;
    public Sprite Icon => icon;

    [SerializeField] private string id = default;
    [SerializeField] private NeedType type = default;
    [SerializeField] private string itemName = default;
    [SerializeField] private int price = default;
    [SerializeField] private string description = default;
    [SerializeField] private Sprite icon = default;

    public void SetupData(string id, string type, string name, int price)
    {
        this.id = id;
        if (type.Equals("Terrain", StringComparison.OrdinalIgnoreCase))
        {
            this.type = NeedType.Terrain;
        }
        if (type.Equals("Atmosphere", StringComparison.OrdinalIgnoreCase))
        {
            this.type = NeedType.Atmosphere;
        }
        if (type.Equals("Density", StringComparison.OrdinalIgnoreCase))
        {
            this.type = NeedType.Density;
        }
        if (type.Equals("Food", StringComparison.OrdinalIgnoreCase))
        {
            this.type = NeedType.Food;
        }
        if (type.Equals("Liquid", StringComparison.OrdinalIgnoreCase))
        {
            this.type = NeedType.Liquid;
        }
        if (type.Equals("Species", StringComparison.OrdinalIgnoreCase))
        {
            this.type = NeedType.Species;
        }
        if (type.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
        {
            this.type = NeedType.Temperature;
        }
        this.itemName = name;
        this.price = price;
    }
}

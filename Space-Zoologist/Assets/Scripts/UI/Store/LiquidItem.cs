using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/LiquidItem")]
public class LiquidItem : Item
{
    public float SaltPercent => saltPercent;
    public float BacteriaPercent => bacteriaPercent;

    [SerializeField] private float saltPercent = default;
    [SerializeField] private float bacteriaPercent = default;

    public new void SetupData(string id, string type, string name, int price)
    {
        base.SetupData(id, type, name, price);
        this.saltPercent = 0;
        this.bacteriaPercent = 2;
    }

    public void SetupData(string id, string type, string name, int price, float saltPercent, float bacteriaPercent)
    {
        base.SetupData(id, type, name, price);
        this.saltPercent = saltPercent;
        this.bacteriaPercent = bacteriaPercent;
    }
}

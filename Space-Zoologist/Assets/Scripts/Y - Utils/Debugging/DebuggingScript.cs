using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingScript : MonoBehaviour
{
    public ItemID unconstrained;
    [ItemIDFilter(ItemRegistry.Category.Species)]
    public ItemID onlyAnimals;
    [ItemIDFilter(ItemRegistry.Category.Food)]
    public ItemID onlyFoods;
    [ItemIDFilter(ItemRegistry.Category.Tile)]
    public ItemID onlyTiles;
}

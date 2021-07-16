using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorModifier : AnimalModifier
{
    [SerializeField] private Color TargetColor = default;
    public override void AddModifier(GameObject animal)
    {
        animal.gameObject.GetComponent<SpriteRenderer>().color = TargetColor;
    }
    public override void RemoveModifier(GameObject animal)
    {
        animal.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedMultiplier : AnimalModifier
{
    [SerializeField] string MultiplierName = default;
    [SerializeField] float Multiplier = default;
    public override void AddModifier(GameObject animal)
    {
        animal.gameObject.GetComponent<Animal>().MovementData.AddMultiplicationSpeedModifiers(this.MultiplierName, this.Multiplier);
    }
    public override void RemoveModifier(GameObject animal)
    {
        animal.gameObject.GetComponent<Animal>().MovementData.RemoveSpeedModifier(this.MultiplierName);
    }
}

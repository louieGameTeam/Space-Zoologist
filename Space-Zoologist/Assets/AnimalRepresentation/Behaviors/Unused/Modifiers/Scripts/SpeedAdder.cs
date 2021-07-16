using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAdder : AnimalModifier
{
    [SerializeField] string AdderName = default;
    [SerializeField] float Adder = default;
    public override void AddModifier(GameObject animal)
    {
        animal.gameObject.GetComponent<Animal>().MovementData.AddAdditionSpeedModifiers(this.AdderName, this.Adder);
    }
    public override void RemoveModifier(GameObject animal)
    {
        animal.gameObject.GetComponent<Animal>().MovementData.RemoveSpeedModifier(this.AdderName);
    }
}

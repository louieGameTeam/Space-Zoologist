using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeModifier : AnimalModifier
{
    // Start is called before the first frame update
    [SerializeField] private Vector3 Multiplier = default;
    public override void AddModifier(GameObject animal)
    {
        animal.gameObject.GetComponent<SpriteRenderer>().transform.localScale = Multiplier;
    }
    public override void RemoveModifier(GameObject animal)
    {
        animal.gameObject.GetComponent<SpriteRenderer>().transform.localScale = Vector3.one;
    }
}

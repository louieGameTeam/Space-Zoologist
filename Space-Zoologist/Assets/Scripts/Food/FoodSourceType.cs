using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FoodSourceType : ScriptableObject
{
    // Getters
    public NeedType Type => type;
    public float BasicOutput => basicOutput;

    // Values
    [Range(0.0f, 100.0f)]
    [SerializeField] private float basicOutput = default;
    [SerializeField] private NeedType type = default;


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO have util handle translation between items
public class ReferenceUtil : MonoBehaviour
{
    public static ReferenceUtil ins;
    [SerializeField] public SpeciesReferenceData SpecisReference = default;
    [SerializeField] public FoodReferenceData FoodReference = default;

    void Awake()
    {
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }
    }
}

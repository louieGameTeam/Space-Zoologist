using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO have util handle translation between items
public class ReferenceUtil : MonoBehaviour
{
    public static ReferenceUtil ins;
    [Expandable] public SpeciesReferenceData SpeciesReference = default;
    [Expandable] public FoodReferenceData FoodReference = default;

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

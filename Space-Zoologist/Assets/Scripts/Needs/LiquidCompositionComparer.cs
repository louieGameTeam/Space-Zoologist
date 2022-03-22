using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidCompositionComparer : IEqualityComparer<float[]>
{
    public bool Equals(float[] a, float[] b)
    {
        return a.SequenceEqual(b);
    }
    public int GetHashCode(float[] a)
    {
        return a[0].GetHashCode() + a[1].GetHashCode() + a[2].GetHashCode();
    }
}
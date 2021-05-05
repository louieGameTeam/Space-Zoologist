using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridItemSet
{
    public string name;
    public float[] coords;
    public GridItemSet(string name, Vector3[] vector3Ints)
    {
        this.name = name;
        this.coords = SerializationUtils.SerializeVector3(vector3Ints);
    }
}

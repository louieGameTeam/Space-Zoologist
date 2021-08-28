using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapItemSet
{
    public string name;
    public float[] coords;
    public MapItemSet(string name, Vector3[] vector3s)
    {
        this.name = name;
        this.coords = SerializationUtils.SerializeVector3(vector3s);
    }
}

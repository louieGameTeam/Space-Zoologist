using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions 
{
    public static Vector3Int ToVector3Int(this Vector3 vec)
    {
        return new Vector3Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y), Mathf.FloorToInt(vec.z));
    }

    public static Vector3 ToVector3(this Vector2 vec)
    {
        return new Vector3(vec.x, vec.y, 0);
    }
}

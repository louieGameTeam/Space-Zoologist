using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializationUtils
{
    /// <summary>
    /// Convert Vector3Int to serializable array
    /// </summary>
    /// <param name="vector3Ints"></param>
    /// <returns></returns>
    public static int[] SerializeVector3Int(Vector3Int[] vector3Ints)
    {
        int[] coords = new int[vector3Ints.Length * 3];
        for (int i = 0; i < vector3Ints.Length; i++)
        {
            coords[i * 3] = vector3Ints[i].x;
            coords[i * 3 + 1] = vector3Ints[i].y;
            coords[i * 3 + 2] = vector3Ints[i].z;
        }
        return coords;
    }
    /// <summary>
    /// Convert serializable array to Vector3Int
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static Vector3Int[] ParseVector3Int(int[] coords)
    {
        if (coords.Length % 3 != 0)
        {
            Debug.LogError("Input is not a array of set of 3s, are you converting a Vector3?");
        }
        Vector3Int[] vector3Ints = new Vector3Int[coords.Length / 3];
        for (int i = 0; i < coords.Length/3; i++)
        {
            vector3Ints[i] = new Vector3Int(coords[i * 3], coords[i * 3 + 1], coords[i * 3 + 2]);
        }
        return vector3Ints;
    }
    /// <summary>
    /// Convert Vector3 to serializable array
    /// </summary>
    /// <param name="vector3s"></param>
    /// <returns></returns>
    public static float[] SerializeVector3(Vector3[] vector3s)
    {
        if (vector3s == null)
        {
            return new float[0];
        }
        float[] coords = new float[vector3s.Length * 3];

        for (int i = 0; i < vector3s.Length; i++)
        {
            Debug.Log(vector3s[i]);
            coords[i * 3] = vector3s[i].x;
            coords[i * 3 + 1] = vector3s[i].y;
            coords[i * 3 + 2] = vector3s[i].z;
        }
        return coords;
    }
    /// <summary>
    /// Convert serializable array to Vector3
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static Vector3[] ParseVector3(float[] coords)
    {
        if (coords.Length % 3 != 0)
        {
            Debug.LogError("Input is not a array of set of 3s, are you converting a Vector3?");
        }
        Vector3[] vector3s = new Vector3[coords.Length / 3];
        for (int i = 0; i < coords.Length / 3; i++)
        {
            vector3s[i] = new Vector3(coords[i * 3], coords[i * 3 + 1], coords[i * 3 + 2]);
        }
        return vector3s;
    }
}

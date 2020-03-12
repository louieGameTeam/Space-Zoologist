using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    /// <summary>
    /// Converts a Vector2Int to a Vector3Int. The resulting vector's 'z' value will be 0.
    /// </summary>
    /// <param name="vector"> The vector to be converted. </param>
    /// <returns></returns>
    public static Vector3Int Vector2IntToVector3Int(Vector2Int vector)
    {
        Vector3Int result = new Vector3Int(vector.x, vector.y, 0);
        return result;
    }
}

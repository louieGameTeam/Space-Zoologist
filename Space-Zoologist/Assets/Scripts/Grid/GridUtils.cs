using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUtils
{
    static public Vector3 AbsVector3(Vector3 vector)
    {
        Vector3 result = new Vector3()
        {
            x = Mathf.Abs(vector.x),
            y = Mathf.Abs(vector.y),
            z = Mathf.Abs(vector.z)
        };
        return result;
    }

    static public Vector3Int SignsVector3(Vector3 vector)
    {
        Vector3 abs = AbsVector3(vector);
        Vector3Int result = new Vector3Int()
        {
            x = abs.x == 0 ? 0 : (int)(vector.x / abs.x),
            y = abs.y == 0 ? 0 : (int)(vector.y / abs.y),
            z = abs.z == 0 ? 0 : (int)(vector.z / abs.z)
        };
        return result;
    }

    static public Vector3Int SignsRoundToIntVector3(Vector3 vector)
    {
        Vector3Int signs = SignsVector3(vector);
        Vector3Int result = new Vector3Int()
        {
            x = signs.x < 0 ? Mathf.FloorToInt(vector.x) : Mathf.CeilToInt(vector.x),
            y = signs.y < 0 ? Mathf.FloorToInt(vector.y) : Mathf.CeilToInt(vector.y),
            z = signs.z < 0 ? Mathf.FloorToInt(vector.z) : Mathf.CeilToInt(vector.z),
        };

        return result;
    }

    /// <summary>
    /// Rounds down or up whichever way is further away from the origin provided.
    /// </summary>
    /// <param name="vector"> The vector to round. </param>
    /// <param name="origin"> The origin that the vector rounds with respect to. </param>
    /// <returns></returns>
    static public Vector3Int SignsRoundToIntVector3(Vector3 vector, Vector3Int origin)
    {
        Vector3Int result = SignsRoundToIntVector3(vector - origin) + origin;

        return result;
    }

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

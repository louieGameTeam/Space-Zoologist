using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUtils
{
    // Used to assign which layer 
    public enum TileLayer
    {
        BaseLayer,
        Terrain,
        LiquidBackground,
        Liquid,
        LiquidSurface,
        LiquidTexture,
        Grass,
        Edge,
        Wall
    }
    private static float[,] defautInterpolationArray = { { 1f, 1f, 1f }, { 1f, 1f, 0f }, { 1f, 0f, 0f }, { 1f, 0.565f, 0f }, { 0.153f, 0.443f, 0.698f }, { 0f, 0.561f, 0.357f }, { 0.439f, 0.212f, 0.588f }, { 0f, 0f, 0f } }; // RYB interpolation cube based on https://ieeexplore.ieee.org/document/5673980
    // public static readonly float[,] interpolationArray = new float[,] { { 1f, 1f, 1f }, { 1f, 1f, 0f }, { 1f, 0f, 0f }, { 1f, 0.5f, 0f }, { 0.163f, 0.373f, 0.6f }, { 0f, 0.66f, 0.2f }, { 0.5f, 0f, 0.5f }, { 0.2f, 0.094f, 0f } }; // Alternative based on https://ieeexplore.ieee.org/document/1382898

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
    /// <summary>
    /// Returns int from start to end (inclusive), default stepping is 1.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static IEnumerable<int> Range(int start, int end, int step = 1)
    {
        if (start < end)
        {
            for (int i = start; i <= end; i += step)
            {
                yield return i;
            }
        }
        else
        {
            for (int i = start; i >= end; i -= step)
                yield return i;
        }
    }
    /// <summary>
    /// Returns FLOAT from start to end (inclusive), default stepping is 1.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static IEnumerable<float> RangeFloat(float start, float end, float step = 1)
    {
        if (start < end)
        {
            for (float i = start; i <= end; i += step)
            {
                yield return i;
            }
        }
        else
        {
            for (float i = start; i >= end; i -= step)
                yield return i;
        }
    }
    /// <summary>
    /// Returns four cell locations next to the given cell location
    /// </summary>
    /// <param name="cellLocation"></param>
    /// <returns></returns>
    public static List<Vector3Int> FourNeighborTiles(Vector3Int cellLocation)
    {
        List<Vector3Int> fourNeighborTiles = new List<Vector3Int>();
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z));
        return fourNeighborTiles;
    }
    /// <summary>
    /// Round a number towards zero
    /// </summary>
    /// <param name="n"> Number to round</param>
    /// <returns></returns>
    public static int RoundTowardsZeroInt (float n)
    {
        if (n > 0)
        {
            return Mathf.FloorToInt(n);
        }
        else
        {
            return Mathf.CeilToInt(n);
        }
    }
    /// <summary>
    /// Round a number towards zero, returns int
    /// </summary>
    /// <param name="n"> Number to round </param>
    /// <returns></returns>
    public static int RoundAwayFromZeroInt(float n)
    {
        if (n < 0)
        {
            return Mathf.FloorToInt(n);
        }
        else
        {
            return Mathf.CeilToInt(n);
        }
    }
    /// <summary>
    /// Increase the magnitude (absolute value) of a number
    /// </summary>
    /// <param name="n"> Number to increase </param>
    /// <param name="increment"> Increment </param>
    /// <returns></returns>
    public static float IncreaseMagnitude(float n, float increment)
    {
        if (n > 0)
        {
            return n += increment;
        }
        else
        {
            return n -= increment;
        }
    }
    public static int IncreaseMagnitudeInt(int n, int increment)
    {
        if (n > 0)
        {
            return n += increment;
        }
        else
        {
            return n -= increment;
        }
    }
    public static bool IsOppositeSign(int m,int n)
    {
        if ((m > 0 && n > 0) || (m < 0 && n < 0))
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// Converts from RYB to RGB color space.
    /// </summary>
    /// <param name="RYBValues">Size must be greater or equal to 3, if greater than 3, the function will only use the first 3 enteries</param>
    /// <param name="interpolationArray">Optional customized trilinear interpolation array</param>
    /// <returns></returns>
    public static Color RYBValuesToRGBColor(float[] RYBValues, float[,] interpolationArray = null) // liquidComposition takes 0 - 1, Converts from RYB to RGB color space
    {
        // Trilinear interpolation method
        if (RYBValues == null)
        {
            return (new Color(1, 1, 1));
        }
        interpolationArray = interpolationArray ?? defautInterpolationArray;
        float[] RGB = { 1, 1, 1 };
        float[] cxy = { 0, 0, 0, 0 };
        for (int i = 0; i < 3; i++) // Trilinear interpolation https://en.wikipedia.org/wiki/Trilinear_interpolation
        {
            for (int j = 0; j < 4; j++)// 1st dimention
            {
                cxy[j] = PerformInterpolation(RYBValues[2], interpolationArray[j, i], interpolationArray[j + 4, i]);
            }
            float c0 = PerformInterpolation(RYBValues[1], cxy[0], cxy[1]); // 2nd dimention
            float c1 = PerformInterpolation(RYBValues[1], cxy[2], cxy[3]);
            RGB[i] = PerformInterpolation(RYBValues[0], c0, c1); // 3rd dimention
        }
        return new Color(RGB[0], RGB[1], RGB[2]);
    }
    private static float PerformInterpolation(float channel, float lowerBound, float upperBound)
    {
        float interpolationResult = lowerBound * (1f - channel) + (channel * upperBound); // Interpolation formula, currently straight line (ax+b)
        return interpolationResult;
    }
}

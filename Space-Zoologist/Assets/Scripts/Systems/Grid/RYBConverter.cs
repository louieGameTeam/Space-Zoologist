using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RYBConverter : MonoBehaviour
{

    public static readonly float[,] interpolationArray = { { 1f, 1f, 1f }, { 1f, 1f, 0f }, { 1f, 0f, 0f }, { 1f, 0.565f, 0f }, { 0.153f, 0.443f, 0.698f }, { 0f, 0.561f, 0.357f }, { 0.439f, 0.212f, 0.588f }, { 0f, 0f, 0f } }; // RYB interpolation cube based on https://ieeexplore.ieee.org/document/5673980
    // public static readonly float[,] interpolationArray = new float[,] { { 1f, 1f, 1f }, { 1f, 1f, 0f }, { 1f, 0f, 0f }, { 1f, 0.5f, 0f }, { 0.163f, 0.373f, 0.6f }, { 0f, 0.66f, 0.2f }, { 0.5f, 0f, 0.5f }, { 0.2f, 0.094f, 0f } }; // Alternative based on https://ieeexplore.ieee.org/document/1382898
    public static Color UpdateLiquidColor(int[] liquidComposition) // liquidComposition takes 0 - 1, Converts from RYB to RGB color space
    {
        // Trilinear interpolation method
        float[] RGB = { 1, 1, 1 };
        float[] cxy = { 0, 0, 0, 0 };
        for (int i = 0; i < 3; i++) // Trilinear interpolation https://en.wikipedia.org/wiki/Trilinear_interpolation
        {
            for (int j = 0; j < 4; j++)// 1st dimention
            {
                cxy[j] = PerformInterpolation(liquidComposition[2], interpolationArray[j, i], interpolationArray[j + 4, i]);
            }
            float c0 = PerformInterpolation(liquidComposition[1], cxy[0], cxy[1]); // 2nd dimention
            float c1 = PerformInterpolation(liquidComposition[1], cxy[2], cxy[3]);
            RGB[i] = PerformInterpolation(liquidComposition[0], c0, c1); // 3rd dimention
        }
        return new Color(RGB[0], RGB[1], RGB[2]);
    }
    private static float PerformInterpolation(float channel, float lowerBound, float upperBound)
    {
        float interpolationResult = lowerBound * (1f - channel) + (channel * upperBound); // Interpolation formula, currently straight line (ax+b)
        return interpolationResult;
    }
}
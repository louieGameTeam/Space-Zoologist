using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// temporary game manager class for storing values
// should be a singleton
public class WorldAtmosphere : MonoBehaviour
{
    [Range(0f, 1f)] [SerializeField] float gasX = 0;
    public float GasX { get => gasX; set => gasX = value; }
    [Range(0f, 1f)] [SerializeField] float gasY = 0;
    public float GasY { get => gasY; set => gasY = value; }
    [Range(0f, 1f)] [SerializeField] float gasZ = 0;
    public float GasZ { get => gasZ; set => gasZ = value; }
    [Range(0f, 100f)] [SerializeField] float temp = 0;
    public float Temp { get => temp; set => temp = value; }
}
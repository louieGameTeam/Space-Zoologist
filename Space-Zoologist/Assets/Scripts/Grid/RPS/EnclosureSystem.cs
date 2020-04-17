using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphericComposition
{
    private float gasX = 0.0f;
    private float gasY = 0.0f;
    private float gasZ = 0.0f;

    public float GasX { get => gasX; }
    public float GasY { get => gasY; }
    public float GasZ { get => gasZ; }

    public AtmosphericComposition(float _gasX, float _gasY, float _gasZ)
    {
        gasX = _gasX;
        gasY = _gasY;
        gasZ = _gasZ;
    }
}

public class EnclosureSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Gets the atmospheric composition at a given position.
    /// </summary>
    /// <param name="position">The position at which to get the atmopheric conditions</param>
    /// <returns></returns>
    public AtmosphericComposition GetAtmosphericComposition(Vector2Int position)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Gets the atmospheric temperature at a given position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float GetTemperature(Vector2Int position)
    {
        throw new System.NotImplementedException();
    }

}

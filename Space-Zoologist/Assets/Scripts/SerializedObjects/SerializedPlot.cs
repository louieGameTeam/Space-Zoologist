using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedPlot
{
    public SerializedMapObjects serializedMapObjects;
    public SerializedGrid serializedGrid;
    // Start is called before the first frame update
    public SerializedPlot (SerializedMapObjects serializedMapObjects, SerializedGrid serializedGrid)
    {
        this.serializedMapObjects = serializedMapObjects;
        this.serializedGrid = serializedGrid;
    }
}

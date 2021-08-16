using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedLiquidBody
{
    public int BodyID;
    public float[] Contents;

    public SerializedLiquidBody(int bodyID, float[] contents)
    {
        this.BodyID = bodyID;
        this.Contents = new float[contents.Length];
        contents.CopyTo(this.Contents, 0);
    }
}

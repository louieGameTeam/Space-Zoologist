using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedLiquidBody
{
    public int BodyID;
    public int[] TilePositions; // Compartmentalized 3 as a set
    public float[] Contents;

    public SerializedLiquidBody(int bodyID, HashSet<Vector3Int> tiles, float[] contents)
    {
        this.BodyID = bodyID;
        this.TilePositions = new int[tiles.Count*3];
        int i = 0;
        foreach (Vector3Int position in tiles)
        {
            int index = i * 3;
            this.TilePositions[index] = position.x;
            this.TilePositions[index + 1] = position.y;
            this.TilePositions[index + 2] = position.z;
            i++;
        }
        this.Contents = new float[contents.Length];
        contents.CopyTo(this.Contents, 0);
    }
}

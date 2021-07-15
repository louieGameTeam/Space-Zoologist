[System.Serializable]
public class SerializedGrid
{
    public SerializedTilemap serializedTilemap;
    public float width;
    public float height;
    // TODO Add other floating objects

    public SerializedGrid(GridSystem gridSystem)
    {
        serializedTilemap = gridSystem.SerializedTilemap();

        UnityEngine.Vector3Int TilemapDimensions = gridSystem.GetReserveDimensions();

        width = TilemapDimensions.x;
        height = TilemapDimensions.y;
    }
}

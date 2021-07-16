[System.Serializable]
public class SerializedGrid
{
    public SerializedTilemap[] serializedTilemaps;
    // TODO Add other floating objects

    public SerializedGrid(TileLayerManager[] tileLayerManagers)
    {
        this.serializedTilemaps = new SerializedTilemap[tileLayerManagers.Length];
        int i = 0;
        foreach(TileLayerManager tileLayerManager in tileLayerManagers)
        {
            this.serializedTilemaps[i] = tileLayerManager.Serialize();
            i++;
        }
    }
}

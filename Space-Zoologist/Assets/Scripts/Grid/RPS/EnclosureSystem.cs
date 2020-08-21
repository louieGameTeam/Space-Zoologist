using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO refactor out temperature
public enum AtmosphereComponent { GasX, GasY, GasZ, Temperature };

/// <summary>
/// A class that represents the atmospheric composition of an area.
/// </summary>
[System.Serializable]
public class AtmosphericComposition
{
    [SerializeField] public float GasX = 0.0f;
    [SerializeField] public float GasY = 0.0f;
    [SerializeField] public float GasZ = 0.0f;
    [SerializeField] private float temperature = 0.0f;

    public float Temperature { get => temperature; }

    public AtmosphericComposition()
    {
        GasX = GasY = GasZ = temperature = 0;
    }

    public AtmosphericComposition(float _GasX, float _GasY, float _GasZ, float _temperature)
    {
        GasX = _GasX;
        GasY = _GasY;
        GasZ = _GasZ;
        temperature = _temperature;
    }

    public AtmosphericComposition(AtmosphericComposition from)
    {
        GasX = from.GasX; GasY = from.GasY; GasZ = from.GasZ; temperature = from.temperature;
    }

    public AtmosphericComposition Copy(AtmosphericComposition from)
    {
        GasX = from.GasX;
        GasY = from.GasY;
        GasZ = from.GasZ;
        temperature = from.temperature;
        return this;
    }

    public static AtmosphericComposition operator +(AtmosphericComposition lhs, AtmosphericComposition rhs)
    {
        return new AtmosphericComposition((lhs.GasX + rhs.GasX) / 2.0f, (lhs.GasY + rhs.GasY) / 2.0f,
            (lhs.GasZ + rhs.GasZ) / 2.0f, (lhs.temperature + rhs.temperature) / 2.0f);
    }

    public override string ToString()
    {
        return "GasX = " + GasX + " GasY = " + GasY + " GasZ = " + GasZ + " Temp = " + temperature;
    }

    /// <summary>
    /// Get the composition of the atmoshpere including temerature, in the order of AtmoshpereComponent enum
    /// </summary>
    /// <returns></returns>
    public float[] GeComposition()
    {
        float[] composition = { GasX, GasY, GasZ, temperature };
        return composition;
    }
}

public class EnclosureSystem : MonoBehaviour
{
    public Dictionary<Vector3Int, byte> PositionToAtmosphere { get; private set; }
    public List<AtmosphericComposition> Atmospheres { get; private set; }

    [SerializeField] private LevelDataReference LevelDataReference = default;
    [SerializeField] private TileSystem TileSystem = default;

    // Have enclosed area been initialized?
    bool initialized = false;

    // The global atmosphere
    private AtmosphericComposition GlobalAtmosphere;

    /// <summary>
    /// Variable initialization on awake.
    /// </summary>
    private void Awake()
    {

        PositionToAtmosphere = new Dictionary<Vector3Int, byte>();
        Atmospheres = new List<AtmosphericComposition>();
    }

    private void Start()
    {
        GlobalAtmosphere = this.LevelDataReference.LevelData.GlobalAtmosphere;
        this.FindEnclosedAreas();
    }

    /// <summary>
    /// Gets the atmospheric composition at a given position.
    /// </summary>
    /// <param name="position">The position at which to get the atmopheric conditions</param>
    /// <returns></returns>
    public AtmosphericComposition GetAtmosphericComposition(Vector3 worldPosition)
    {
        Vector3Int position = this.TileSystem.WorldToCell(worldPosition);
        if (PositionToAtmosphere.ContainsKey(position))
            return Atmospheres[PositionToAtmosphere[position]];
        else
            throw new System.Exception("Unable to find atmosphere at position (" + position.x + " , " + position.y + ")");
    }

    public void UpdateAtmosphereComposition(Vector3 worldPosition, AtmosphericComposition atmosphericComposition)
    {
        Vector3Int position = this.TileSystem.WorldToCell(worldPosition);
        if (PositionToAtmosphere.ContainsKey(position))
        {
            this.GlobalAtmosphere = atmosphericComposition;
            Atmospheres[PositionToAtmosphere[position]] = atmosphericComposition;
            Debug.Log(GetAtmosphericComposition(worldPosition).ToString());
        }
        else
            throw new System.Exception("Unable to find atmosphere at position (" + position.x + " , " + position.y + ")");
    }

    /// <summary>
    /// Private function for determining if a position is within the area
    /// </summary>
    bool WithinRange(Vector3Int pos, int minx, int miny, int maxx, int maxy)
    {
        if (pos.x >= minx && pos.y >= miny && pos.x <= maxx && pos.y <= maxy)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Update the surrounding atmospheres of the position. Only used when adding or removing walls.
    /// </summary>
    /// <param name="positions">Positions where the walls are placed or removed.</param>
    public void UpdateSurroundingAtmosphere(int minx, int miny, int maxx, int maxy)
    {
        // If not initialized or have more than , initialize instead
        if (!initialized || Atmospheres.Count >= 120)
        {
            FindEnclosedAreas();
            return;
        }

        // Step 1: Populate tiles outside with 0 and find walls

        // tiles to-process
        Stack<Vector3Int> stack = new Stack<Vector3Int>();

        // non-wall tiles
        HashSet<Vector3Int> accessed = new HashSet<Vector3Int>();

        // wall or null tiles
        HashSet<Vector3Int> unaccessible = new HashSet<Vector3Int>();

        // walls
        Stack<Vector3Int> walls = new Stack<Vector3Int>();

        // starting location, may be changed later for better performance
        Vector3Int cur = this.TileSystem.WorldToCell(new Vector3(minx, miny, 0));
        TerrainTile curTile = this.TileSystem.GetTerrainTileAtLocation(cur);
        if (curTile != null)
        {
            if (curTile.type == TileType.Wall)
            {
                walls.Push(cur);
            }
            else
            {
                stack.Push(cur);
            }
        }

        byte atmNum = (byte)Atmospheres.Count;
        List<byte> containedAtmosphere = new List<byte>();
        bool newAtmosphere = false;

        // iterate until no tile left in stack
        while (stack.Count > 0)
        {
            // next point
            cur = stack.Pop();

            if (accessed.Contains(cur) || unaccessible.Contains(cur))
            {
                // checked before, move on
                continue;
            }

            // check if tilemap has tile
            TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
            if (tile != null)
            {
                if (tile.type != TileType.Wall)
                {
                    // save the Vector3Int since it is already checked
                    accessed.Add(cur);

                    // ignore global areas outside of range to reduce waste
                    if (!WithinRange(cur, minx, miny, maxx, maxy) && PositionToAtmosphere[cur] == 0)
                    {
                        continue;
                    }

                    // save what used to be here
                    if (!containedAtmosphere.Contains(PositionToAtmosphere[cur]))
                    {
                        containedAtmosphere.Add(PositionToAtmosphere[cur]);
                    }

                    // save the atmosphere here as the current one
                    PositionToAtmosphere[cur] = atmNum;

                    // check all 4 tiles around, may be too expensive/awaiting optimization
                    stack.Push(cur + Vector3Int.left);
                    stack.Push(cur + Vector3Int.up);
                    stack.Push(cur + Vector3Int.right);
                    stack.Push(cur + Vector3Int.down);
                }
                else
                {
                    PositionToAtmosphere[cur] = 255;
                    unaccessible.Add(cur);

                    if (WithinRange(cur, minx, miny, maxx, maxy))
                        walls.Push(cur);
                }
            }
            else
            {
                // save the Vector3Int since it is already checked
                unaccessible.Add(cur);
            }
        }

        if (newAtmosphere)
        {
            Atmospheres.Add(new AtmosphericComposition(Atmospheres[containedAtmosphere[0]]));
            for (int i = 1; i < containedAtmosphere.Count; i++)
            {
                Atmospheres[atmNum] += Atmospheres[containedAtmosphere[i]];
            }
        }

        // Step 2: Loop through walls and push every adjacent tile into the stack
        // and iterate through stack and assign atmosphere number
        atmNum = (byte)Atmospheres.Count;

        // iterate until no tile left in walls
        while (walls.Count > 0)
        {
            // next point
            cur = walls.Pop();

            // check all 4 tiles around, may be too expensive/awaiting optimization
            stack.Push(cur + Vector3Int.left);
            stack.Push(cur + Vector3Int.up);
            stack.Push(cur + Vector3Int.right);
            stack.Push(cur + Vector3Int.down);

            newAtmosphere = false;
            containedAtmosphere = new List<byte>();

            while (stack.Count > 0)
            {
                // next point
                cur = stack.Pop();

                if (accessed.Contains(cur) || unaccessible.Contains(cur))
                {
                    // checked before, move on
                    continue;
                }

                // check if tilemap has tile
                TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
                if (tile != null)
                {
                    if (tile.type != TileType.Wall)
                    {
                        // save the Vector3Int since it is already checked
                        accessed.Add(cur);

                        // ignore global areas outside of range to reduce waste
                        if (!WithinRange(cur, minx, miny, maxx, maxy) && PositionToAtmosphere[cur] == 0)
                        {
                            continue;
                        }

                        // wasn't a wall and the atmosphere wasn't already included
                        if (PositionToAtmosphere[cur] != 255 && !containedAtmosphere.Contains(PositionToAtmosphere[cur]))
                        {
                            containedAtmosphere.Add(PositionToAtmosphere[cur]);
                        }

                        newAtmosphere = true;
                        PositionToAtmosphere[cur] = atmNum;

                        // check all 4 tiles around, may be too expensive/awaiting optimization
                        stack.Push(cur + Vector3Int.left);
                        stack.Push(cur + Vector3Int.up);
                        stack.Push(cur + Vector3Int.right);
                        stack.Push(cur + Vector3Int.down);
                    }
                    else
                    {
                        // walls inside walls
                        PositionToAtmosphere[cur] = 255;
                        unaccessible.Add(cur);
                        if (WithinRange(cur, minx, miny, maxx, maxy))
                            walls.Push(cur);
                    }
                }
                else
                {
                    // save the Vector3Int since it is already checked
                    unaccessible.Add(cur);
                }
            }

            // new atmosphere detected
            if (newAtmosphere)
            {
                atmNum++;
                AtmosphericComposition atmosphere;
                if (containedAtmosphere.Contains(0))
                {
                    // if contains the global atmosphere, no other atmospheres matters
                    atmosphere = Atmospheres[0];
                }
                else if (containedAtmosphere.Count > 0)
                {
                    atmosphere = Atmospheres[containedAtmosphere[0]];
                    for (int i = 1; i < containedAtmosphere.Count; i++)
                    {
                        atmosphere += Atmospheres[containedAtmosphere[i]];
                    }

                }
                else
                {
                    // empty atmosphere if out of nowhere
                    atmosphere = new AtmosphericComposition();
                }
                Atmospheres.Add(atmosphere);
            }
        }
    }


    /// <summary>
    /// Update the System. Find enclosed spaces and populate PositionToAtmosphere, which gives the atmosphere of the tile.
    /// </summary>
    public void FindEnclosedAreas()
    {
        // temporary list of atmosphere
        List<AtmosphericComposition> newAtmospheres = new List<AtmosphericComposition>();
        newAtmospheres.Add(GlobalAtmosphere);


        // Step 1: Populate tiles outside with 0 and find walls

        // tiles to-process
        Stack<Vector3Int> stack = new Stack<Vector3Int>();

        // non-wall tiles
        HashSet<Vector3Int> accessed = new HashSet<Vector3Int>();

        // wall or null tiles
        HashSet<Vector3Int> unaccessible = new HashSet<Vector3Int>();

        // walls
        Stack<Vector3Int> walls = new Stack<Vector3Int>();

        // outer most position
        Vector3Int cur = this.TileSystem.WorldToCell(new Vector3(1, 1, 0));
        stack.Push(cur);

        // iterate until no tile left in stack
        while (stack.Count > 0)
        {
            // next point
            cur = stack.Pop();

            if (accessed.Contains(cur) || unaccessible.Contains(cur))
            {
                // checked before, move on
                continue;
            }

            // check if tilemap has tile
            TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
            if (tile != null)
            {
                if (tile.type != TileType.Wall)
                {
                    // save the Vector3Int since it is already checked
                    accessed.Add(cur);

                    PositionToAtmosphere[cur] = 0;

                    // check all 4 tiles around, may be too expensive/awaiting optimization
                    stack.Push(cur + Vector3Int.left);
                    stack.Push(cur + Vector3Int.up);
                    stack.Push(cur + Vector3Int.right);
                    stack.Push(cur + Vector3Int.down);
                }
                else
                {
                    walls.Push(cur);
                    unaccessible.Add(cur);
                    PositionToAtmosphere[cur] = 255;
                }
            }
            else
            {
                // save the Vector3Int since it is already checked
                unaccessible.Add(cur);
            }
        }


        // Step 2: Loop through walls and push every adjacent tile into the stack
        // and iterate through stack and assign atmosphere number
        byte atmNum = 1;
        // number of tiles in an atmosphere

        // iterate until no tile left in walls
        while (walls.Count > 0)
        {
            // next point
            cur = walls.Pop();

            // check all 4 tiles around, may be too expensive/awaiting optimization
            stack.Push(cur + Vector3Int.left);
            stack.Push(cur + Vector3Int.up);
            stack.Push(cur + Vector3Int.right);
            stack.Push(cur + Vector3Int.down);

            bool newAtmosphere = false;
            List<byte> containedAtmosphere = new List<byte>();

            while (stack.Count > 0)
            {
                // next point
                cur = stack.Pop();

                if (accessed.Contains(cur) || unaccessible.Contains(cur))
                {
                    // checked before, move on
                    continue;
                }

                // check if tilemap has tile
                TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
                if (tile != null)
                {
                    if (tile.type != TileType.Wall)
                    {
                        // save the Vector3Int since it is already checked
                        accessed.Add(cur);

                        if (PositionToAtmosphere.ContainsKey(cur) && PositionToAtmosphere[cur] != 255 && !containedAtmosphere.Contains(PositionToAtmosphere[cur]))
                        {
                            containedAtmosphere.Add(PositionToAtmosphere[cur]);
                        }

                        newAtmosphere = true;
                        PositionToAtmosphere[cur] = atmNum;

                        // check all 4 tiles around, may be too expensive/awaiting optimization
                        stack.Push(cur + Vector3Int.left);
                        stack.Push(cur + Vector3Int.up);
                        stack.Push(cur + Vector3Int.right);
                        stack.Push(cur + Vector3Int.down);
                    }
                    else
                    {
                        // walls inside walls
                        walls.Push(cur);
                        unaccessible.Add(cur);
                        PositionToAtmosphere[cur] = 255;
                    }
                }
                else
                {
                    // save the Vector3Int since it is already checked
                    unaccessible.Add(cur);
                }
            }

            // a new atmosphere was added
            if (newAtmosphere && !initialized)
            {
                atmNum++;
                newAtmospheres.Add(new AtmosphericComposition(Random.value, Random.value, Random.value, Random.value * 100));
            }
            else if (newAtmosphere && initialized)
            {
                atmNum++;
                AtmosphericComposition atmosphere;
                if (containedAtmosphere.Contains(0))
                {
                    // if contains the global atmosphere, no other atmospheres matters
                    atmosphere = Atmospheres[0];
                }
                else if (containedAtmosphere.Count > 0)
                {
                    atmosphere = Atmospheres[containedAtmosphere[0]];
                    for (int i = 1; i < containedAtmosphere.Count; i++)
                    {
                        atmosphere += Atmospheres[containedAtmosphere[i]];
                    }
                }
                else
                {
                    // empty atmosphere if out of nowhere
                    atmosphere = new AtmosphericComposition();
                }
                newAtmospheres.Add(atmosphere);
            }
        }

        Atmospheres = newAtmospheres;
        //print("Number of Atmospheres = " + Atmospheres.Count);
        //print("Detected Number of Atmospheres = " + atmNum);
        initialized = true;
    }
}
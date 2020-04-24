using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphericComposition
{
    private float gasX = 0.0f;
    private float gasY = 0.0f;
    private float gasZ = 0.0f;
    private float temperature = 0.0f;

    public float GasX { get => gasX; }
    public float GasY { get => gasY; }
    public float GasZ { get => gasZ; }
    public float Temperature { get => temperature; }

    public AtmosphericComposition()
    {
        gasX = gasY = gasZ = temperature = 0;
    }

    public AtmosphericComposition(float _gasX, float _gasY, float _gasZ, float temperature)
    {
        gasX = _gasX;
        gasY = _gasY;
        gasZ = _gasZ;
    }

    public AtmosphericComposition(AtmosphericComposition from)
    {
        gasX = from.gasX; gasY = from.gasY; gasZ = from.gasZ; temperature = from.temperature;
    }

    public AtmosphericComposition Copy(AtmosphericComposition from)
    {
        gasX = from.gasX;
        gasY = from.gasY;
        gasZ = from.gasZ;
        temperature = from.temperature;
        return this;
    }

    public static AtmosphericComposition operator +(AtmosphericComposition lhs, AtmosphericComposition rhs)
    {
        return new AtmosphericComposition((lhs.gasX + rhs.gasX)/2.0f, (lhs.gasY + rhs.gasY) / 2.0f,
            (lhs.gasZ + rhs.gasZ) / 2.0f, (lhs.temperature + rhs.temperature) / 2.0f);
    }

    public override string ToString() {
        return "gasX = " + gasX + " gasY = " + gasY + " gasZ = " + gasZ + " Temp = " + temperature;
    }
}

public class EnclosureSystem : MonoBehaviour
{
    public Dictionary<Vector3Int, byte> PositionToAtmosphere { get; private set; }
    public List<AtmosphericComposition> Atmospheres { get; private set; }

    TileSystem _tileSystem; //GetTerrainTile API from Virgil

    // Have enclosed area been initialized?
    bool EnclosureInit = false;

    //singleton
    public static EnclosureSystem ins;


    public void Awake()
    {
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }

        PositionToAtmosphere = new Dictionary<Vector3Int, byte>();
        Atmospheres = new List<AtmosphericComposition>();
        Atmospheres.Add(new AtmosphericComposition());
        _tileSystem = FindObjectOfType<TileSystem>();
    }

    public void Start()
    {
        InvokeRepeating("UpdateAtmosphere", 1.0f, 1.0f);
    }

    private void UpdateAtmosphere() {
        FindEnclosedAreas();
        GetComponent<AtmosphereTester>().Graph();
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


    /// <summary>
    /// Update the System. Find enclosed spaces and populate PositionToAtmosphere, which gives the atmosphere of the tile.
    /// </summary>
    public void FindEnclosedAreas()
    {
        if (PositionToAtmosphere == null)
        {
            //initialize
            PositionToAtmosphere = new Dictionary<Vector3Int, byte>();
        }

        //temporary list of atmosphere
        List<AtmosphericComposition> newAtmospheres = new List<AtmosphericComposition>();
        newAtmospheres.Add(Atmospheres[0]);

        //Step 1: Populate tiles outside with 0 and find walls

        //tiles to-process
        Stack<Vector3Int> stack = new Stack<Vector3Int>();

        //non-wall tiles
        List<Vector3Int> accessible = new List<Vector3Int>();

        //wall or null tiles
        List<Vector3Int> unaccessible = new List<Vector3Int>();

        //walls
        Stack<Vector3Int> walls = new Stack<Vector3Int>();

        //starting location, may be changed later for better performance
        int p = -100;
        while (p < 100)
        {
            if (_tileSystem.GetTerrainTileAtLocation(_tileSystem.WorldToCell(new Vector3(p, 0, 0))) != null)
            {
                break;
            }
            else
            {
                p++;
            }
        }

        //outer most position
        Vector3Int cur = _tileSystem.WorldToCell(new Vector3(p, 0, 0));
        stack.Push(cur);

        //iterate until no tile left in stack
        while (stack.Count > 0)
        {
            //next point
            cur = stack.Pop();

            if (accessible.Contains(cur) || unaccessible.Contains(cur) || walls.Contains(cur))
            {
                //checked before, move on
                continue;
            }

            //check if tilemap has tile
            TerrainTile tile = _tileSystem.GetTerrainTileAtLocation(cur);
            if (tile != null)
            {
                if (tile.type != TileType.Wall)
                {
                    //save the Vector3Int since it is already checked
                    accessible.Add(cur);

                    if (PositionToAtmosphere.ContainsKey(cur))
                    {
                        PositionToAtmosphere[cur] = 0;
                    }
                    else
                    {
                        //the position uses the normal atmosphere
                        PositionToAtmosphere.Add(cur, 0);
                    }

                    //check all 4 tiles around, may be too expensive/awaiting optimization
                    stack.Push(cur + Vector3Int.left);
                    stack.Push(cur + Vector3Int.up);
                    stack.Push(cur + Vector3Int.right);
                    stack.Push(cur + Vector3Int.down);
                }
                else
                {
                    walls.Push(cur);
                }
            }
            else
            {
                //save the Vector3Int since it is already checked
                unaccessible.Add(cur);
            }
        }


        //Step 2: Loop through walls and push every adjacent tile into the stack
        //and iterate through stack and assign atmosphere number
        byte atmNum = 1;

        //iterate until no tile left in walls
        while (walls.Count > 0)
        {
            //next point
            cur = walls.Pop();

            //the position uses the normal atmosphere
            if (!PositionToAtmosphere.ContainsKey(cur))
                PositionToAtmosphere.Add(cur, 255);
            else
                PositionToAtmosphere[cur] = 255;

            //check all 4 tiles around, may be too expensive/awaiting optimization
            stack.Push(cur + Vector3Int.left);
            stack.Push(cur + Vector3Int.up);
            stack.Push(cur + Vector3Int.right);
            stack.Push(cur + Vector3Int.down);

            //save the Vector3Int since it is already checked
            unaccessible.Add(cur);

            bool newAtmosphere = false;
            List<byte> containedAtmosphere = new List<byte>();

            while (stack.Count > 0)
            {
                //next point
                cur = stack.Pop();

                if (accessible.Contains(cur) || unaccessible.Contains(cur) || walls.Contains(cur))
                {
                    //checked before, move on
                    continue;
                }

                //check if tilemap has tile
                TerrainTile tile = _tileSystem.GetTerrainTileAtLocation(cur);
                if (tile != null)
                {
                    if (tile.type != TileType.Wall)
                    {
                        //save the Vector3Int since it is already checked
                        accessible.Add(cur);

                        if (!EnclosureInit)
                        {
                            //the position uses a different atmosphere
                            newAtmosphere = true;
                            PositionToAtmosphere.Add(cur, atmNum);
                        }
                        else //already initialized
                        {
                            if (PositionToAtmosphere[cur] != 255 && !containedAtmosphere.Contains(PositionToAtmosphere[cur]))
                            {
                                containedAtmosphere.Add(PositionToAtmosphere[cur]);
                            }

                            if (!newAtmosphere)
                            {
                                newAtmosphere = true;
                            }
                            PositionToAtmosphere[cur] = atmNum;
                        }
                        //check all 4 tiles around, may be too expensive/awaiting optimization
                        stack.Push(cur + Vector3Int.left);
                        stack.Push(cur + Vector3Int.up);
                        stack.Push(cur + Vector3Int.right);
                        stack.Push(cur + Vector3Int.down);
                    }
                    else
                    {
                        //walls inside walls
                        walls.Push(cur);
                    }
                }
                else
                {
                    //save the Vector3Int since it is already checked
                    unaccessible.Add(cur);
                }
            }

            //a new atmosphere was added
            if (newAtmosphere && !EnclosureInit)
            {
                atmNum++;
                newAtmospheres.Add(new AtmosphericComposition(Random.value, Random.value, Random.value, Random.value*100));
            }
            else if (newAtmosphere && EnclosureInit) {
                atmNum++;
                if (containedAtmosphere.Count > 0)
                {
                    AtmosphericComposition atmosphere = Atmospheres[containedAtmosphere[0]];
                    //if contains the outside atmosphere, no other atmospheres matters
                    if (containedAtmosphere[0] != 0)
                    {
                        for (int i = 1; i < containedAtmosphere.Count; i++)
                        {
                            if (containedAtmosphere[i] == 0)
                            {
                                //if contains the outside atmosphere, no other atmospheres matters
                                atmosphere = Atmospheres[0];
                                break;
                            }
                            else if (containedAtmosphere[i] != 255)
                            {
                                atmosphere += Atmospheres[containedAtmosphere[i]];
                            }
                        }
                    }
                    newAtmospheres.Add(atmosphere);
                }
                else {
                    //empty atmosphere if out of nowhere
                    newAtmospheres.Add(new AtmosphericComposition());
                }
            }
        }

        Atmospheres = newAtmospheres;
        print("Number of Atmospheres = " + Atmospheres.Count);
        print("Detected Number of Atmospheres = " + atmNum);
        EnclosureInit = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A manager for managing how the reserve is "separated" for each population.
/// </summary>
public class ReservePartitionManager : MonoBehaviour
{
    //singleton
    public static ReservePartitionManager ins;

    //may change
    public const int maxPop = 64;

    public List<Population> Pops { get; private set; }
    private Queue<int> openID;
    public Dictionary<Population, int> PopToID { get; private set; }
    private Dictionary <Vector3Int, long> accessMap;
    public Dictionary<Vector3Int, byte> PositionToAtmosphere { get; private set; }
    public List<Atmosphere> Atmospheres { get; private set; }
    public FoodSource food; //TODO to be removed, only for Demo purposes

    GetTerrainTile GTT; //GetTerrainTile API from Virgil
    Tilemap reference; //a reference tilemap for converting cell position

    public bool RPMDemo = false; //if in demo

    int lastRecycledID;

    public void Awake() {
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else {
            ins = this;
        }

        //long mask is limited to 64 bits
        openID = new Queue<int>();
        lastRecycledID = maxPop - 1; //63
        for (int i = maxPop-1; i >= 0 ; i--) {
            openID.Enqueue(i);
        }
        Pops = new List<Population>();
        PopToID = new Dictionary<Population, int>();
        accessMap = new Dictionary<Vector3Int, long>();
        PositionToAtmosphere = new Dictionary<Vector3Int, byte>();
        Atmospheres = new List<Atmosphere>();
        GTT = FindObjectOfType<GetTerrainTile>();
        reference = GTT.GetComponent<TilePlacementController>().tilemapList[0];

        Atmospheres.Add(new Atmosphere());//global atmosphere
    }

    public void Start()
    {
        FindEnclosedAreas();
        GetComponent<AtmosphereTester>().Graph();
    }

    public void Update()
    {
        if (RPMDemo) //if in demo
        {
            foreach (Population pop in Pops)
                if (CanAccess(pop, food.transform.position))
                {
                    pop.transform.Translate((food.transform.position - pop.transform.position) * 0.01f);
                }
        }
    }

    ///<summary>
    ///Add a population to the RPM.
    ///</summary>
    public void AddPopulation(Population pop) {
        if (!Pops.Contains(pop)){
            //ignore their old id and assign it a new one
            int id = openID.Dequeue();
            //since IDs after 63 are recycled, we need to do clean up old values
            if (id == lastRecycledID) CleanupAccessMapForRecycledID();
            PopToID.Add(pop, id);
            Pops.Add(pop);

            //generate the map with the new id  
            GenerateMap(pop);
            print(pop.name + " " + CanAccess(pop, new Vector3(0, 0, 0)));
            if(PopDensityManager.ins != null) PopDensityManager.ins.AddPop(pop);
        }
    }

    ///<summary>
    ///Remove a population from the RPM.
    ///</summary>
    public void RemovePopulation(Population pop)
    {
        if (PopDensityManager.ins != null) PopDensityManager.ins.RemovePop(pop);
        Pops.Remove(pop);
        openID.Enqueue(PopToID[pop]);
        PopToID.Remove(pop); //free ID
    }

    ///<summary>
    ///Called internally when ID is recycled.
    ///</summary>
    void CleanupAccessMapForRecycledID() {
        foreach (int id in openID)
        {
            foreach (Vector3Int loc in accessMap.Keys) {
                //set the values to 0 through bit masking
                accessMap[loc] &= ~(1L << id);
            }
            lastRecycledID = id;
        }
        if (PopDensityManager.ins != null) PopDensityManager.ins.CleanupDensityMap(openID.ToArray());
    }
    ///<summary>
    ///Populate the access map for a population with depth first search.
    ///</summary>
    private void GenerateMap(Population pop) {
        if (!Pops.Contains(pop)) {
            AddPopulation(pop);
        }
        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        List<Vector3Int> accessible = new List<Vector3Int>();
        List<Vector3Int> unaccessible = new List<Vector3Int>();
        Vector3Int cur;

        //starting location
        Vector3Int location = reference.WorldToCell(pop.transform.position);
        stack.Push(location);

        //iterate until no tile left in list, ends in iteration 1 if pop.location is not accessible
        while (stack.Count > 0) {
            //next point
            cur = stack.Pop();

            if (accessible.Contains(cur) || unaccessible.Contains(cur)) {
                //checked before, move on
                continue;
            }

            //check if tilemap has tile and if pop can access the tile (e.g. some cannot move through water)
            TerrainTile tile = GTT.GetTerrainTileAtLocation(cur);
            if (tile != null && pop.Species.accessibleTerrain.Contains(tile.type))
            {
                if (pop.name == "Dirt") { print(pop.name + " accesses " + tile.name + " of type " +  tile.type); }
                //save the Vector3Int since it is already checked
                accessible.Add(cur);

                //check all 4 tiles around, may be too expensive/awaiting optimization
                stack.Push(cur + Vector3Int.left);
                stack.Push(cur + Vector3Int.up);
                stack.Push(cur + Vector3Int.right);
                stack.Push(cur + Vector3Int.down);
            }
            else {
                //save the Vector3Int since it is already checked
                unaccessible.Add(cur);
            }
        }

        foreach (Vector3Int pos in accessible) {
            if (!accessMap.ContainsKey(pos)) {
                accessMap.Add(pos, 0L);
            }
            //set the pop.getID()th bit in accessMap[pos] to 1
            accessMap[pos] |= 1L << PopToID[pop];
        }
    }

    ///<summary>
    ///Update the access map for every population in Pops.
    ///</summary>
    public void UpdateAccessMap()
    {
        accessMap = new Dictionary<Vector3Int, long>();
        foreach (Population pop in Pops)
        {
            GenerateMap(pop);
        }
    }

    ///<summary>
    ///Check if a population can access toWorldPos.
    ///</summary>
    public bool CanAccess(Population pop, Vector3 toWorldPos)
    {
        //convert to map position
        Vector3Int mapPos = reference.WorldToCell(toWorldPos);

        return CanAccess(pop, mapPos);
    }

    ///<summary>
    ///Check if a population can access CellPos.
    ///</summary>
    public bool CanAccess(Population pop, Vector3Int CellPos)
    {
        //convert to map position
        Vector3Int mapPos = CellPos;

        //if accessible
        //check if the nth bit is set (i.e. accessible for the pop)
        if (accessMap.ContainsKey(mapPos))
        {
            if (((accessMap[mapPos] >> PopToID[pop]) & 1L) == 1L)
            {
                return true;
            }
        }

        //pop can't access the position
        return false;
    }

    ///<summary>
    ///Go through Pops and return a list of populations that has access to the tile corresponding to toWorldPos.
    ///</summary>
    public List<Population> GetPopulationsWithAccessTo(Vector3 toWorldPos)
    {
        List<Population> accessible = new List<Population>();
        foreach (Population pop in Pops)
        {
            //utilize CanAccess()
            if (CanAccess(pop, toWorldPos))
            {
                accessible.Add(pop);
            }
        }
        return accessible;
    }

    /// <summary>
    /// Converts a world position to cell position using the reference tilemap.
    /// </summary>
    public Vector3Int WorldToCell(Vector3 worldPos) {
        return reference.WorldToCell(worldPos);
    }

    public Atmosphere GetAtmosphere(Vector3 worldPos) {
        Vector3Int cellPos = WorldToCell(worldPos);
        if (PositionToAtmosphere.ContainsKey(cellPos)) {
            return Atmospheres[PositionToAtmosphere[cellPos]];
        }
        return Atmospheres[0];
    }

    //Find enclosed spaces and populate PositionToAtmosphere, which gives the atmosphere of the tile
    public void FindEnclosedAreas() {
        //initialize
        PositionToAtmosphere = new Dictionary<Vector3Int, byte>();


        //Step 1: Populate tiles outside with 0 and find walls

        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        List<Vector3Int> accessible = new List<Vector3Int>();
        List<Vector3Int> unaccessible = new List<Vector3Int>();
        Stack<Vector3Int> walls = new Stack<Vector3Int>();

        //starting location, may be changed later for better performance
        Vector3Int cur = WorldToCell(new Vector3(-9, 0, 0));
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
            TerrainTile tile = GTT.GetTerrainTileAtLocation(cur);
            if (tile != null)
            {
                if (tile.type != TileType.Wall)
                {
                    //save the Vector3Int since it is already checked
                    accessible.Add(cur);

                    //the position uses the normal atmosphere
                    PositionToAtmosphere.Add(cur, 0);

                    //check all 4 tiles around, may be too expensive/awaiting optimization
                    stack.Push(cur + Vector3Int.left);
                    stack.Push(cur + Vector3Int.up);
                    stack.Push(cur + Vector3Int.right);
                    stack.Push(cur + Vector3Int.down);
                }
                else {
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
        bool newAtm;

        //iterate until no tile left in walls
        while (walls.Count > 0)
        {
            //next point
            cur = walls.Pop();

            //the position uses the normal atmosphere
            PositionToAtmosphere.Add(cur, 255);

            //check all 4 tiles around, may be too expensive/awaiting optimization
            stack.Push(cur + Vector3Int.left);
            stack.Push(cur + Vector3Int.up);
            stack.Push(cur + Vector3Int.right);
            stack.Push(cur + Vector3Int.down);

            //save the Vector3Int since it is already checked
            unaccessible.Add(cur);

            newAtm = false;

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
                TerrainTile tile = GTT.GetTerrainTileAtLocation(cur);
                if (tile != null)
                {
                    if (tile.type != TileType.Wall)
                    {
                        //save the Vector3Int since it is already checked
                        accessible.Add(cur);

                        //the position uses a different atmosphere
                        newAtm = true;
                        PositionToAtmosphere.Add(cur, atmNum);

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
            if (newAtm)
                atmNum++;
        }
        //copy the outside atmosphere
        while (Atmospheres.Count < atmNum) {
            Atmospheres.Add(new Atmosphere(Atmospheres[0]));
        }
        print("Number of Atmospheres = " + atmNum);
    }
}
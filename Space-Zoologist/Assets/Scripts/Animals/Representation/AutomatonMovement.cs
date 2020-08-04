using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animals will attempt to move NumSteps based off of the seeded directions given.
/// The seeded directions determine what order directions should be tried.
/// </summary>
public class AutomatonMovement : MonoBehaviour
{
    [SerializeField] int NumTiles = 3;
    [SerializeField] int MaxNumTiles = 6;
    private MovementController movementController = default;
    private int numTilesMoved = 0;

    private Population population = default;
    private List<int> DirectionSeed = default;
    private int CurrentDirectionSeedIndex = 0;
    private List<int> TilesToMoveSeed = default;
    private int TilesMovedIndex = 0;
    private Direction CurrentDirection = Direction.down;
    private AnimalPathfinding.Node previousTile = default;
    private List<Vector3> PathToFollow = default;

    public void Awake()
    {
        this.DirectionSeed = GenerateDirectionSeed();
        this.TilesToMoveSeed = GenerateTilesToMoveSeed(MaxNumTiles);
    }

    public void Start()
    {
        this.movementController = this.gameObject.GetComponent<MovementController>();
    }

    public void Initialize(Population population)
    {
        this.population = population;
    }

    // If animal walked predetermined number of tiles or animal cannot move in specified direction, update based off seed.
    public void Update()
    {
        if (this.population == null || this.movementController.IsPaused)
        {
            return;
        }
        if (!this.movementController.DestinationReached)
        {
            this.movementController.MoveTowardsDestination();
            return;
        }
        if (this.numTilesMoved >= this.NumTiles || !this.DirectionAllowed((Direction)this.DirectionSeed[this.CurrentDirectionSeedIndex], this.transform.position, this.population.grid))
        {
            if (!this.TryToUpdateDirection())
            {
                AnimalPathfinding.PathRequestManager.RequestPath(TilemapUtil.ins.WorldToCell(this.transform.position), this.population.AccessibleLocations[0], this.SetupPathfinding, this.population.grid);
                return;
            }
            this.UpdateTilesToMove();
        }
        this.UpdateNumTilesMoved();
        this.movementController.MoveInDirection(this.CurrentDirection);
    }

    private void SetupPathfinding(List<Vector3> path, bool pathFound)
    {
        // TODO figure out what to do if you can't pathfind.. Keep trying to pathfind? Notify player?
        Debug.Log("Animal stuck, setting up pathfinding");
        Debug.Assert(pathFound == true);
        this.movementController.AssignPath(path);
        this.PathToFollow = path;
    }

    private void UpdateNumTilesMoved()
    {
        AnimalPathfinding.Node currentTile = population.grid.GetNode(TilemapUtil.ins.WorldToCell(this.transform.position).x, TilemapUtil.ins.WorldToCell(this.transform.position).y);
        if (this.previousTile != currentTile)
        {
            this.previousTile = currentTile;
            this.numTilesMoved++;
        }
    }

    // Keep track of CurrentSeedIndex, iterate through seed (looping back around)
    // until a direction is found or no viable direction can be found
    private bool TryToUpdateDirection()
    {
        int count = 0;
        CurrentDirectionSeedIndex = IncrementSeedIndex(CurrentDirectionSeedIndex);
        while (!this.DirectionAllowed((Direction)this.DirectionSeed[this.CurrentDirectionSeedIndex], this.transform.position, this.population.grid))
        {
            CurrentDirectionSeedIndex = IncrementSeedIndex(CurrentDirectionSeedIndex);
            count++;
            if (count > 8)
            {
                break;
            }
        }
        // nowhere for the animal to move
        if (count > 8)
        {
            this.CurrentDirection = Direction.down;
            return false;
        }
        else
        {
            this.CurrentDirection = (Direction)DirectionSeed[CurrentDirectionSeedIndex];
            return true;
        }
    }

    private int IncrementSeedIndex(int currentDirectionSeedIndex)
    {
        currentDirectionSeedIndex++;
        if (currentDirectionSeedIndex == 8)
        {
            currentDirectionSeedIndex = 0;
        }
        return currentDirectionSeedIndex;
    }

    private void UpdateTilesToMove()
    {
        this.TilesMovedIndex++;
        if (this.TilesMovedIndex >= this.TilesToMoveSeed.Count)
        {
            this.TilesMovedIndex = 0;
        }
        this.NumTiles = this.TilesToMoveSeed[this.TilesMovedIndex];
        this.numTilesMoved = 0;
    }

    private List<int> GenerateDirectionSeed()
    {
        HashSet<int> seed = new HashSet<int>();
        System.Random random = new System.Random();
        while (seed.Count < 8)
        {
            int r = random.Next(0, 8);
            if (!seed.Contains(r))
            {
                seed.Add(r);
            }
        }
        return seed.ToList();
    }

    private List<int> GenerateTilesToMoveSeed(int count)
    {
        HashSet<int> seed = new HashSet<int>();
        System.Random random = new System.Random();
        while (seed.Count < count)
        {
            int r = random.Next(1, count + 1);
            if (!seed.Contains(r))
            {
                seed.Add(r);
            }
        }
        return seed.ToList();
    }

    // Using direction, starting location, and grid, determine if the next spot on the grid is accessible
    public bool DirectionAllowed(Direction direction, Vector3 startingLocation, AnimalPathfinding.Grid grid)
    {
        bool isAllowed = false;
        AnimalPathfinding.Node currentSpot = population.grid.GetNode(TilemapUtil.ins.WorldToCell(this.transform.position).x, TilemapUtil.ins.WorldToCell(this.transform.position).y);
        if (currentSpot == null)
        {
            // Debug.Log("Node outside of range");
            return false;
        }
        switch(direction)
        {
            case Direction.up:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX, currentSpot.gridY + 1);
                break;
            }
            case Direction.down:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX, currentSpot.gridY - 1);
                break;
            }
            case Direction.left:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX - 1, currentSpot.gridY);
                break;
            }
            case Direction.right:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX + 1, currentSpot.gridY);
                break;
            }
            case Direction.upRight:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX + 1, currentSpot.gridY + 1);
                break;
            }
            case Direction.upLeft:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX - 1, currentSpot.gridY + 1);
                break;
            }
            case Direction.downRight:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX + 1, currentSpot.gridY - 1);
                break;
            }
            case Direction.downLeft:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX - 1, currentSpot.gridY - 1);
                break;
            }
        }
        return isAllowed;
    }
}

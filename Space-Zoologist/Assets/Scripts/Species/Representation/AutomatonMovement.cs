using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

// TODO figure out how this can be refactored for behaviors
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

    public void Start()
    {
        this.movementController = this.gameObject.GetComponent<MovementController>();
    }

    public void Initialize(Population population)
    {
        this.population = population;
        this.DirectionSeed = GenerateDirectionSeed();
        this.TilesToMoveSeed = GenerateTilesToMoveSeed(MaxNumTiles);
        this.CheckSurroundings();
    }

    // If animal walked predetermined number of tiles or animal cannot move in specified direction, update based off seed.
    public void Update()
    {
        if (this.population == null || this.movementController.IsPaused)
        {
            return;
        }
        if (this.numTilesMoved >= this.NumTiles)
        {
            this.UpdateTilesToMove();
        }
        this.CheckSurroundings();
        this.movementController.MoveInDirection(this.CurrentDirection);
    }

    // If on new tile then respond to new surroundings
    private void CheckSurroundings()
    {
        AnimalPathfinding.Node currentTile = population.Grid.GetNode(this.transform.position);
        if (currentTile != this.previousTile)
        {
            this.previousTile = currentTile;
            this.numTilesMoved++;
            this.RespondToNewSurroundings(currentTile);
        }
    }

    // TODO setup pathfinding back to an accessiblearea for when the animal gets stuck or kill them...
    private void RespondToNewSurroundings(AnimalPathfinding.Node newTile)
    {
        if (!this.DirectionAllowed(this.CurrentDirection, newTile, this.population.Grid))
        {
            if (!this.TryToUpdateDirection(newTile))
            {
                this.movementController.IsPaused = true;
                //AnimalPathfinding.PathRequestManager.RequestPath(TilemapUtil.ins.WorldToCell(this.transform.position), this.population.AccessibleLocations[0], this.SetupPathfinding, this.population.grid);
            }
        }
    }

    // Using direction, starting location, and grid, determine if the next spot on the grid is accessible
    public bool DirectionAllowed(Direction direction, AnimalPathfinding.Node currentSpot, AnimalPathfinding.Grid grid)
    {
        bool isAllowed = false;
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

    // Keep track of CurrentSeedIndex, iterate through seed (looping back around)
    // until a direction is found or no viable direction can be found
    private bool TryToUpdateDirection(AnimalPathfinding.Node currentTile)
    {
        int count = 0;
        while (!this.DirectionAllowed((Direction)this.DirectionSeed[this.CurrentDirectionSeedIndex], currentTile, this.population.Grid))
        {
            CurrentDirectionSeedIndex = IncrementSeedIndex(CurrentDirectionSeedIndex);
            count++;
            if (count > 8)
            {
                this.CurrentDirection = Direction.down;
                return false;
            }
        }
        this.CurrentDirection = (Direction)DirectionSeed[CurrentDirectionSeedIndex];
        return true;
    }

    private int IncrementSeedIndex(int currentDirectionSeedIndex)
    {
        currentDirectionSeedIndex++;
        if (currentDirectionSeedIndex >= 8)
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

    private void SetupPathfinding(List<Vector3> path, bool pathFound)
    {
        // TODO figure out what to do if you can't pathfind.. Keep trying to pathfind? Notify player?
        if (!pathFound)
        {
            Debug.Log("Issue with pathfinding");
            return;
        }
        Debug.Log("Animal stuck, setting up pathfinding");
        this.movementController.AssignPath(path, pathFound);
    }

    private List<int> GenerateDirectionSeed()
    {
        HashSet<int> seed = new HashSet<int>();
        while (seed.Count < 8)
        {
            int r = this.population.random.Next(0, 8);
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
        while (seed.Count < count)
        {
            int r = this.population.random.Next(1, count + 1);
            if (!seed.Contains(r))
            {
                seed.Add(r);
            }
        }
        return seed.ToList();
    }
}

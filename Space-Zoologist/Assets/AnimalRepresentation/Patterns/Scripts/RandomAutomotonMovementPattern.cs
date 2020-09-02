using System.Linq;

using System.Collections.Generic;
using UnityEngine;

public class RandomAutomotonMovementPattern : BehaviorPattern
{
    [SerializeField] int NumTiles = 3;
    [SerializeField] int MaxNumTiles = 6;
    private List<int> DirectionSeed = default;
    private int CurrentDirectionSeedIndex = 0;
    private List<int> TilesToMoveSeed = default;
    private int TilesMovedIndex = 0;
    private bool Initialized = false;
    private Population population = default;
    private MovementController movementController = default;
    private AnimalPathfinding.Node previousTile = default;
    private Direction CurrentDirection = Direction.down;
    private int numTilesMoved = 0;
    private int totalTileMoved = 0;

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        this.movementController = animalData.animal.MovementController;
        this.population = animalData.animal.PopulationInfo;
        if (!this.Initialized)
        {
            this.Initialized = true;
            this.DirectionSeed = GenerateDirectionSeed();
            this.TilesToMoveSeed = GenerateTilesToMoveSeed(this.MaxNumTiles);
            this.CheckSurroundings();
        }
    }
    // Default behavior moves along a random path
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (this.population == null || this.movementController.IsPaused || this.totalTileMoved > 30)
        {
            return true;
        }
        if (this.numTilesMoved >= this.NumTiles)
        {
            this.UpdateTilesToMove();
        }
        this.CheckSurroundings();
        this.movementController.MoveInDirection(this.CurrentDirection);
        return false;
    }

    // If on new tile then respond to new surroundings
    private void CheckSurroundings()
    {
        AnimalPathfinding.Node currentTile = this.population.Grid.GetNode(this.transform.position);
        if (currentTile != this.previousTile)
        {
            this.previousTile = currentTile;
            this.numTilesMoved++;
            this.totalTileMoved++;
            this.RespondToNewSurroundings(currentTile);
        }
    }

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

    private int IncrementSeedIndex(int currentDirectionSeedIndex)
    {
        currentDirectionSeedIndex++;
        if (currentDirectionSeedIndex >= 8)
        {
            currentDirectionSeedIndex = 0;
        }
        return currentDirectionSeedIndex;
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

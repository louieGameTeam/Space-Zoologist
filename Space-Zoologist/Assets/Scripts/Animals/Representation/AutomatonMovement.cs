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
    private int numTilesWalked = 0;

    private Population population = default;
    private List<int> DirectionSeed = default;
    private int CurrentDirectionSeedIndex = 0;
    private List<int> TilesToMoveSeed = default;
    private int TilesMovedIndex = 0;
    private Direction CurrentDirection = default;
    private AnimalPathfinding.Node previousTile = default;

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
        if (this.population == null)
        {
            return;
        }
        if (this.numTilesWalked >= this.NumTiles || !TilemapUtil.ins.DirectionAllowed((Direction)this.DirectionSeed[this.CurrentDirectionSeedIndex], this.transform.position, this.population.grid))
        {
            this.UpdateDirection();
            this.UpdateTilesToMove();
        }
        this.UpdateNumTilesWalked();
        this.movementController.MoveInDirection(this.CurrentDirection);
    }


    private void UpdateNumTilesWalked()
    {
        AnimalPathfinding.Node currentTile = TilemapUtil.ins.CellToGrid(TilemapUtil.ins.WorldToCell(this.transform.position), population.grid);
        if (this.previousTile != currentTile)
        {
            this.previousTile = currentTile;
            this.numTilesWalked++;
        }
    }

    // Keep track of CurrentSeedIndex, iterate through seed (looping back around)
    // until a direction is found
    private void UpdateDirection()
    {
        int count = 0;
        CurrentDirectionSeedIndex++;
        if (CurrentDirectionSeedIndex == 8)
        {
            CurrentDirectionSeedIndex = 0;
        }
        while (!TilemapUtil.ins.DirectionAllowed((Direction)this.DirectionSeed[this.CurrentDirectionSeedIndex], this.transform.position, this.population.grid))
        {
            CurrentDirectionSeedIndex++;
            if (CurrentDirectionSeedIndex == 8)
            {
                CurrentDirectionSeedIndex = 0;
            }
            count++;
            if (count > 8)
            {
                Debug.Log("Animal unable to move");
                break;
            }
        }
        this.CurrentDirection = (Direction)DirectionSeed[CurrentDirectionSeedIndex];
    }

    private void UpdateTilesToMove()
    {
        this.TilesMovedIndex++;
        if (this.TilesMovedIndex >= this.TilesToMoveSeed.Count)
        {
            this.TilesMovedIndex = 0;
        }
        this.NumTiles = this.TilesToMoveSeed[this.TilesMovedIndex];
        this.numTilesWalked = 0;
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
}

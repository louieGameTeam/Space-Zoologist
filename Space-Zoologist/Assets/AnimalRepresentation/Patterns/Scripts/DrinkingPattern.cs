using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkingPattern : UniversalAnimatorPattern
{
    [SerializeField] private string Up;
    [SerializeField] private string Down;
    [SerializeField] private string Left;
    [SerializeField] private string Right;
    [SerializeField] private TerrainTile liquidTile;
    private TileSystem tileSystem;
    public override void StartUp()
    {
        tileSystem = FindObjectOfType<TileSystem>();
        base.StartUp();
    }
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        Vector3Int currentCell = tileSystem.WorldToCell(animal.transform.position);
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (currentCell[0] + j < 0 || currentCell[1] + i < 0)
                {
                    continue;
                }
                if (tileSystem.GetTerrainTileAtLocation(new Vector3Int(currentCell[0] + j, currentCell[1] + i, 0)) == liquidTile)
                {
                    //if (GridSystem.CellGrid[currentCell[0] + j, currentCell[1] + i].Food.SpeciesName.Equals(foodName))
                    //{
                        this.AnimatorTriggerName = GetTriggerName(i, j);
                        base.EnterPattern(animal, animalData);
                        return;
                    //}
                }
            }
        }
        this.AnimatorTriggerName = this.Up;
        base.EnterPattern(animal, animalData);
    }
    private string GetTriggerName(int i, int j)
    {
        if (i == 1)
        {
            return this.Up;
        }
        if (i == -1)
        {
            return this.Down;
        }
        if (j == 1)
        {
            return this.Right;
        }
        if (j == -1)
        {
            return this.Left;
        }
        return this.Up;
    }
}

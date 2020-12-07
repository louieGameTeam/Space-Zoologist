using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkingPattern : UniversalAnimatorPattern
{
    [SerializeField] private string Up = default;
    [SerializeField] private string Down = default;
    [SerializeField] private string Left = default;
    [SerializeField] private string Right = default;
    [SerializeField] private GameTile liquidTile = default;
    private TileSystem tileSystem = default;
    public override void StartUp()
    {
        tileSystem = FindObjectOfType<TileSystem>();
        base.StartUp();
    }
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        Vector3Int currentCell = tileSystem.WorldToCell(animal.transform.position);
        tileSystem.GetTerrainTileAtLocation(currentCell).targetTilemap.SetColor(currentCell,Color.red);
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (Mathf.Abs(y) + Mathf.Abs(x) > 1)
                {
                    continue;
                }
                if (tileSystem.GetGameTileAt(new Vector3Int(currentCell[0] + x, currentCell[1] + y, 0)) == liquidTile)
                {
                    //if (GridSystem.CellGrid[currentCell[0] + x, currentCell[1] + i].Food.SpeciesName.Equals(foodName))
                    //{
                        this.AnimatorTriggerName = GetTriggerName(x, y);
                    if (AnimatorTriggerName == this.Up)
                    {
                        print((currentCell[0]+x)+","+ (currentCell[1] +y)+":" + tileSystem.GetTerrainTileAtLocation(new Vector3Int(currentCell[0] + x, currentCell[1] + y, 0)).name);
                    }
                        base.EnterPattern(animal, animalData);
                        return;
                    //}
                }
            }
        }
        Debug.LogError("no liquid2");
        this.AnimatorTriggerName = this.Down;
        base.EnterPattern(animal, animalData);
    }
    private string GetTriggerName(int x, int y)
    {
        if (y == -1)
        {
            return this.Down;
        }
        if (x == 1)
        {
            return this.Right;
        }
        if (x == -1)
        {
            return this.Left;
        }
        if (y == 1)
        {
            return this.Up;
        }
        Debug.LogError("no liquid");
        return this.Down;
    }
}

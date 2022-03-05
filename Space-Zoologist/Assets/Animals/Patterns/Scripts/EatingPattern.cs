using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingPattern : UniversalAnimatorPattern
{
    [SerializeField] private string Up = default;
    [SerializeField] private string Down = default;
    [SerializeField] private string Left = default;
    [SerializeField] private string Right = default;
    [SerializeField] private string foodName = default;
    public override void StartUp()
    {
        base.StartUp();
    }
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        print("Trying to eat");
        Vector3Int currentCell = base.TileDataController.WorldToCell(animal.transform.position);
        (int, int)[] checks = { (1, 0), (-1, 0), (0, 1), (0, -1) };
        foreach (var position in checks)
        {
            int i = position.Item1, j = position.Item2;
            if (currentCell[0] + j < 0 || currentCell[1] + i < 0)
            {
                continue;
            }
            Vector3Int loopedTile = new Vector3Int(currentCell[0] + j, currentCell[1] + i, 0);
            if (TileDataController.IsCellinGrid(currentCell[0] + j, currentCell[1] + i) && TileDataController.GetTileData(loopedTile).Food)
            {
                if (TileDataController.GetTileData(loopedTile).Food.GetComponent<FoodSource>().Species.SpeciesName == foodName)
                {
                    this.AnimatorTriggerName = GetTriggerName(i, j);
                    base.EnterPattern(animal, animalData);
                    SetAnimDirectionFloat(animal, i, j);
                    return;
                }
            }
        }
        // No edible food
        // this.AnimatorTriggerName = this.Up;
        base.EnterPattern(animal, animalData);
        base.ExitPattern(animal, true);
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

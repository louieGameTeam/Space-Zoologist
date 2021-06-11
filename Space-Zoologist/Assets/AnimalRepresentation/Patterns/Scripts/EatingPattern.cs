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
    private TileSystem tileSystem = default;
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
                if (GridSystem.isCellinGrid(currentCell[0] + j, currentCell[1] + i) && GridSystem.CellGrid[currentCell[0] + j, currentCell[1] + i].ContainsFood)
                {
                    if (GridSystem.CellGrid[currentCell[0] + j, currentCell[1] + i].Food.GetComponent<FoodSource>().Species.SpeciesName == foodName)
                    {
                        this.AnimatorTriggerName = GetTriggerName(i, j);
                        base.EnterPattern(animal, animalData);
                        return;
                    }
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

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkingPattern : UniversalAnimatorPattern
{
    [SerializeField] private string Up = default;
    [SerializeField] private string Down = default;
    [SerializeField] private string Left = default;
    [SerializeField] private string Right = default;
    public override void StartUp()
    {
        base.StartUp();
    }
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        Vector3Int currentCell = base.GridSystem.WorldToCell(animal.transform.position);
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector3Int loopedCell = new Vector3Int(currentCell[0] + j, currentCell[1] + i, 0); 
                if (GridSystem.IsCellinGrid(currentCell[0] + j, currentCell[1] + i) && GridSystem.GetTileData(loopedCell).currentLiquidBody != null)
                {
                    this.AnimatorTriggerName = GetTriggerName(i, j);
                    base.EnterPattern(animal, animalData);
                    return;
                }
            }
        }
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
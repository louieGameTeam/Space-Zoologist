using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingPattern : UniversalAnimatorPattern
{
    [SerializeField] private string Up = default;
    [SerializeField] private string Down = default;
    [SerializeField] private string Left = default;
    [SerializeField] private string Right = default;

    private ItemID foodID = default;
    public ItemID FoodID => foodID;

    public override void Init()
    {
        base.Init();
    }

    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        Vector3Int currentCell = base.TileDataController.WorldToCell(animal.transform.position);

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (currentCell[0] + j < 0 || currentCell[1] + i < 0)
                {
                    continue;
                }
                Vector3Int loopedTile = new Vector3Int(currentCell[0] + j, currentCell[1] + i, 0);

                if (TileDataController.IsCellinGrid(currentCell[0] + j, currentCell[1] + i) && TileDataController.GetTileData(loopedTile).Food)
                {
                    FoodSource foodSource = TileDataController.GetTileData(loopedTile).Food.GetComponent<FoodSource>();
                    foodID = foodSource.Species.ID;

                    // Added to address bug with animals eating incorrect food if foods were placed too closely
                    if (foodID.Data.Name.Get(ItemName.Type.Serialized) != animal.GetComponent<Animal>().FoodTargetSpeciesName)
                    {
                        continue;
                    }

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
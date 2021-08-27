using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBalance : MonoBehaviour
{
    private LevelDataReference levelDataReference = default;
    public float Balance { get; private set; }

    private void Awake()
    {
        levelDataReference = FindObjectOfType<LevelDataReference>();
        this.Balance = this.levelDataReference.LevelData.StartingBalance;
    }

    public void SubtractFromBalance(float value)
    {
        if (this.Balance - value >= 0)
        {
            this.Balance -= value;
        }
    }

    public void SetBalance(float value)
    {
        this.Balance = value;
    }
}

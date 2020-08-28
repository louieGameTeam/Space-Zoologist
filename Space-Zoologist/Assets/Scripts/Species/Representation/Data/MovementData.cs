using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Modify in the editor or via code to change an animal's behavior.
/// </summary>
/// TODO add more unique fields as more behaviors are created. Consider creating specific data scripts if this becomes too unorganized.
public enum Movement { idle = 0, walking = 1, running = 2, eating = 3 }
public enum Direction { up = 0, down = 1, left = 2, right = 3, upRight = 4, upLeft = 5, downRight = 6, downLeft = 7 }
[System.Serializable]
public class MovementData
{
    public Movement MovementStatus = Movement.idle;
    public Direction CurrentDirection = Direction.down;

    [SerializeField] public float Speed  = 1f;
    [SerializeField] public float RunThreshold  = 2f;
    [SerializeField] public float IdleTimeBetweenBehaviors = 0f;
}


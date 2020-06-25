using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Each animal has it's own BehaviorsData object that is Serialized by the editor and can be modified by accessing the animal object.
/// </summary>
public enum Movement { idle = 0, walking = 1, running = 2, eating = 3 }
public enum Direction { up = 0, down = 1, left = 2, right = 3 }
[System.Serializable]
public class BehaviorsData
{
    public Movement MovementStatus = Movement.idle;
    public Direction CurrentDirection = Direction.down;

    [SerializeField] public float Speed  = 1f;
    [SerializeField] public float RunThreshold  = 2f;
    [SerializeField] public float IdleTimeBetweenBehaviors = 0f;
    
    [Header("Eating Data")]
    [SerializeField] public float EatingLength = 3f;
    [SerializeField] public List<GameObject> FoodSourceLocations = default;
    [SerializeField] public GameObject CurrentFoodSourceLocation = default;
}


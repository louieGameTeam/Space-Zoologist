using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO 1 figure out how to inject GridSystem dependency for WorldToCell and
public class TimedPattern : BehaviorPattern
{
    protected Dictionary<GameObject,float> AnimalToTimeElapsed = new Dictionary<GameObject, float>();
    [SerializeField] protected float ExitTime = 0f;

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        this.AnimalToTimeElapsed.Add(gameObject, 0);
    }
    // Default behavior moves along a random path
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (this.AnimalToTimeElapsed[animal] > ExitTime)
        {
            return true;
        }
        this.AnimalToTimeElapsed[animal] += Time.deltaTime;
        return false;
    }
    protected override void ExitPattern(GameObject gameObject, bool callCallback)
    {
        this.AnimalToTimeElapsed.Remove(gameObject);
        base.ExitPattern(gameObject, callCallback);
    }
}

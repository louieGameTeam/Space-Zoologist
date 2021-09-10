using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepPattern : BehaviorPattern
{
    private Dictionary<GameObject,float> AnimalToTimeElapsed = new Dictionary<GameObject, float>();
    [SerializeField]private float ExitTime = 0f;
    [SerializeField] private Sprite SleepOverlay = default;

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        this.AnimalToTimeElapsed.Add(gameObject, 0);
        animalData.animal.Overlay.sprite = this.SleepOverlay;
        animalData.animal.Overlay.enabled = true;
        animalData.animal.MovementData.MovementStatus = Movement.idle;
        // Debug.Log("Overlay ENABLED");
    }
    // Default behavior moves along a random path
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (this.AnimalToTimeElapsed[animal] > ExitTime)
        {
            animalData.animal.Overlay.enabled = false;
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

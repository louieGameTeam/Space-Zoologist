using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO 1 figure out how to inject GridSystem dependency for WorldToCell and
public class CrazyPattern : BehaviorPattern
{
    private float TimeElapsed = 0f;
    private float Spinner = 1f;

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        this.TimeElapsed = 0f;
    }
    // Default behavior moves along a random path
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        // TODO 2
        if (this.TimeElapsed > 5f)
        {
            var rotationVectorr = animalData.animal.gameObject.transform.rotation.eulerAngles;
            rotationVectorr.z = 0f;
            return true;
        }
        var rotationVector = animalData.animal.gameObject.transform.rotation.eulerAngles;
        rotationVector.z += this.Spinner;
        animalData.animal.gameObject.transform.rotation = Quaternion.Euler(rotationVector);
        this.TimeElapsed += Time.deltaTime;
        return false;
    }
}

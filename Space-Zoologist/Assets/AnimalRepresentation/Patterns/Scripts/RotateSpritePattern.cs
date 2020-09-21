using System.Collections.Generic;
using UnityEngine;

public class RotateSpritePattern : TimedPattern
{
    [SerializeField] Vector3 rotationAngles;
    [SerializeField] Vector3 angularSpeeds;
    Dictionary<GameObject, Quaternion> animalsToOriginalRotationAngles = new Dictionary<GameObject, Quaternion>();
    Dictionary<GameObject, Vector3> animalsToCurrentAngles = new Dictionary<GameObject, Vector3>();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        animalsToOriginalRotationAngles.Add(gameObject, gameObject.transform.rotation);
        animalsToCurrentAngles.Add(gameObject, gameObject.transform.rotation.eulerAngles);
        base.EnterPattern(gameObject, animalData);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        Vector3 newRotation = animalsToCurrentAngles[animal];
        for (int i = 0; i < 3; i++)
        {
            if (!BehaviorUtils.IsTargetValueReached(animalsToOriginalRotationAngles[animal].eulerAngles[i], rotationAngles[i], animalsToCurrentAngles[animal][i]))
            {
                newRotation[i] += angularSpeeds[i] * Time.deltaTime;
                
            }
        }
        animal.transform.rotation = Quaternion.Euler(newRotation);
        animalsToCurrentAngles[animal] = newRotation;
        return base.IsPatternFinishedAfterUpdate(animal, animalData);
    }
    protected override void ExitPattern(GameObject gameObject, bool callCallback)
    {
        gameObject.transform.rotation = animalsToOriginalRotationAngles[gameObject];
        animalsToOriginalRotationAngles.Remove(gameObject);
        animalsToCurrentAngles.Remove(gameObject);
        base.ExitPattern(gameObject, callCallback);
    }
}

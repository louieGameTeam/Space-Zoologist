using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChange : BehaviorPattern
{
    [SerializeField] Vector3 targetScale;
    protected Dictionary<GameObject, Vector3> animalsToOriginalScales = new Dictionary<GameObject, Vector3>();
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        animalsToOriginalScales.Add(animal, animal.transform.localScale);
        animal.transform.localScale = targetScale;
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        // does not end until interrupted
        return false;
    }
    protected override void ExitPattern(GameObject animal, bool callCallback = true)
    {
        animal.transform.localScale = animalsToOriginalScales[animal];
        animalsToOriginalScales.Remove(animal);
        base.ExitPattern(animal, callCallback);
    }
}

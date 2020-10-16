using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChangeGradual : TimedPattern
{
    [SerializeField] Vector3 changeRates;
    [SerializeField] Vector3 targetScale;
    protected Dictionary<GameObject, Vector3> animalsToOriginalScales = new Dictionary<GameObject, Vector3>();
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        animalsToOriginalScales.Add(animal, animal.transform.localScale);
        animal.transform.localScale = targetScale;
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        Vector3 currentScale = animal.transform.localScale;
        for (int i = 0; i < 4; i++)
        {
            if (animal.transform.localScale != targetScale)
            {
                currentScale[i] = movesTowards(currentScale[i], changeRates[i] * Time.deltaTime, targetScale[i]);
            }
        }
        animal.transform.localScale = currentScale;
        return base.IsPatternFinishedAfterUpdate(animal, animalData);
    }
    protected override void ExitPattern(GameObject animal, bool callCallback = true)
    {
        animal.transform.localScale = animalsToOriginalScales[animal];
        animalsToOriginalScales.Remove(animal);
        base.ExitPattern(animal, callCallback);
    }
    private float movesTowards(float cur, float change, float target) {
        float end = cur + change;
        if (end > target && change > 0)
        {
            return target;
        }
        else if (end < target && change < 0)
        {
            return target;
        }
        else {
            return end;
        }
    }
}

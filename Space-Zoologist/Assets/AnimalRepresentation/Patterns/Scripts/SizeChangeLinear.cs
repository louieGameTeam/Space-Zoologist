using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChangeLinear : TimedPattern
{
    [SerializeField] Vector3 changeRates;
    [SerializeField] Vector3 targetScale;
    protected Dictionary<GameObject, Vector3> animalsToOriginalScales = new Dictionary<GameObject, Vector3>();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        if (!animalsToOriginalScales.ContainsKey(gameObject))
            animalsToOriginalScales.Add(gameObject, gameObject.transform.localScale);
        base.EnterPattern(gameObject, animalData);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        Vector3 currentScale = animal.transform.localScale;
        for (int i = 0; i < 3; i++)
        {
            if (animal.transform.localScale != targetScale)
            {
                currentScale[i] = movesTowards(currentScale[i], changeRates[i] * Time.deltaTime, targetScale[i]);
            }
        }
        animal.transform.localScale = currentScale;
        return base.IsPatternFinishedAfterUpdate(animal, animalData);
    }
    protected override void ExitPattern(GameObject gameObject, bool callCallback)
    {
        gameObject.transform.localScale = animalsToOriginalScales[gameObject];
        animalsToOriginalScales.Remove(gameObject);
        base.ExitPattern(gameObject, callCallback);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearColorChangePattern : TimedPattern
{
    [SerializeField] private Vector4 changeRates;
    [SerializeField] protected Color color;
    protected Dictionary<GameObject, Color> animalsToOriginalColors = new Dictionary<GameObject, Color>();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        animalsToOriginalColors.Add(gameObject, gameObject.GetComponent<SpriteRenderer>().color);
        base.EnterPattern(gameObject, animalData);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        Color currentColor = animal.GetComponent<SpriteRenderer>().color;
        for (int i = 0; i < 4; i++)
        {
            if (!BehaviorUtils.IsTargetValueReached(animalsToOriginalColors[animal][i], color[i], currentColor[i]))
            {
                currentColor[i] += changeRates[i] * Time.deltaTime;
                currentColor[i] = this.Clamp(currentColor[i]);
            }
        }
        animal.GetComponent<SpriteRenderer>().color = currentColor;
        return base.IsPatternFinishedAfterUpdate(animal, animalData);
    }
    protected override void ExitPattern(GameObject gameObject, bool callCallback)
    {
        gameObject.GetComponent<SpriteRenderer>().color = animalsToOriginalColors[gameObject];
        animalsToOriginalColors.Remove(gameObject);
        base.ExitPattern(gameObject, callCallback);
    }
    private float Clamp(float f)
    {
        if (f > 1f)
        {
            return 1f;
        }
        if (f < 0f)
        {
            return 0f;
        }
        return f;
    }
}

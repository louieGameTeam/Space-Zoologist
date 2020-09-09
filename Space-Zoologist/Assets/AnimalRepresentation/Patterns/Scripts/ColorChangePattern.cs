using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangePattern : TimedPattern
{
    [SerializeField] protected Color color;
    protected Dictionary<GameObject, Color> animalsToOriginalColors = new Dictionary<GameObject, Color>();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        animalsToOriginalColors.Add(gameObject, gameObject.GetComponent<SpriteRenderer>().color);
        gameObject.GetComponent<SpriteRenderer>().color = color;
        base.EnterPattern(gameObject, animalData);
    }
    protected override void ExitPattern(GameObject gameObject)
    {
        gameObject.GetComponent<SpriteRenderer>().color = animalsToOriginalColors[gameObject];
        animalsToOriginalColors.Remove(gameObject);
        base.ExitPattern(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEmoji : TimedPattern
{
    [SerializeField] Sprite Emoji = default;
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        base.EnterPattern(gameObject, animalData);
        gameObject.transform.GetChild(0).GetComponent<Animator>().enabled = false;
        animalData.animal.Overlay.sprite = Emoji;
    }

    protected override void ExitPattern(GameObject gameObject, bool callCallback = true)
    {
        gameObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
        AnimalsToAnimalData[gameObject].animal.Overlay.sprite = null;
        //AnimalsToAnimalData[gameObject].animal.Overlay.enabled = false;
        base.ExitPattern(gameObject, callCallback);
    }

    protected override void ForceExit(GameObject gameObject)
    {
        // Debug.Log("Overlay DISABLED " + gameObject.GetComponent<AnimalBehaviorManager>().activeBehaviorPatterns[0].gameObject.name);
        gameObject.GetComponent<Animal>().Overlay.enabled = false;
        base.ForceExit(gameObject);
    }
}

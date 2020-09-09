using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadButtingAnime : UniversalAnimatorPattern
{
    [SerializeField] private string triggerNameL = default;
    [SerializeField] private string triggerNameR = default;
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        if (animal.transform.position.x < animalData.collaboratingAnimals[0].transform.position.x)
        {
            AnimatorTriggerName = triggerNameL;
        }
        else
        {
            AnimatorTriggerName = triggerNameR;
        }
        base.EnterPattern(animal, animalData);
    }
}

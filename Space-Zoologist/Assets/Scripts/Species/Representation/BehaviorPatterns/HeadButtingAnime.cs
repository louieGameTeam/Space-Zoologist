using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadButtingAnime : UniversalAnimatorPattern
{
    [SerializeField] private string triggerNameL;
    [SerializeField] private string triggerNameR;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HeadButtingBehavior", menuName = "SpeciesBehavior/HeadButtingBehavior")]
public class HeadButtingBehavior : SpecieBehaviorTrigger
{
    protected override List<GameObject> AnimalSelection(Dictionary<Availability, List<GameObject>> avalabilityToAnimals)
    {
        return BehaviorUtils.SelectAnimals(2, avalabilityToAnimals);
    }
    protected override void ProceedToNext(GameObject animal, List<GameObject> collaboratingAnimals, bool isDriven = false)
    {
        base.ProceedWhenAllAnimalsReady(animal, collaboratingAnimals, isDriven);
    }
}

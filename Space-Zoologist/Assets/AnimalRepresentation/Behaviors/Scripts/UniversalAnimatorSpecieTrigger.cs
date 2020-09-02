using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SleepBehavior", menuName = "SpeciesBehavior/UniversalAnimatorSpecieTrigger")]
public class UniversalAnimatorSpecieTrigger : SpecieBehaviorTrigger
{
    protected override List<GameObject> AnimalSelection(Dictionary<Availability, List<GameObject>> avalabilityToAnimals)
    {
        return BehaviorUtils.SelectAnimals(1, avalabilityToAnimals, SelectionType.All);
    }
}

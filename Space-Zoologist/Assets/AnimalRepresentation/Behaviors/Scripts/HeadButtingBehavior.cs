using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HeadButtingBehavior", menuName = "SpeciesBehavior/HeadButtingBehavior")]
public class HeadButtingBehavior : SynchronizedBehaviorTrigger
{
    protected override List<GameObject> AnimalSelection(Dictionary<Availability, List<GameObject>> avalabilityToAnimals)
    {
        return BehaviorUtils.SelectAnimals(this.numberTriggerdPerLoop, avalabilityToAnimals, SelectionType.FreeAndConcurrent);
    }
}

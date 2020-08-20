using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleRandom", menuName = "SpeciesBehaviorTrigger/IdleRandom")]
public class IdleRandomTrigger : SpecieBehaviorTrigger
{

    protected override List<GameObject> AnimalSelection(Dictionary<Availability, List<GameObject>> avalabilityToAnimals)
    {
        // Debug.Log("Animal Selected");
        return BehaviorUtils.SelectAnimals(1, avalabilityToAnimals, SelectionType.All);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrazyTrigger", menuName = "SpeciesBehaviorTrigger/CrazyTrigger")]
public class CrazyTrigger : SpecieBehaviorTrigger
{
    [SerializeField]
    private float refreshPeriod = 3;
    [SerializeField]
    private float elapsedTime = 0;

    protected override List<GameObject> AnimalSelection(Dictionary<Availability, List<GameObject>> avalabilityToAnimals)
    {
        // Debug.Log("Animal Selected");
        return BehaviorUtils.SelectAnimals(1, avalabilityToAnimals, SelectionType.All);
    }
}

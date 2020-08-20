using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleRandom", menuName = "SpeciesBehaviorTrigger/IdleRandom", order = 1)]
public class IdleRandomTrigger : SpecieBehaviorTrigger
{
    [SerializeField]
    private float refreshPeriod = 3;
    [SerializeField]
    private float elapsedTime = 0;
    public override bool IsConditionSatisfied()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > refreshPeriod)
        {
            return true;
        }
        return false;
    }
    public override void ResetCondition()
    {
        elapsedTime = 0;
    }
    protected override List<GameObject> AnimalSelection(Dictionary<Availability, List<GameObject>> avalabilityToAnimals)
    {
        Debug.Log("Animal Selected");
        return BehaviorUtils.SelectAnimals(1, avalabilityToAnimals, SelectionType.All);
    }
}

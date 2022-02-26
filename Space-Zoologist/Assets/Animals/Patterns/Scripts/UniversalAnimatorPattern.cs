using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalAnimatorPattern : BehaviorPattern
{
    public string AnimatorTriggerName;
    protected Dictionary<GameObject, AnimatorData> animalToAnimatorData = new Dictionary<GameObject, AnimatorData>();
    protected Dictionary<GameObject, float> animalToTimeData = new Dictionary<GameObject, float>();
    [SerializeField] protected bool OverlayAnimation;

    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        animalToAnimatorData.Add(animal, new AnimatorData());
        if (OverlayAnimation)
        {
            animalToAnimatorData[animal].animator = animal.transform.GetChild(0).GetComponent<Animator>();
        }
        else
        {
            animalToAnimatorData[animal].animator = animal.GetComponent<Animator>();
        }
        if (animalToAnimatorData[animal].animator.layerCount != 1)
        {
            if (animalToAnimatorData[animal].animator.GetLayerIndex(AnimatorTriggerName) != -1)
            {
                animalToAnimatorData[animal].layerIndex = animalToAnimatorData[animal].animator.GetLayerIndex(AnimatorTriggerName);
            }
        }
        animalToAnimatorData[animal].initialState = animalToAnimatorData[animal].animator.GetCurrentAnimatorStateInfo(animalToAnimatorData[animal].layerIndex);
        animalToAnimatorData[animal].animator.SetBool("IsStateFinished", false);
        animalToAnimatorData[animal].animator.SetTrigger(AnimatorTriggerName);
        animalToAnimatorData[animal].animator.Update(0);
        //Universal animator pattern exits after the animation plays once
        animalToTimeData.Add(animal, animalToAnimatorData[animal].animator.GetCurrentAnimatorStateInfo(0).length);
    }

    protected void SetAnimDirectionFloat(GameObject animal, int i, int j)
    {
        if (i == 1)
        {
            animalToAnimatorData[animal].animator.SetInteger("Direction", 0);
        }
        else if (i == -1)
        {
            animalToAnimatorData[animal].animator.SetInteger("Direction", 1);
        }
        else if (j == 1)
        {
            animalToAnimatorData[animal].animator.SetInteger("Direction", 3);
        }
        else if (j == -1)
        {
            animalToAnimatorData[animal].animator.SetInteger("Direction", 2);
        }
    }

    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        //Animation clip duration timing
        animalToTimeData[animal] -= Time.deltaTime;
        if (animalToAnimatorData[animal].animator.GetBool("IsStateFinished") ||
            animalToTimeData[animal] <= 0)
        {
            return true;
        }
        return false;
    }
    protected override void ExitPattern(GameObject animal, bool callCallback)
    {
        animal.transform.GetChild(0).transform.localPosition = new Vector3(0, 1, 0); // reset position of the overlay if being modified by animation
        animalToAnimatorData[animal].animator.SetBool("IsStateFinished", true);
        animalToAnimatorData.Remove(animal);
        animalToTimeData.Remove(animal);
        base.ExitPattern(animal, callCallback);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedOverlayAnimation : BehaviorPattern
{
    public string AnimatorBoolName;
    private Dictionary<GameObject, AnimatorData> animalToAnimatorData = new Dictionary<GameObject, AnimatorData>();
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        animalToAnimatorData.Add(animal, new AnimatorData());
        animalToAnimatorData[animal].animator = animal.transform.GetChild(0).GetComponent<Animator>();
        if (animalToAnimatorData[animal].animator.layerCount != 1)
        {
            if (animalToAnimatorData[animal].animator.GetLayerIndex(AnimatorBoolName) != -1)
            {
                animalToAnimatorData[animal].layerIndex = animalToAnimatorData[animal].animator.GetLayerIndex(AnimatorBoolName);
            }
        }
        animalToAnimatorData[animal].initialState = animalToAnimatorData[animal].animator.GetCurrentAnimatorStateInfo(animalToAnimatorData[animal].layerIndex);
        animalToAnimatorData[animal].animator.SetBool("IsStateFinished", false);
        animalToAnimatorData[animal].animator.SetBool(AnimatorBoolName, true);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (animalToAnimatorData[animal].animator.GetBool("IsStateFinished"))
        {
            return true;
        }
        return false;
    }
    protected override void ExitPattern(GameObject gameObject, bool callCallback)
    {
        animalToAnimatorData[gameObject].animator.SetBool("IsStateFinished", true);
        animalToAnimatorData[gameObject].animator.SetBool(AnimatorBoolName, false);
        animalToAnimatorData.Remove(gameObject);
        base.ExitPattern(gameObject, callCallback);
    }
}

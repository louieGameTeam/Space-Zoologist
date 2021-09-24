using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedAnimation : TimedPattern
{
    [SerializeField] protected string AnimatorBoolName;
    private Dictionary<GameObject, AnimatorData> animalToAnimatorData = new Dictionary<GameObject, AnimatorData>();
    [SerializeField] protected bool OverlayAnimation;
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        this.AnimalToTimeElapsed.Add(animal, 0);
        this.animalToAnimatorData.Add(animal, new AnimatorData());
        if (OverlayAnimation)
        {
            animalToAnimatorData[animal].animator = animal.transform.GetChild(0).GetComponent<Animator>();
        }
        else
        {
            animalToAnimatorData[animal].animator = animal.GetComponent<Animator>();
        }
        animalToAnimatorData[animal].initialState = animalToAnimatorData[animal].animator.GetCurrentAnimatorStateInfo(animalToAnimatorData[animal].layerIndex);
        animalToAnimatorData[animal].animator.SetBool("IsStateFinished", false);
        animalToAnimatorData[animal].animator.SetBool(AnimatorBoolName, true);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (this.AnimalToTimeElapsed[animal] > ExitTime)
        {
            return true;
        }
        this.AnimalToTimeElapsed[animal] += Time.deltaTime;
        return false;
    }
    protected override void ExitPattern(GameObject animal, bool callCallback)
    {
        animal.transform.GetChild(0).transform.localPosition = new Vector3(0, 1, 0); // reset position of the overlay if being modified by animation
        animalToAnimatorData[animal].animator.SetBool("IsStateFinished", true);
        animalToAnimatorData[animal].animator.SetBool(AnimatorBoolName, false);
        animalToAnimatorData.Remove(animal);
        base.ExitPattern(animal, callCallback);
    }
}

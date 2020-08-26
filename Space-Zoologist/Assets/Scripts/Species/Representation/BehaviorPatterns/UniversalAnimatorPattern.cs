using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalAnimatorPattern : BehaviorPattern
{
    public string AnimatorTriggerName;
    public string AnimatorLayerName;
    private Dictionary<GameObject, AnimatorData> animalToAnimatorData = new Dictionary<GameObject, AnimatorData>();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        animalToAnimatorData.Add(gameObject, new AnimatorData(gameObject.GetComponent<Animator>()));
        if (animalToAnimatorData[gameObject].animator.layerCount != 1)
        {
            if (animalToAnimatorData[gameObject].animator.GetLayerIndex(AnimatorTriggerName) != -1)
            {
                animalToAnimatorData[gameObject].SetLayerIndex(animalToAnimatorData[gameObject].animator.GetLayerIndex(AnimatorTriggerName));
            }
        }
        animalToAnimatorData[gameObject].SetInitialState(animalToAnimatorData[gameObject].animator.GetCurrentAnimatorStateInfo(animalToAnimatorData[gameObject].layerIndex));
        animalData.animal.SetAnimatorTrigger(AnimatorTriggerName);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (!animalToAnimatorData[animal].StateChange)
        {
            if (animalToAnimatorData[animal].animator.GetCurrentAnimatorStateInfo(animalToAnimatorData[animal].layerIndex).GetHashCode() != animalToAnimatorData[animal].initialState.GetHashCode())
            {
                animalToAnimatorData[animal].ToggleStateChange();
            }
        }
        else
        {
            if(animalToAnimatorData[animal].animator.GetCurrentAnimatorStateInfo(animalToAnimatorData[animal].layerIndex).GetHashCode() == animalToAnimatorData[animal].initialState.GetHashCode())
            {
                return true;
            }
        }
        return false;
    }
    protected override void ExitPattern(GameObject gameObject)
    {
        animalToAnimatorData.Remove(gameObject);
    }
    private struct AnimatorData
    {
        public Animator animator;
        public bool StateChange;
        public AnimatorStateInfo initialState;
        public int layerIndex;
        public AnimatorData(Animator animator)
        {
            this.animator = animator;
            this.StateChange = false;
            initialState = new AnimatorStateInfo();
            layerIndex = 0;
        }
        public void SetLayerIndex(int index)
        {
            layerIndex = index;
        }
        public void SetInitialState(AnimatorStateInfo animatorStateInfo)
        {
            initialState = animatorStateInfo;
        }
        public void ToggleStateChange()
        {
            StateChange = true;
        }
    }
}

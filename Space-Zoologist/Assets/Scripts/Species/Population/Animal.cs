using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public MovementData MovementData { get; private set; }
    public Population PopulationInfo { get; private set; }
    private Animator Animator = null;
    public MovementController MovementController {get; set; }

    public void Start()
    {
        if (!this.gameObject.TryGetComponent(out this.Animator))
        {
            this.Animator = null;
            Debug.Log("Animator component not attached");
        }
        this.MovementController = this.gameObject.GetComponent<MovementController>();
    }

    public void Initialize(Population population, MovementData data)
    {
        this.MovementData = data;
        this.PopulationInfo = population;
        this.gameObject.GetComponent<Animator>().runtimeAnimatorController = this.PopulationInfo.Species.AnimatorController;
    }

    public void SetAnimatorTrigger(string triggerName)
    {
        Animator.SetTrigger(triggerName);
    }
    public void SetAnimatorBool(string boolName, bool value)
    {
        Animator.SetBool(boolName, value);
    }
    public void SetAnimatorInt(string intName, int value)
    {
        Animator.SetInteger(intName, value);
    }
    public void SetAnimatorFloat(string floatName, float value)
    {
        Animator.SetFloat(floatName, value);
    }
}

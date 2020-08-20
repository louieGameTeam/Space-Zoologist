using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO refactor so automotan movement can be switched to when needed
public delegate void BehaviorFinished();
public class Animal : MonoBehaviour
{
    public MovementData BehaviorsData { get; private set; }
    public Population PopulationInfo { get; private set; }
    private Animator Animator = null;
    private BehaviorFinished OnBehaviorFinished { get; set; }
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
        this.BehaviorsData = data;
        this.PopulationInfo = population;
        this.gameObject.GetComponent<Animator>().runtimeAnimatorController = this.PopulationInfo.Species.AnimatorController;
        // this.gameObject.GetComponent<AutomatonMovement>().Initialize(this.PopulationInfo);
        // this.BehaviorComponents = new Dictionary<string, Behavior>();
        // foreach (BehaviorScriptTranslation component in this.PopulationInfo.Species.Behaviors)
        // {
        //     this.AddBehaviorByName(component);
        // }
        // // TODO test if this can be done in the above loop
        // foreach (Behavior behaviorComponent in this.gameObject.GetComponents<Behavior>())
        // {
        //     // Debug.Log("Behavior mapped: " +  behaviorComponent.GetType().ToString());
        //     this.BehaviorComponents.Add(behaviorComponent.GetType().ToString(), behaviorComponent);
        // }
        // this.ChooseNextBehavior();
    }

    public void ResetBehavior()
    {
        // if (this.gameObject.activeSelf && this.CurrentBehavior != null)
        // {
        //     this.CurrentBehavior.ExitBehavior();
        //     this.ChooseNextBehavior();
        // }
    }

    // Gets a random behaviorScriptName from currentBehaviors in BehaviorData and then uses the BehaviorComponents dictionary to get out the hashed component
    // private void ChooseNextBehavior()
    // {
    //     // TODO replace with Caleb's increased random probability function
    //     System.Random random = new System.Random();
    //     if (this.PopulationInfo.CurrentBehaviors.Count == 0)
    //     {
    //         Debug.Log("No behaviors to choose from");
    //         return;
    //     }
    //     int randNum = random.Next(this.PopulationInfo.CurrentBehaviors.Count);
    //     string chosenBehavior = this.PopulationInfo.CurrentBehaviors[randNum].ToString();
    //     this.CurrentBehavior = this.BehaviorComponents[chosenBehavior];
    //     this.OnBehaviorFinished = this.ChooseNextBehavior;
    //     this.CurrentBehavior.EnterBehavior(this.OnBehaviorFinished);
    // }

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

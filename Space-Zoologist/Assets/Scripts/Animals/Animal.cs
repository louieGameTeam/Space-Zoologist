using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void BehaviorFinished();
public class Animal : MonoBehaviour
{
    public BehaviorsData BehaviorsData { get; private set; }
    public Population PopulationInfo { get; private set; }

    private Animator Animator = null;
    private Behavior CurrentBehavior { get; set; }
    private BehaviorFinished OnBehaviorFinished { get; set; }
    // Behavior components hashed to their name
    private Dictionary<string, Behavior> BehaviorComponents { get; set; }

    public void Start()
    {
        if (!this.gameObject.TryGetComponent(out this.Animator))
        {
            this.Animator = null;
        }
    }

    public void Initialize(Population population, BehaviorsData data)
    {
        this.BehaviorsData = data;
        this.PopulationInfo = population;
        this.gameObject.GetComponent<Animator>().runtimeAnimatorController = this.PopulationInfo.Species.AnimatorController;
        this.BehaviorComponents = new Dictionary<string, Behavior>();
        foreach (Behavior behaviorComponent in this.gameObject.GetComponents<Behavior>())
        {   
            // Debug.Log("Behavior mapped: " +  behaviorComponent.GetType().ToString());
            this.BehaviorComponents.Add(behaviorComponent.GetType().ToString(), behaviorComponent); 
        }
        this.ChooseNextBehavior();
    }

    // Gets a random beaviorScriptName from currentBehaviors in BehaviorData and then uses the BehaviorComponents dictionary to get out the hashed component
    private void ChooseNextBehavior()
    {
        // TODO replace with Caleb's increased random probability function
        System.Random rand = new System.Random();
        string chosenBehavior = this.PopulationInfo.CurrentBehaviors[rand.Next(this.PopulationInfo.CurrentBehaviors.Count)].ToString();
        //Debug.Log("Behavior chosen: " + chosenBehavior);
        this.CurrentBehavior = this.BehaviorComponents[chosenBehavior];
        this.OnBehaviorFinished = ChooseNextBehavior;
        this.CurrentBehavior.EnterBehavior(this.OnBehaviorFinished);
    }

    void Update()
    {
        this.UpdateAnimations();
    }

    public void UpdateAnimations()
    {
        this.Animator.SetInteger("Movement", (int)this.BehaviorsData.MovementStatus);
        this.Animator.SetInteger("Direction", (int)this.BehaviorsData.CurrentDirection);
    }
}

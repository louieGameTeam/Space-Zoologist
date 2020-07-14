﻿using System.Collections;
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
        foreach (BehaviorScriptTranslation component in this.PopulationInfo.Species.Behaviors)
        {
            this.AddBehaviorByName(component);
        }
        // TODO test if this can be done in the above loop
        foreach (Behavior behaviorComponent in this.gameObject.GetComponents<Behavior>())
        {
            // Debug.Log("Behavior mapped: " +  behaviorComponent.GetType().ToString());
            this.BehaviorComponents.Add(behaviorComponent.GetType().ToString(), behaviorComponent);
        }
        this.ChooseNextBehavior();
    }

    //TODO make addBehavior function

    // Gets a random behaviorScriptName from currentBehaviors in BehaviorData and then uses the BehaviorComponents dictionary to get out the hashed component
    private void ChooseNextBehavior()
    {
        // TODO replace with Caleb's increased random probability function
        System.Random rand = new System.Random();
        if (this.PopulationInfo.CurrentBehaviors.Count == 0)
        {
            Debug.Log("No behaviors to choose from");
            return;
        }
        int randNum = rand.Next(this.PopulationInfo.CurrentBehaviors.Count);
        string chosenBehavior = this.PopulationInfo.CurrentBehaviors[randNum].ToString();
        // Debug.Log("Behavior chosen: " + chosenBehavior);
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
        if (this.Animator != null && this.BehaviorsData != null)
        {
            this.Animator.SetInteger("Movement", (int)this.BehaviorsData.MovementStatus);
            this.Animator.SetInteger("Direction", (int)this.BehaviorsData.CurrentDirection);
        }
    }

    // No way to dynamically add scripts by name, have to use if statements and add to this everytime new behavior is defined
    // TODO need to check if a behavior has been added already
    public void AddBehaviorByName(BehaviorScriptTranslation component)
    {
        switch(component.behaviorScriptName)
        {
            case BehaviorScriptName.RandomMovement:
                this.gameObject.AddComponent<RandomMovement>();
                break;
            case BehaviorScriptName.Idle:
                this.gameObject.AddComponent<Idle>();
                break;
            case BehaviorScriptName.GetFood:
                this.gameObject.AddComponent<GetFood>();
                break;
            default:
                Debug.Log("No component with the type found");
                break;
        }
    }
}

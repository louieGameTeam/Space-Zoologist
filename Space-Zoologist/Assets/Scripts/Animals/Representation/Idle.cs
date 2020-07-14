using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : Behavior
{
    private float IdleTime = 0f;
    private float WalkCountDown = 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        if (base.Animal.BehaviorsData.IdleTimeBetweenBehaviors < this.IdleTime)
        {
            this.IdleTime = 0f;
            WalkCountDown = 0f;
            base.ExitBehavior();
        } else if (WalkCountDown > 0.5f) {

        }
        else
        {
            this.IdleTime += Time.deltaTime;
            WalkCountDown += Time.deltaTime;
            base.Animal.BehaviorsData.MovementStatus = Movement.idle;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : Behavior
{
    private float IdleTime = 0f;

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
            base.ExitBehavior();
        }
        else
        {
            this.IdleTime += Time.deltaTime;
            base.Animal.BehaviorsData.MovementStatus = Movement.idle;
        }
    }
}

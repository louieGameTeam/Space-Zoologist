using System.Collections.Generic;

// Behavior that will be displayed when a need is not being met
public enum BehaviorScriptName {RandomMovement, Idle, Eating, None}
// Core behaviors that most animals will display
public enum CoreBehavior {Eat, Drink, Mate, Sleep, MoveRandomly}

[System.Serializable]
public class BehaviorScriptTranslation
{
    public NeedType NeedType;
    public BehaviorScriptName behaviorScriptName; 
}

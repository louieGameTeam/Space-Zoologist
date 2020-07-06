using System.Collections.Generic;

// TODO add more behaviors (and modify animal to support these behavior components being added)

// Behavior that will be displayed when a need is not being met
public enum BehaviorScriptName {RandomMovement, Idle, None}

[System.Serializable]
public class BehaviorScriptTranslation
{
    public NeedType NeedType;
    public BehaviorScriptName behaviorScriptName;
}

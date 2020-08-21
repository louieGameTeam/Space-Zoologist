using System.Collections.Generic;

// Behavior that will be displayed when a need is not being met
[System.Serializable]
public class BehaviorScriptTranslation
{
    public NeedType NeedType;
    public SpecieBehaviorTrigger Behavior;
}

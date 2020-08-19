using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorPatternUpdater : MonoBehaviour
{
    public List<BehaviorPattern> behaviorPatterns = new List<BehaviorPattern>();
    public bool isPaused = false;
    // TODO manage pause for all behaviors
    void Update()
    {
        if (isPaused)
        {
            return;
        }

        foreach (BehaviorPattern behaviorPattern in behaviorPatterns)
        {
            behaviorPattern.UpdatePattern();
        }
    }
    public void RegisterPattern (BehaviorPattern behaviorPattern)
    {
        if (!behaviorPatterns.Contains(behaviorPattern))
        {
            behaviorPatterns.Add(behaviorPattern);
        }
    }
}

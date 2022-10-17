using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorPatternUpdater : MonoBehaviour
{
    public List<BehaviorPattern> behaviorPatterns = new List<BehaviorPattern>();

    void Update()
    {
        if (GameManager.Instance.IsPaused)
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
            behaviorPattern.Init();
            behaviorPatterns.Add(behaviorPattern);
        }
    }

    public void RegisterPopulation(Population population)
    {
        foreach (PopulationBehavior behaviorTrigger in population.GetComponent<PopulationBehaviorManager>().GetAllUsedBehaviors())
        {
            foreach (BehaviorPattern behaviorPattern in behaviorTrigger.behaviorPatterns)
            {
                this.RegisterPattern(behaviorPattern);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorPatternUpdater : MonoBehaviour
{
    public List<BehaviorPattern> behaviorPatterns = new List<BehaviorPattern>();
    // TODO manage pause for all behaviors
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
            behaviorPattern.StartUp();
            behaviorPatterns.Add(behaviorPattern);
        }
    }

    public void RegisterPopulation(Population population)
    {
        // Temp
        PopulationBehaviorManager populationBehaviorManager = FindObjectOfType<PopulationBehaviorManager>();
        foreach (PopulationBehavior behaviorTrigger1 in populationBehaviorManager.tempBehaviors)
        {
            foreach (BehaviorPattern behaviorPattern in behaviorTrigger1.behaviorPatterns)
            {
                this.RegisterPattern(behaviorPattern);
            }
        }
        // Register unique behaviors
        foreach (PopulationBehavior behaviorTrigger in population.Species.GetBehaviors())
        {
            foreach (BehaviorPattern behaviorPattern in behaviorTrigger.behaviorPatterns)
            {
                this.RegisterPattern(behaviorPattern);
            }
        }
        // Register default behaviors
        foreach (PopulationBehavior behaviorTrigger in population.DefaultBehaviors)
        {
            foreach (BehaviorPattern behaviorPattern in behaviorTrigger.behaviorPatterns)
            {
                this.RegisterPattern(behaviorPattern);
            }
        }
    }
}

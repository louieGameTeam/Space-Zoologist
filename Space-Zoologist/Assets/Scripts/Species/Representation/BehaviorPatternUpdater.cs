using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorPatternUpdater : MonoBehaviour
{
    [SerializeField] GridSystem GridSystem = default;
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
            behaviorPattern.StartUp();
            behaviorPatterns.Add(behaviorPattern);
            behaviorPattern.SetupDependencies(this.GridSystem);
        }
    }

    public void RegisterPopulation(Population population)
    {
        // Register unique behaviors
        foreach (SpecieBehaviorTrigger behaviorTrigger in population.Species.GetBehaviors())
        {
            foreach (BehaviorPattern behaviorPattern in behaviorTrigger.behaviorPatterns)
            {
                this.RegisterPattern(behaviorPattern);
            }
        }
        // Register default behaviors
        foreach (SpecieBehaviorTrigger behaviorTrigger in population.DefaultBehaviors)
        {
            foreach (BehaviorPattern behaviorPattern in behaviorTrigger.behaviorPatterns)
            {
                this.RegisterPattern(behaviorPattern);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SelectionType { All, FreeOnly, ConcurrentOnly, OverrideOnly, FreeAndConcurrent, FreeAndOverride, ConcurrentAndOverride };
public class BehaviorUtils
{
    public static BehaviorType[][] exclusiveBehaviors = { new BehaviorType[] { BehaviorType.Movement, BehaviorType.Animation }
                                                        , new BehaviorType[] { BehaviorType.Mixed, BehaviorType.Movement, BehaviorType.Animation, BehaviorType.ColorChange, BehaviorType.LayerOverlay} };
    /// <summary>
    /// Helper function iterates and compares all combinations
    /// </summary>
    /// <param name="activeBehaviors"></param>
    /// <param name="triggerBehaviorData"></param>
    /// <returns></returns>
    public static bool IsBehaviorConflicting(List<BehaviorData> activeBehaviors, BehaviorData triggerBehaviorData)
    {
        foreach (BehaviorData animalBehaviorData in activeBehaviors)
        {
            foreach (BehaviorType[] subArray in exclusiveBehaviors)
            {
                foreach (BehaviorType type1 in animalBehaviorData.behaviorTypes)
                {
                    foreach (BehaviorType type2 in triggerBehaviorData.behaviorTypes)
                    {
                        if (Array.Exists(subArray, x => x == type1) && Array.Exists(subArray, x => x == type2))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Selects chosen amount of animals from the dictionary
    /// </summary>
    /// <param name="amount">Number of animals needed, usually consistent with Number Trigger per loop defined in behavior prefab</param>
    /// <param name="availabilityToAnimals">The same dictionary that is passed into the animal selection function</param>
    /// <param name="selectionType">Types of availability will be selectred, defauts to all, priority free > concurrent > override </param>
    /// <returns></returns>
    public static List<GameObject> SelectAnimals(int amount, Dictionary<Availability, List<GameObject>> availabilityToAnimals, SelectionType selectionType = SelectionType.All)
    {
        int selectedCount = 0;
        List<GameObject> selectedAnimals = new List<GameObject>();
        if (selectionType == SelectionType.All || selectionType == SelectionType.FreeOnly || selectionType == SelectionType.FreeAndConcurrent || selectionType == SelectionType.FreeAndOverride)
        {
            foreach (GameObject freeAnimal in availabilityToAnimals[Availability.Free])
            {
                if (selectedCount < amount)
                {
                    selectedAnimals.Add(freeAnimal);
                    selectedCount++;
                    continue;
                }
                return selectedAnimals;
            }
        }
        if (selectionType == SelectionType.All || selectionType == SelectionType.ConcurrentOnly || selectionType == SelectionType.FreeAndConcurrent || selectionType == SelectionType.ConcurrentAndOverride)
        {
            foreach (GameObject concurrentAnimal in availabilityToAnimals[Availability.Concurrent])
            {
                if (selectedCount < amount)
                {
                    selectedAnimals.Add(concurrentAnimal);
                    selectedCount++;
                    continue;
                }
                return selectedAnimals;
            }
        }
        if (selectionType == SelectionType.All || selectionType == SelectionType.OverrideOnly || selectionType == SelectionType.FreeAndOverride || selectionType == SelectionType.ConcurrentAndOverride)
        {
            foreach (GameObject overrideAnimal in availabilityToAnimals[Availability.Override])
            {
                if (selectedCount < amount)
                {
                    selectedAnimals.Add(overrideAnimal);
                    selectedCount++;
                    continue;
                }
                return selectedAnimals;
            }
        }
        if (selectedCount == amount)
        {
            return selectedAnimals;
        }
        selectedAnimals.Clear();
        return selectedAnimals;
    }
    public static bool IsTargetValueReached(float original, float target, float current)
    {
        if (target > original)
        {
            if (current < target)
            {
                return false;
            }
        }
        else if(target < original)
        {
            if (current > target)
            {
                return false;
            }
        }
        return true;
    }
}

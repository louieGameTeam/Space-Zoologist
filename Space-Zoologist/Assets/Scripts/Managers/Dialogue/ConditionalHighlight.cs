using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalHighlight
{
    public Func<bool> predicate;
    public bool invert = false;
    public Func<RectTransform> target;

    public bool Target(TutorialHighlight highlight)
    {
        bool result = predicate.Invoke();

        // Invert result if that is what we should do
        if (invert) result = !result;

        // If the predicate is true
        if (result)
        {
            RectTransform target = this.target.Invoke();

            // Target the rect transform received
            if (target)
            {
                // If the target of the highlight is not already this object then set the target
                if (target != highlight.Root.parent) highlight.Target(target);
                return true;
            }
            // If no target received log it and return false
            else
            {
                Debug.Log($"{nameof(ConditionalHighlight)}: Target functor " +
                    $"failed to find a target");
                return false;
            }
        }
        else return false;
    }
}
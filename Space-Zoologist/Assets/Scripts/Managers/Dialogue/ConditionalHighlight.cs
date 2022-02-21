using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalHighlight
{
    #region Public Fields
    public Func<bool> predicate;
    public bool invert = false;
    public Func<RectTransform> target;
    #endregion

    #region Factories
    public static ConditionalHighlight NoTarget(Func<bool> predicate, bool invert = false)
    {
        return new ConditionalHighlight()
        {
            predicate = predicate,
            invert = invert,
            target = () => null
        };
    }
    #endregion

    #region Public Methods
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
            // If the target of the highlight is not already this object then set the target
            if (target && target != highlight.Root.parent) highlight.Target(target);

            // Return true since the predicate was true
            return true;
        }
        else return false;
    }
    #endregion
}
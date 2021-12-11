using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DialogueEditor;

/// <summary>
/// Simple object used by the tutorial manager to schedule coroutines,
/// because the tutorial manager stays stuck on a prefab
/// </summary>
public class ConversationFreezingScheduler : MonoBehaviour
{
    #region Private Fields
    private Coroutine freezeConversationRoutine;
    #endregion

    #region Public Methods
    public void FreezeUntilConditionIsMet(Func<bool> predicate)
    {
        if(freezeConversationRoutine != null)
        {
            StopCoroutine(freezeConversationRoutine);
            freezeConversationRoutine = null;
        }

        // Start the coroutine
        freezeConversationRoutine = StartCoroutine(FreezeConversationUntilConditionIsMetRoutine(predicate));
    }
    #endregion

    #region Private Methods
    private IEnumerator FreezeConversationUntilConditionIsMetRoutine(Func<bool> predicate)
    {
        if (ConversationManager.Instance)
        {
            ConversationManager.Instance.FreezeConversation();
            yield return new WaitUntil(predicate);
            ConversationManager.Instance.UnfreezeConversation();
        }
        else yield return null;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetScrollOnEnable : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the scroll view to reset on enable")]
    private ScrollRect scrollRect;
    #endregion

    #region Monobehaviour Messages
    private void OnEnable()
    {
        scrollRect.verticalNormalizedPosition = 1;
    }
    #endregion

    #region Public Methods
    public void SetScrollRect(ScrollRect scrollRect)
    {
        this.scrollRect = scrollRect;
    }
    #endregion
}

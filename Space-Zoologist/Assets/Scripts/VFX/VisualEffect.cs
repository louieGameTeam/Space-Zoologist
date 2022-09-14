using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffect : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private Animator animator;
    #endregion

    #region Monobehaviour Callbacks
    private void Update()
    {
        if (HasFinishedPlaying())
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Check for whether current VisualEffect has finished playing
    /// </summary>
    private bool HasFinishedPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }
    #endregion
}
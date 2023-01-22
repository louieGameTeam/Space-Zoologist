using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple monobehavior for playing an inspector set animation clip
/// </summary>
public class SimpleAnimatorPlayer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string stateName;
    [SerializeField] private int animLayerIndex;
    
    [Header("Play On Enable")]
    [SerializeField] private bool playOnEnable;
    [SerializeField] private float delay;

    private void OnEnable()
    {
        if (playOnEnable)
            StartCoroutine(CoroutPlayDelayed());
    }

    private IEnumerator CoroutPlayDelayed()
    {
        yield return new WaitForSeconds(delay);
        PlayAnim();
    }
    

    public void PlayAnim()
    {
        if (!animator.HasState(animLayerIndex, Animator.StringToHash(stateName)))
        {
            Debug.LogError($"{gameObject} Unable to play anim of string {stateName} in layer {animLayerIndex}");
        }
        else
        {
            animator.Play(stateName);
        }
    }
}

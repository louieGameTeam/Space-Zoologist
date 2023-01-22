using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireworkEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Tooltip("Total number of firework variants (used for randomize range)")]
    [SerializeField] private int variantsCount;

    private void OnEnable()
    {
        // Randomize position
        float heightExt = Screen.currentResolution.height / 2.25f;
        float widthExt = Screen.currentResolution.width / 2.25f;
        GetComponent<RectTransform>().anchoredPosition = new Vector3(
            Random.Range(-widthExt, widthExt), 
            Random.Range(-heightExt, heightExt), 
            0f);
        
        // Play random firework animation (states must be named after numbers)
        animator.Play(UnityEngine.Random.Range(0,variantsCount).ToString());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    private Species _species;
    public Species species { get; set; }

    private void Awake()
    {
        gameObject.AddComponent<SpriteRenderer>();
        
    }

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = species.sprite;
    }
}

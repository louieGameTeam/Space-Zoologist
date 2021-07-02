using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManagerTester : MonoBehaviour
{
    private List<GameObject> Animals = new List<GameObject>();
    public int Days;
    public float time;
    private DayManager dayManager;
    private void Awake()
    {
        dayManager = this.gameObject.GetComponent<DayManager>();
        Animal[] animals = FindObjectsOfType<Animal>();
        foreach (Animal animal in animals)
        {
            Animals.Add(animal.gameObject);
        }

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dayManager.RegisterAnimalDeath(Animals[0], Days, time);
            Animals.RemoveAt(0);
        }
    }
}

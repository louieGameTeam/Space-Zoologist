using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDeathInfo
{
    public GameObject animal;
    public int DeathDay;
    public float DeathTime;
    public AnimalDeathInfo(GameObject gameObject, int day, float time)
    {
        animal = gameObject;
        DeathDay = day;
        DeathTime = time;
    }
}

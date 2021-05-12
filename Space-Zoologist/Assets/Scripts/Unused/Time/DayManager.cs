using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    //TODO? Save number of days passed
    public float SecondsPerDay;
    [SerializeField] private float secondsPassed = 0;
    [SerializeField] private bool isDay;
    public float DayTime = 0.2f;
    public float NightTime = 0.8f;
    public float SpeedMult = 1;
    private Dictionary<GameObject, AnimalDeathInfo> animalsToDeathInfos = new Dictionary<GameObject, AnimalDeathInfo>();
    private void Awake()
    {
        if (SecondsPerDay <= 0)
        {
            Debug.LogError("Seconds per day must be positive, set time in DayManager");
        }
        this.animalsToDeathInfos = new Dictionary<GameObject, AnimalDeathInfo>();
    }
    private void Update()
    {
        this.secondsPassed += Time.deltaTime * this.SpeedMult;
        if (this.secondsPassed <= this.SecondsPerDay)
        {
            this.ProcessTimePassage();
            return;
        }
        this.NextDay();
    }
    private void EnterNightTime()
    {
        this.isDay = false;
        //TODO?
    }
    private void EnterDayTime()
    {
        this.isDay = true;
        //TODO?
    }
    public void ProcessTimePassage()
    {
        float currentTime = this.secondsPassed / this.SecondsPerDay;
        if (this.isDay && currentTime > this.NightTime)
        {
            this.EnterNightTime();
        }
        else if(!this.isDay && (currentTime > this.DayTime && currentTime <= this.NightTime))
        {
            this.EnterDayTime();
        }
        foreach (AnimalDeathInfo animalDeathInfo in this.animalsToDeathInfos.Values)
        {
            if (animalDeathInfo.DeathDay == 0 && currentTime >= animalDeathInfo.DeathTime)
            {
                KillAnimal(animalDeathInfo.animal);
            }
        }

    }
    public void NextDay()
    {
        this.secondsPassed -= this.SecondsPerDay;//Reset Timer, but pass the extra time to next day
        foreach (AnimalDeathInfo animalDeathInfo in this.animalsToDeathInfos.Values)
        {
            animalDeathInfo.DeathDay -= 1;
            if (animalDeathInfo.DeathDay == -1)
            {
                KillAnimal(animalDeathInfo.animal); 
            }
        }
    }
    private void KillAnimal(GameObject animal)
    {
        this.animalsToDeathInfos.Remove(animal);
        Destroy(animal);// Use OnDestroy() on animal classes?
    }
    /// <summary>
    /// Register a animal death
    /// </summary>
    /// <param name="gameObject">The animal to die</param>
    /// <param name="day">Number of days until the animal dies, 0 means on the current day</param>
    /// <param name="time">Part of the day passed until the animal dies, 0 is the beginning of the day, 1 is the end</param>
    /// <returns>Whether the death is registered successfully</returns>
    public bool RegisterAnimalDeath(GameObject gameObject, int day, float time)
    {
        if (animalsToDeathInfos.ContainsKey(gameObject))
        {
            return false;
        }
        animalsToDeathInfos.Add(gameObject, new AnimalDeathInfo(gameObject, day, time));
        return true;
    }
    public bool InterruptAnimalDeath(GameObject gameObject)
    {
        return this.animalsToDeathInfos.Remove(gameObject);
    }
}

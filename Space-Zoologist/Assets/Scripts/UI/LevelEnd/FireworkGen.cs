using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level end screen firework vfx generator.
/// We skip using pooling since performance likely isnt a concern during the end screen
/// </summary>
public class FireworkGen : MonoBehaviour
{
    [SerializeField] private GameObject fireworkPrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float lifeTime;
    [SerializeField] private Transform fireworkParentTransform;

    class FireworkInstance
    {
        public GameObject firework;
        public float spawnTime;
    }

    private Queue<FireworkInstance> fireworkInstances = new();
    private float spawnTime = 0f;
    private bool spawning;

    public void SetSpawning(bool val)
    {
        spawning = val;
    }
    
    private void Update()
    {
        // Check for any firework instances that pass the lifetime
        if (fireworkInstances.TryPeek(out FireworkInstance instance))
        {
            if (Time.time - instance.spawnTime > lifeTime)
            {
                Destroy(instance.firework);
                fireworkInstances.Dequeue();
            }
        }
        
        // Spawn new fireworks
        if (spawning && Time.time - spawnTime >= spawnInterval)
        {
            spawnTime = Time.time;
            SpawnFirework();
        }
    }

    private void SpawnFirework()
    {
        fireworkInstances.Enqueue(new FireworkInstance()
        {
            firework = Instantiate(fireworkPrefab, fireworkParentTransform),
            spawnTime = Time.time
        });
    }
}

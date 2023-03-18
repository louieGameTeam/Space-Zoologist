using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// Level end screen firework vfx generator.
/// We skip using pooling since performance likely isnt a concern during the end screen
/// </summary>
public class FireworkGen : MonoBehaviour
{
    [SerializeField] private GameObject fireworkPrefab;
    [SerializeField] private Vector2 spawnInterval;
    [SerializeField] private float lifeTime;
    [SerializeField] private Transform fireworkParentTransform;

    private float nextSpawnDelay;
    
    class FireworkInstance
    {
        public GameObject firework;
        public float spawnTime;
    }

    private Queue<FireworkInstance> fireworkInstances = new();
    private float prevSpawnTime = 0f;
    private bool isSpawning;

    public void SetIsSpawning(bool val)
    {
        isSpawning = val;
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
        if (isSpawning && Time.time - prevSpawnTime >= nextSpawnDelay)
        {
            prevSpawnTime = Time.time;
            SpawnFirework();
            nextSpawnDelay = UnityEngine.Random.Range(spawnInterval.x, spawnInterval.y);
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

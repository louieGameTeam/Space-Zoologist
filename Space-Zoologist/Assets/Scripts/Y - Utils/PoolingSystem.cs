using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    // Keeps track of all inactive pooled objects
    private List<GameObject> PooledObjects = new List<GameObject>();
    /// <summary>
    /// Adds inactive gameobjects to the list
    /// </summary>
    /// <param name="pooledObjects"></param>
    /// <param name="numObjectsToPool"></param>
    /// <param name="objectToPool"></param>
    public void AddPooledObjects(int numObjectsToPool, GameObject objectToPool)
    {
        for (int i = 0; i < numObjectsToPool; i++)
        {
            GameObject newObject = Instantiate(objectToPool, this.gameObject.transform);
            newObject.SetActive(false);
            this.PooledObjects.Add(newObject);
        }
    }

    /// <summary>
    /// Finds the first inactive gameobject and returns it as active. Returns null if no active objects found.
    /// </summary>
    /// <param name="listToPullFrom"></param>
    /// <returns></returns>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < PooledObjects.Count; i++) {
            GameObject pooled = PooledObjects[i];
            if (!pooled.activeSelf)
            {
                PooledObjects.RemoveAt(i);
                pooled.SetActive(true);
                return pooled;
            }
        }
        return null;
    }

    /// <summary>
    /// Finds the first inactive gameobject and returns it as active. Returns a new instance of the poolable prefab if no active objects found.
    /// </summary>
    /// <param name="listToPullFrom"></param>
    /// <returns></returns>
    public GameObject GetGuaranteedPooledObject(GameObject poolablePrefab)
    {
        for (int i = 0; i < PooledObjects.Count; i++) {
            GameObject pooled = PooledObjects[i];
            if (!pooled.activeSelf)
            {
                PooledObjects.RemoveAt(i);
                pooled.SetActive(true);
                return pooled;
            }
        }
        return Instantiate(poolablePrefab, this.gameObject.transform);
    }

    public void ReturnObjectToPool(GameObject gameObject)
    {
        this.PooledObjects.Add(gameObject);
    }
}

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
    public GameObject GetPooledObject(List<GameObject> listToAddTo)
    {
        for (int i = 0; i < PooledObjects.Count; i++) {
            if (!PooledObjects[i].activeSelf)
            {
                PooledObjects[i].SetActive(true);
                listToAddTo.Add(PooledObjects[i]);
                PooledObjects.RemoveAt(i);
                return listToAddTo[listToAddTo.Count - 1];
            }
        }
        return null;
    }

    public void ReturnObjectToPool(GameObject gameObject)
    {
        this.PooledObjects.Add(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    /// <summary>
    /// Adds inactive gameobjects to the list
    /// </summary>
    /// <param name="pooledObjects"></param>
    /// <param name="numObjectsToPool"></param>
    /// <param name="objectToPool"></param>
    public void AddPooledObjects(List<GameObject> pooledObjects, int numObjectsToPool, GameObject objectToPool)
    {
        for (int i = 0; i < numObjectsToPool; i++)
        {
            GameObject newObject = Instantiate(objectToPool, this.gameObject.transform);
            newObject.SetActive(false);
            pooledObjects.Add(newObject);
        }
    }

    /// <summary>
    /// Finds the first inactive gameobject and returns it as active. Returns null if no active objects found.
    /// </summary>
    /// <param name="listToPullFrom"></param>
    /// <returns></returns>
    public GameObject GetPooledObject(List<GameObject> listToPullFrom)
    {
        for (int i = 0; i < listToPullFrom.Count; i++) {
            if (!listToPullFrom[i].activeSelf)
            {
                listToPullFrom[i].SetActive(true);
                return listToPullFrom[i];
            }
        }
        return null;
    }
}

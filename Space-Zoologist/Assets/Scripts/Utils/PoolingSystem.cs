using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    public List<GameObject> AddPooledObjects(int numObjectsToPool, GameObject objectToPool)
    {
        List<GameObject> listToAddTo = new List<GameObject>();
        for (int i = 0; i < numObjectsToPool; i++)
        {
            listToAddTo.Add(Instantiate(objectToPool));
        }
        return listToAddTo;
    }

    public GameObject GetPooledObject(List<GameObject> listToPullFrom)
    {
        for (int i = 0; i < listToPullFrom.Count; i++) {
            if (!listToPullFrom[i].activeInHierarchy) {
            return listToPullFrom[i];
            }
        }
        return null;
    }
}

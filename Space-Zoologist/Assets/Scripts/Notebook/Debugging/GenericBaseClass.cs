using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericBaseClass<T>
{
    [SerializeField] private GameObject parent;
    [SerializeField] private T item;
}

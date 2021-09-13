using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecifiedDerivedClass<T> : MonoBehaviour
{
    [SerializeField] private GenericBaseClass<T> item;
}

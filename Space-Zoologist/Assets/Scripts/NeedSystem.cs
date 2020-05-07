using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class NeedSystem : MonoBehaviour
{
    public string NeedName => needName;
    [SerializeField] protected string needName = default;
    protected HashSet<Population> populations = new HashSet<Population>();
    virtual public bool AddPopulation(Population population)
    {
        return populations.Add(population);
    }
    virtual public bool RemovePopulation(Population population)
    {
        return populations.Remove(population);
    }
    abstract public void UpdateSystem();
}

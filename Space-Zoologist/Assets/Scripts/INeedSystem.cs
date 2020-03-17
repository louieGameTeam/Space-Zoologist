using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface INeedSystem
{
    void RegisterPopulation(Population population);
    void UnregisterPopulation(Population population);
    NeedType Need { get; }
}



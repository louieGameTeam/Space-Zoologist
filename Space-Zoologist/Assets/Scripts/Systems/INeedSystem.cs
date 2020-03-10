using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface INeedSystem
{
    void RegisterPopulation(AnimalPopulation population);
    void UnregisterPopulation(AnimalPopulation population);
    string NeedName();
}
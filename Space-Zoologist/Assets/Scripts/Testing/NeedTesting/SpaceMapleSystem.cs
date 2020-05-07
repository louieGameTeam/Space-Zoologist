using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMapleSystem : NeedSystem
{

    

    public override void UpdateSystem()
    {
        foreach (Population population in populations)
        {
            population.UpdateNeed(NeedName, Random.value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

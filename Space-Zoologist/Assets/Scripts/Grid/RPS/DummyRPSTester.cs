using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRPSTester : MonoBehaviour
{
    DummyRPS rps;
    public Population population;
    // Start is called before the first frame update
    void Start()
    {
        rps = DummyRPS.ins;
        Invoke("Run", 0.3f);
    }

    // Update is called once per frame
    void Run()
    {
        List<Vector3Int> list = GetComponent<DummyRPS>().GetLocationsWithAccess(population);

        // debug
        print(list.Count);
    }
}

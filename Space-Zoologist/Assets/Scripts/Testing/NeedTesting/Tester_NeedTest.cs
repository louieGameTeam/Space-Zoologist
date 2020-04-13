using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester_NeedTest : MonoBehaviour
{
    public Species species = default;
    public float currentValue = default;

    private void OnValidate()
    {
        Debug.Log(species.GetNeedCondition(NeedType.GasX, currentValue));
    }
}

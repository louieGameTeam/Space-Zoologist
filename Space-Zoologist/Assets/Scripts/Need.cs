using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class Need : ScriptableObject
{
    [SerializeField] private List<float> thresholds = default;
    [SerializeField] private List<NeedCondition> conditions = default;
    [SerializeField] private string needName = default;

    public string Name { get => needName; set => needName = value; }

    public void OnValidate()
    {
        while (conditions.Count < thresholds.Count + 1)
        {
            conditions.Add(NeedCondition.Bad);
        }
        while (conditions.Count> thresholds.Count + 1)
        {
            conditions.RemoveAt(conditions.Count - 1);
        }

        for(var i = 0; i < thresholds.Count - 1; i++)
        {
            if (thresholds[i + 1] <= thresholds[i])
            {
                thresholds[i + 1] = thresholds[i] + 1;
            }
        }
        
    }
}

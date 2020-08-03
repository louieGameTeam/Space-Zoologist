using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWarning : MonoBehaviour
{
    [SerializeField] Text AccessibleAreaWarning = default;
    [SerializeField] PopulationManager PopulationManager = default;

    public void Update()
    {
        this.UpdateAccessibleAreaWarning();
    }

    public void UpdateAccessibleAreaWarning()
    {
        this.AccessibleAreaWarning.text = "";
        foreach(Population population in this.PopulationManager.Populations)
        {
            if (population.IssueWithAccessibleArea)
            {
                this.AccessibleAreaWarning.text += population + " population does not have enough tiles around it. Population rapidly decreasing!\n";
            }
        }
    }
}

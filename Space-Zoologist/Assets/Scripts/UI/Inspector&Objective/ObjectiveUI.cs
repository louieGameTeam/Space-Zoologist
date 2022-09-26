using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the objective panel")]
    private GameObject objectivePanel = null;

    [SerializeField]
    [Tooltip("Objective display Cell Prefab")]
    private GameObject objectiveDisplayCell;
    [SerializeField]
    [Tooltip("Objective cell container")]
    private GameObject objectiveCellContainer = null;

    private Dictionary<Objective, GameObject> objectives = new Dictionary<Objective, GameObject>();
    public void SetupObjectives(IEnumerable<Objective> objectivesToDisplay)
    {
        // clear out old cells
        foreach(var pair in objectives)
        {
            Destroy(pair.Value);
        }
        // generate new cells
        foreach(var objective in objectivesToDisplay)
        {
            var newCell = Instantiate(objectiveDisplayCell, objectiveCellContainer.transform);
            objectives.Add(objective, newCell);
        }
    }

    public void UpdateObjectiveUI()
    {
        foreach(var pair in objectives)
        {
            // Should set this through inspector in a script, simple enough to use this for now
            pair.Value.GetComponentInChildren<TextMeshProUGUI>().text = pair.Key.GetObjectiveText();
            var bar = pair.Value.GetComponentInChildren<HorizontalProgressBar>();
            bar.MaxValue = pair.Key.GetTargetValue();
            bar.CurrentValue = pair.Key.GetCurrentValue();
        }
    }

    public void TurnObjectivePanelOn()
    {
        AudioManager.instance.PlayOneShot(SFXType.General);
        objectivePanel.SetActive(true);
    }

    public void TurnObjectivePanelOff()
    {
        objectivePanel.SetActive(false);
    }
}

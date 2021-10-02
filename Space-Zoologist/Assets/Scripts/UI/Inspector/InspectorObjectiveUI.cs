using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorObjectiveUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Root game object of the full inspector/object UI")]
    private GameObject root;
    [SerializeField]
    [Tooltip("Toggle that turns the objective panel on")]
    private Toggle objectiveToggle;
    [SerializeField]
    [Tooltip("Reference to the objective panel")]
    private GameObject objectivePanel;
    [SerializeField]
    [Tooltip("Toggle that turns the inspector panel on")]
    private Toggle inspectorToggle;
    [SerializeField]
    [Tooltip("Reference to the main inspector script")]
    private Inspector inspector;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Initialize the inspector
        inspector.Initialize();

        // Add listeners to the toggle events
        objectiveToggle.onValueChanged.AddListener(x =>
        {
            if(x) GameManager.Instance.TurnObjectivePanelOn();
            else GameManager.Instance.TurnObjectivePanelOff();
        });
        inspectorToggle.onValueChanged.AddListener(x =>
        {
            if (x) inspector.OpenInspector();
            else inspector.CloseInspector();
        });

        // Close inspector at the start
        inspector.CloseInspector();
    }
    #endregion

    #region Public Methods
    public void SetIsOpen(bool isOpen)
    {
        root.SetActive(isOpen);
    }
    #endregion
}

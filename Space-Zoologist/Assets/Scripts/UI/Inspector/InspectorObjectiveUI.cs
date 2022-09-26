using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorObjectiveUI : MonoBehaviour
{
    #region Public Properties
    public Toggle InspectorToggle => inspectorToggle;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Root game object of the full inspector/object UI")]
    private GameObject root = null;
    [SerializeField]
    [Tooltip("Toggle that turns the objective panel on")]
    private Toggle objectiveToggle = null;
    [SerializeField]
    [Tooltip("Reference to the objective panel")]
    private GameObject objectivePanel = null;
    [SerializeField]
    [Tooltip("Toggle that turns the inspector panel on")]
    private Toggle inspectorToggle = null;
    [SerializeField]
    [Tooltip("Reference to the main inspector script")]
    private Inspector inspector = null;
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

        // Enable the toggle and invoke the event
        objectiveToggle.isOn = true;
        inspectorToggle.onValueChanged.Invoke(true);
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

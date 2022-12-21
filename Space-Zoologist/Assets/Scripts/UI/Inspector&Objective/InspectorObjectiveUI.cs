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
    [Tooltip("Reference to the Objective UI Script")]
    private ObjectiveUI objectiveUI = null;
    [SerializeField]
    [Tooltip("Toggle that turns the inspector panel on")]
    private Toggle inspectorToggle = null;
    [SerializeField]
    [Tooltip("Reference to the main inspector script")]
    private Inspector inspector = null;

    [Header("Minimize/Maximize Fields")]
    [SerializeField]
    [Tooltip("Button that minimizes the panel")]
    private Button minimizeButton;
    [SerializeField]
    [Tooltip("Button that maximizes the panel")]
    private Button maximizeButton;
    [SerializeField]
    [Tooltip("Main Panel/Window, excluding maximize buttons")]
    private CanvasGroup mainDisplayWindowCanvasGroup;
    #endregion

    private bool wasInInspector = false;

    #region Monobehaviour Messages
    private void Start()
    {
        // Initialize the inspector
        inspector.Initialize();
        // Add listeners to the toggle events
        objectiveToggle.onValueChanged.AddListener(x =>
        {
            if(x) objectiveUI.TurnObjectivePanelOn();
            else objectiveUI.TurnObjectivePanelOff();
        });
        inspectorToggle.onValueChanged.AddListener(x =>
        {
            if (x) inspector.OpenInspector();
            else inspector.CloseInspector();
        });
        maximizeButton.onClick.AddListener(ShowInspectorObjectiveWindow);
        minimizeButton.onClick.AddListener(HideInspectorObjectiveWindow);
        ShowInspectorObjectiveWindow();
        // Enable the toggle and invoke the event
        objectiveToggle.isOn = true;
        inspectorToggle.onValueChanged.Invoke(true);
        inspector.CloseInspector();

    }
    #endregion

    private void HideInspectorObjectiveWindow()
    {
        mainDisplayWindowCanvasGroup.alpha = 0f;
        mainDisplayWindowCanvasGroup.blocksRaycasts = false;
        maximizeButton.gameObject.SetActive(true);
        if (inspector.IsInInspectorMode)
        {
            wasInInspector = true;
            inspector.CloseInspector();
        }
        AudioManager.instance.PlayOneShot(SFXType.General);
    }

    private void ShowInspectorObjectiveWindow()
    {
        mainDisplayWindowCanvasGroup.alpha = 1f;
        mainDisplayWindowCanvasGroup.blocksRaycasts = true;
        maximizeButton.gameObject.SetActive(false);
        if (wasInInspector)
            inspector.OpenInspector();
        wasInInspector = false;
        AudioManager.instance.PlayOneShot(SFXType.General);
    }

    #region Public Methods
    public void SetIsOpen(bool isOpen)
    {
        root.SetActive(isOpen);
    }

    public void UpdateObjectiveUI()
    {
        objectiveUI.UpdateObjectiveUI();
    }

    public void SetupObjectiveUI(IEnumerable<Objective> objectives)
    {
        objectiveUI.SetupObjectives(objectives);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using DialogueEditor;

public class ConceptsCanvasUI : NotebookUIChild
{
    #region Public Typedefs
    [System.Serializable]
    public class FoldoutAnchor
    {
        public Vector2 anchor;
        public Vector2 sizeDelta;

        public Tween Apply(RectTransform rectTransform, float time)
        {
            rectTransform.DOKill();
            rectTransform.DOAnchorMax(anchor, time);
            return rectTransform.DOSizeDelta(sizeDelta, time);
        }
    }
    [System.Serializable]
    public class CameraZoom
    {
        [Tooltip("Orthographic size of the camera when viewing the concept canvas")]
        public float orthographicSize = 21f;
        [Tooltip("Y-position that the camera moves to at the center of the drawing canvas")]
        public float yCenter = 13f;
        [Tooltip("X-position that the camera moves to when viewing the leftmost part of the enclosure")]
        public float xLeft = 0f;

        // Apply this camera zoom to the main camera
        public void Apply(Vector2 scrollPos, float smoothingTime)
        {
            Vector3 pos = new Vector3(xLeft, yCenter, GameManager.Instance.m_cameraController.transform.position.z);
            GameManager.Instance.m_cameraController.Lock(new CameraPositionLock(pos, orthographicSize, smoothingTime));
        }
    }
    #endregion

    #region Public Properties
    public Toggle FoldoutToggle => foldoutToggle;
    public bool IsExpanded => foldoutToggle.isOn;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform that expands and contracts when the canvas folds in/out")]
    private RectTransform foldoutRect = null;
    [SerializeField]
    [Tooltip("Toggle that expands/collapses the concept canvas")]
    private Toggle foldoutToggle = null;
    [SerializeField]
    [Tooltip("Time it takes for the canvas to expand/collapse")]
    private float foldoutTime = 0.3f;
    [SerializeField]
    [Tooltip("Anchors of the rect transform when the canvas is folded out")]
    private FoldoutAnchor foldoutAnchors = null;
    [SerializeField]
    [Tooltip("Anchors of the rect transform when the canvas is folded in")]
    private FoldoutAnchor foldinAnchors = null;
    
    [Space]

    [SerializeField]
    [Tooltip("Reference to the game object at the root of the drawing canvas to enable/disable on foldout")]
    private GameObject drawingCanvasParent = null;
    [SerializeField]
    [Tooltip("Rect transform attached to the drawing canvas")]
    private RectTransform drawingCanvasRect = null;
    [SerializeField]
    [Tooltip("Reference to the script that handles drawing on the canvas")]
    private DrawingCanvas drawingCanvas = null;
    [SerializeField]
    [Tooltip("Script that is used to select a drawing mode for the canvas")]
    private DrawingCanvasModeGroupPicker modePicker = null;
    [SerializeField]
    [Tooltip("Picker group used to select the color of the drawing canvas")]
    private ColorToggleGroupPicker colorPicker = null;
    [SerializeField]
    [Tooltip("Script used to select a stroke weight for the canvas")]
    private StrokeWeightGroupPicker strokeWeightPicker = null;
    [SerializeField]
    [Tooltip("Button used to clear the canvas")]
    private Button clearButton = null;

    [Space]

    [SerializeField]
    [Tooltip("Reference to the scroll rect that moves the drafting area side to side")]
    private ScrollRect scroll = null;
    [SerializeField]
    [Tooltip("Time it takes for the camera to move into zoomed position")]
    private float smoothingTime = 1f;
    [SerializeField]
    [Tooltip("Zoom applied to the camera while folded out and dialogue is not active")]
    private CameraZoom dialogueInactiveZoom = null;
    [SerializeField]
    [Tooltip("Zoom applied to the camera while folded out and dialogue is active")]
    private CameraZoom dialogueActiveZoom = null;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Modify camera position when conversation is started or ended
        // We need to pass the true-false value because apparently the "IsConversationActive"
        // bool on the conversation manager is not set until AFTER these events are invoked
        // (We really should fix that...)

        // ugly fix for linking issue
        ConversationManager.OnConversationStarted += SetCameraPositionWithDialogue;
        ConversationManager.OnConversationEnded += SetCameraPositionWithoutDialogue;
        
        // Apply foldout state to the anchors when we start
        ApplyFoldoutState(foldoutToggle.isOn);
    }
    private void OnEnable()
    {
        if(foldoutToggle.isOn)
        {
            SetCameraPosition();
        }
    }
    private void OnDisable()
    {
        GameManager instance = GameManager.Instance;

        if(instance)
        {
            instance.m_cameraController.Unlock();
        }
    }

    private void OnDestroy()
    {
        // causing issues in instantiation
        ConversationManager.OnConversationStarted -= SetCameraPositionWithDialogue;
        ConversationManager.OnConversationEnded -= SetCameraPositionWithoutDialogue;
    }
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Apply foldout state when toggle state changes
        foldoutToggle.isOn = false;
        foldoutToggle.onValueChanged.AddListener(ApplyFoldoutState);
        ApplyFoldoutState(false);

        // Set the object picked on each picker to whatever the canvas's current setting is
        modePicker.SetValuePicked(drawingCanvas.CurrentMode);
        strokeWeightPicker.SetValuePicked(drawingCanvas.CurrentWeight);

        // Set the color to the first in the list
        colorPicker.SetTogglePicked(0);
        drawingCanvas.CurrentColor = colorPicker.FirstValuePicked;

        // Add listeners for groups that change the canvas parameters
        modePicker.OnToggleStateChanged.AddListener(() => drawingCanvas.CurrentMode = modePicker.FirstValuePicked);
        colorPicker.OnToggleStateChanged.AddListener(() => drawingCanvas.CurrentColor = colorPicker.FirstValuePicked);
        strokeWeightPicker.OnToggleStateChanged.AddListener(() => drawingCanvas.CurrentWeight = strokeWeightPicker.FirstValuePicked);

        // Clear canvas when clear button clicked
        clearButton.onClick.AddListener(drawingCanvas.Clear);
    }
    #endregion

    #region Private Methods
    private void ApplyFoldoutState(bool state)
    {
        // Change the anchor to either the far right of the parent or the middle of the parent
        if (state)
        {
            foldoutAnchors.Apply(foldoutRect, foldoutTime).OnComplete(() => drawingCanvasParent.gameObject.SetActive(true));

            // Try to get the game manager
            GameManager instance = GameManager.Instance;

            // If we get the game manager then set the camera position
            if(instance)
            {
                scroll.normalizedPosition = Vector2.zero;
                SetCameraPosition();
            }

            // hide inspector
            TryAddBlockInspectorSet();
        }
        else
        {
            drawingCanvasParent.gameObject.SetActive(false);
            foldinAnchors.Apply(foldoutRect, foldoutTime);

            // Unlock the camera so it goes back to its previous position
            GameManager instance = GameManager.Instance;
            if (instance) instance.m_cameraController.Unlock();
            TryRemoveBlockInspectorSet();
        }
    }
    private void SetCameraPositionWithDialogue() => SetCameraPosition(true);
    private void SetCameraPositionWithoutDialogue() => SetCameraPosition(false);
    private void SetCameraPosition()
    {
        ConversationManager conversation = ConversationManager.Instance;

        // If conversation was found then use its state to set the camera position
        if (conversation) SetCameraPosition(conversation.IsConversationActive);
        // If no conversation was found set camera for no dialogue present
        else SetCameraPosition(false);
    }
    private void SetCameraPosition(bool conversationActive)
    {
        // Only set the camera position
        // if the game object is active in the heirarchy
        if (gameObject.activeInHierarchy)
        {
            if (foldoutToggle.isOn)
            {
                if (conversationActive)
                {
                    dialogueActiveZoom.Apply(scroll.normalizedPosition, smoothingTime);
                }
                else dialogueInactiveZoom.Apply(scroll.normalizedPosition, smoothingTime);
            }
        }
    }
    #endregion
}

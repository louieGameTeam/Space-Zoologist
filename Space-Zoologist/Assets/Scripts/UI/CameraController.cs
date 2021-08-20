  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private const float cameraAccel = 0.75f;
    private const float deadzone = 0.01f;

    [SerializeField] float WASDSpeed = 0.5f;
    [SerializeField] private float zoomLerpSpeed = 5f;
    [SerializeField] bool EdgeMovement = false;
    [SerializeField] private float edgeSpeed = 5f;
    [SerializeField] private float edgeBoundary = 10f;
    [SerializeField] private float zoomHeight = 10f;
    [SerializeField] private int MapWidth = default;
    [SerializeField] private int MapHeight = default;   

    private Camera cam = default;
    private float targetZoom;
    private float zoomFactor = 3f;
    private Vector3 dragOrigin;
    private Vector3 oldPos;
    private Vector2 currentVelocity;
    // True if the mouse is over the world space and not some other UI element
    private bool hasMouse = true;
    // True when the camera is being dragged using the center mouse button
    private bool hasMouseDrag = false;

    void Start()
    {
        cam = this.GetComponent<Camera>();
        targetZoom = cam.orthographicSize;

        Canvas canvas = FindObjectOfType<Canvas>();

        if(canvas)
        {
            RectTransform canvasTransform = canvas.GetComponent<RectTransform>();

            // Setup an event trigger entry that resets "hasMouse" to true when the pointer enters
            UnityEngine.EventSystems.EventTrigger.Entry pointerEnterTrigger = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter,
                callback = new UnityEngine.EventSystems.EventTrigger.TriggerEvent()
            };
            pointerEnterTrigger.callback.AddListener(eventData => hasMouse = true);

            // Setup an event trigger entry that sets "hasMouse" to false when the pointer exits
            UnityEngine.EventSystems.EventTrigger.Entry pointerExitTrigger = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit,
                callback = new UnityEngine.EventSystems.EventTrigger.TriggerEvent()
            };
            pointerExitTrigger.callback.AddListener(eventData => hasMouse = false);

            // Create a new object with a rect transform
            GameObject mouseCatcherObject = new GameObject("Background");
            RectTransform mouseCatcherTransform = mouseCatcherObject.AddComponent<RectTransform>();

            // Make this a child of the canvas
            mouseCatcherTransform.SetParent(canvas.transform);
            mouseCatcherTransform.SetAsFirstSibling();

            // Stretch the object so it fills the canvas
            mouseCatcherTransform.anchorMin = Vector2.zero;
            mouseCatcherTransform.anchorMax = Vector2.one;
            mouseCatcherTransform.offsetMin = mouseCatcherTransform.offsetMax = Vector2.zero;

            // Add a clear image to the object so that the event system still receives pointer events for it
            mouseCatcherObject.AddComponent<CanvasRenderer>();
            Image mouseCatcherImage = mouseCatcherObject.AddComponent<Image>();
            mouseCatcherImage.color = Color.clear;

            // Add the entries to the list of triggers on the mouse catcher event trigger
            UnityEngine.EventSystems.EventTrigger mouseCatcher = mouseCatcherObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            mouseCatcher.triggers.Add(pointerEnterTrigger);
            mouseCatcher.triggers.Add(pointerExitTrigger);
        }
        else
        {
            Debug.LogWarning("CameraController: no active canvas component found in the scene");
            hasMouse = true;
        }

        
    }

    void Update()
    {
        this.HandleKeyboard();
        this.HandleMouse();
        this.HandleZoom();
        if (this.EdgeMovement) this.HandleEdgeScreen();
    }

    private void HandleZoom()
    {
        //if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5)
        //{
        //    //Debug.Log("Not zooming");
        //    return;
        //}

        // Only receive input if we have the mouse
        float scrollData = 0;
        if (hasMouse) scrollData = Input.GetAxis("Mouse ScrollWheel");

        targetZoom -= scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 2.5f, zoomHeight);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
    }

    private void HandleKeyboard()
    {
        float xValue = currentVelocity.x;
        float yValue = currentVelocity.y;
        float zoomScalar = targetZoom/zoomHeight; //Scale the camera's speed by the zoom amount

        float accleration = cameraAccel * zoomScalar * Time.deltaTime;
        float maxSpeed = WASDSpeed * zoomScalar;

        if (Input.GetKey(KeyCode.A) && hasMouse) //If pressing left and not yet reached maximum negative horizontal speed, decrease by acceleration
        {
            if(xValue > -maxSpeed)
                xValue -= accleration;
        }
        else if (Input.GetKey(KeyCode.D) && hasMouse) //If pressing right and not yet reached maximum horizontal speed, increase by acceleration
        {
            if(xValue < maxSpeed)
                xValue += accleration;
        }
        else if(Mathf.Abs(xValue) <= deadzone) //If within deadzone, set velocity to 0
        {
            xValue = 0;
        }
        else //Otherwise, exert drag on the camera
        {
            xValue -= accleration * Mathf.Sign(xValue);
        }

        if (Input.GetKey(KeyCode.S) && hasMouse) //If pressing down and not yet reached maximum negative vertical speed, decrease by acceleration
        {
            if(yValue > -maxSpeed)
                yValue -= accleration;
        }
        else if (Input.GetKey(KeyCode.W) && hasMouse) //If pressing up and not yet reached maximum vertical speed, increase by acceleration
        {
            if(yValue < maxSpeed)
                yValue += accleration;
        }
        else if(Mathf.Abs(yValue) <= deadzone) //If within deadzone, set velocity to 0
        {
            yValue = 0;
        }
        else //Otherwise, exert drag on the camera
        {
            yValue -= accleration * Mathf.Sign(yValue);
        }

        Vector3 attemptedPosition = new Vector3(transform.position.x + xValue, transform.position.y + yValue, -10);

        if(attemptedPosition.x < 0 || attemptedPosition.x > MapWidth)
            xValue = 0;

        if(attemptedPosition.y < 0 || attemptedPosition.y > MapHeight)
            yValue = 0;

        this.transform.position = new Vector3(transform.position.x + xValue, transform.position.y + yValue, -10);

        currentVelocity = new Vector2(xValue, yValue);
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(2) && hasMouse)
        {
            dragOrigin = Input.mousePosition;
            oldPos = this.transform.position;
            hasMouseDrag = true;
        }

        if (!Input.GetMouseButton(2)) hasMouseDrag = false;

        if(hasMouseDrag)
        {
            Vector3 displacement = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(dragOrigin);
            Vector3 newPosition = new Vector3(oldPos.x + displacement.x * -1, oldPos.y + displacement.y * -1, -10);
            if (!this.IsValidLocation(newPosition))
            {
                newPosition = ClampToValidLocation(newPosition);
            }
            this.transform.position = newPosition;
        }
    }

    private void HandleEdgeScreen()
    {
        float x = 0;
        float y = 0;
        if (Input.mousePosition.x > Screen.width - edgeBoundary)
        {
            x += this.edgeSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x < 0 + edgeBoundary)
        {
            x -= this.edgeSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y > Screen.height - edgeBoundary)
        {
            y += this.edgeSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y < 0 + edgeBoundary)
        {
            y -= this.edgeSpeed * Time.deltaTime;
        }
        Vector3 newPosition = new Vector3(this.transform.position.x + x, this.transform.position.y + y, this.transform.position.z);
        if (!this.IsValidLocation(newPosition))
        {
            return;
        }
        this.transform.position = newPosition;
    }

    private bool IsValidLocation(Vector3 newPosition)
    {
        return (newPosition.x >= 0 && newPosition.y >= 0
        && newPosition.x <= MapWidth && newPosition.y <= MapHeight);
    }

    private Vector3 ClampToValidLocation(Vector3 toClamp) {
        float x = Mathf.Clamp(toClamp.x, 0, MapWidth);
        float y = Mathf.Clamp(toClamp.y, 0, MapHeight);
        return new Vector3(x, y, toClamp.z);
    }
}

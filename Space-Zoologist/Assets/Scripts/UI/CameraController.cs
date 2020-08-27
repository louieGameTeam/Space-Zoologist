using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float WASDSpeed = 0.5f;
    [SerializeField] float dragSpeed = 2;
    [SerializeField] private float zoomLerpSpeed = 5f;
    [SerializeField] bool EdgeMovement = false;
    [SerializeField] private float edgeSpeed = 5f;
    [SerializeField] private float edgeBoundary = 10f;
    [SerializeField] private LevelDataReference LevelDataReference = default;

    private Camera cam = default;
    private float targetZoom;
    private float zoomFactor = 3f;
    private Vector3 dragOrigin;

    void Start()
    {
        cam = this.GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
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
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 2.5f, 10f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
    }

    private void HandleKeyboard()
    {
        float yValue = 0.0f;
        float xValue = 0.0f;
        if (Input.GetKey(KeyCode.W))
        {
            yValue = WASDSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            yValue = -WASDSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xValue = -WASDSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            xValue = WASDSpeed;
        }
        Vector3 newPosition = new Vector3(transform.position.x + xValue, transform.position.y + yValue, -10);
        if (!this.IsValidLocation(newPosition))
        {
            return;
        }
        this.transform.position = newPosition;
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 newPosition = new Vector3(this.transform.position.x + pos.x * dragSpeed * -1, this.transform.position.y + pos.y * dragSpeed * -1, -10);
        if (!this.IsValidLocation(newPosition))
        {
            return;
        }
        this.transform.position = newPosition;
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
        && newPosition.x <= LevelDataReference.MapWidth && newPosition.y <= LevelDataReference.MapHeight);
    }
}

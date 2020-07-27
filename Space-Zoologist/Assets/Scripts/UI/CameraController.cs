using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float WASDSpeed = 0.5f;
    [SerializeField] float dragSpeed = 2;
    [SerializeField] private float zoomLerpSpeed = 5f;

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
        if (newPosition.x < TilemapUtil.ins.largestMap.origin.x || newPosition.y < TilemapUtil.ins.largestMap.origin.y
        || newPosition.x > TilemapUtil.ins.largestMap.origin.x + TilemapUtil.ins.largestMap.size.x || newPosition.y > TilemapUtil.ins.largestMap.origin.y + TilemapUtil.ins.largestMap.size.y)
        {
            return;
        }
        this.transform.position = newPosition;
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 newPosition = new Vector3(this.transform.position.x + pos.x * dragSpeed * -1, this.transform.position.y + pos.y * dragSpeed * -1, -10);
        if (newPosition.x < TilemapUtil.ins.largestMap.origin.x || newPosition.y < TilemapUtil.ins.largestMap.origin.y
        || newPosition.x > TilemapUtil.ins.largestMap.origin.x + TilemapUtil.ins.largestMap.size.x || newPosition.y > TilemapUtil.ins.largestMap.origin.y + TilemapUtil.ins.largestMap.size.y)
        {
            return;
        }

        this.transform.position = newPosition;
    }
}

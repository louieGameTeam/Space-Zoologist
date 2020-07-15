using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float Speed = 0.5f;
    private Camera cam = default;
    private float targetZoom;
    private float zoomFactor = 3f;
    [SerializeField] private float zoomLerpSpeed = 5f;

    void Start()
    {
        cam = this.GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        float yValue = 0.0f;
        float xValue = 0.0f;
        if (Input.GetKey(KeyCode.W))
        {
            yValue = Speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            yValue = -Speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xValue = -Speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            xValue = Speed;
        }
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 2.5f, 10f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
        Vector3 newPosition = new Vector3(transform.position.x + xValue, transform.position.y + yValue, -10);
        if (newPosition.x < TilemapUtil.ins.largestMap.origin.x || newPosition.y < TilemapUtil.ins.largestMap.origin.y
        || newPosition.x > TilemapUtil.ins.largestMap.origin.x + TilemapUtil.ins.largestMap.size.x || newPosition.y > TilemapUtil.ins.largestMap.origin.y + TilemapUtil.ins.largestMap.size.y)
        {
            return;
        }
        this.transform.position = newPosition;
    }
}

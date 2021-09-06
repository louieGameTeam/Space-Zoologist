using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UILine : MonoBehaviour
{
    #region Public Properties
    public RectTransform RectTransform => GetComponent<RectTransform>();
    public float Length => (end - start).magnitude + thickness;
    public Vector3 Start
    {
        get => start;
        set
        {
            start = value;
            Redraw();
        }
    }
    public Vector3 End
    {
        get => end;
        set
        {
            end = value;
            Redraw();
        }
    }
    public float Thickness
    {
        get => thickness;
        set
        {
            thickness = value;
            Redraw();
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Start position of the line")]
    private Vector3 start;
    [SerializeField]
    [Tooltip("End position of the line")]
    private Vector3 end;
    [SerializeField]
    [Tooltip("Thickness of the line")]
    private float thickness;
    #endregion

    #region Monobehaviour Messages
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(start, 10);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(end, 10);
    }
    #endregion

    #region Public Methods
    public void SetPoints(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
        Redraw();
    }
    public void Redraw()
    {
        // Anchor and pivot in the center
        RectTransform.anchorMin = RectTransform.anchorMax = RectTransform.pivot = Vector2.one * 0.5f;

        // Compute some values we need 
        Vector3 diff = end - start;
        Vector3 midpoint = Vector3.Lerp(start, end, 0.5f);

        // Compute the angle
        float angle = 0f;
        if(diff.magnitude > 0.001f) angle = Mathf.Atan(diff.y / diff.x) * Mathf.Rad2Deg;

        // Set size delta (is width and height since center is anchor)
        RectTransform.position = midpoint;
        RectTransform.sizeDelta = new Vector2(Length, thickness);
        RectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
    #endregion
}

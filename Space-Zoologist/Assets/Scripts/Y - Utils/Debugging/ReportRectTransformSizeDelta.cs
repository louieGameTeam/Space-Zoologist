using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportRectTransformSizeDelta : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform to report the size delta on")]
    private RectTransform rectTransform = null;
    #endregion

    #region Public Methods
    public void Report()
    {
        if (rectTransform)
        {
            Debug.Log($"Report from '{ToString()}' says -> '{rectTransform}' size delta: {rectTransform.sizeDelta}", rectTransform);
        }
        else Debug.Log($"Report from '{ToString()}' says -> <no rect transform to report the size delta for>", this);
    }
    #endregion
}

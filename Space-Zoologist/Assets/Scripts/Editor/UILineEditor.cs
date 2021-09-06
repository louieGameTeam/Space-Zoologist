using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UILine)), CanEditMultipleObjects]
public class UILineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target as the script itself
        UILine targetScript = (UILine)target;
        // Create a tracker to drive the aspects of the rect transform
        DrivenRectTransformTracker tracker = new DrivenRectTransformTracker();
        tracker.Add(target, targetScript.RectTransform,
            DrivenTransformProperties.AnchoredPosition |
            DrivenTransformProperties.SizeDelta |
            DrivenTransformProperties.Anchors |
            DrivenTransformProperties.Pivot |
            DrivenTransformProperties.Rotation
        );

        // Draw default inspector and check for changes
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        // Redraw the line if something changed
        if(EditorGUI.EndChangeCheck())
        {
            targetScript.Redraw();
        }
    }
}

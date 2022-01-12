using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIAuto
{
    #region Button
    public static bool Button(ref Rect position, string label)
    {
        position.height = EditorGUIAuto.SingleControlHeight;
        bool result = GUI.Button(position, label);
        position.y += position.height;
        return result;
    }
    #endregion
}

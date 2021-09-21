using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemRegistry))]
public class ItemRegistryEditor : Editor
{
    #region Private Fields
    private ArrayOnEnumEditor<ItemRegistry.Category> editor = new ArrayOnEnumEditor<ItemRegistry.Category>();
    #endregion

    #region Public Methods
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        editor.OnInspectorGUI(serializedObject.FindProperty("itemDatas"));        
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}

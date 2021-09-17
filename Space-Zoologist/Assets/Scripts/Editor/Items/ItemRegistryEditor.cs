using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemRegistry))]
public class ItemRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditArrayOnEnum.OnInspectorGUI<ItemRegistry.Category>(serializedObject.FindProperty("itemDatas"));
        serializedObject.ApplyModifiedProperties();
    }
}

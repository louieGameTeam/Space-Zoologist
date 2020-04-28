//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

/*
 * TODO: Using [Expandable] gui for now, can use this if more gui customiziation needed (a little fragile though)
 * Logic to show the ranges of conditions for each need.
 * This could be altered to provide sliders, buttons, or other cool additions to
 * the editor window based on the object.
 *
 * Need and NeedEditor are very coupled right now because of how directly they interact with each other.
 * Unsure/unclear how to better structure this interaction right now.
 */

//[CustomEditor(typeof(Need))]
//public class NeedsEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        // Setup connection to need and grab values for editor display
//        // Initialize if needed or update when the needConditionsValue.Count changes.
//        Need need = (Need)target;
//        List<float> needConditionsValues = need.GetNeedRangeFloatValues();
//        if (need.needConditions.Count == 0 || need.needConditions.Count != needConditionsValues.Count - 1)
//        {
//            SetupNeedConditions(need.needConditions, needConditionsValues.Count);
//        }
//        DisplayLabelsAndPopups(need, needConditionsValues);

//    }

//    // needConditions size should be one less than floatListSize to display properly
//    private void SetupNeedConditions(List<Need.NeedCondition> needConditions, int floatListSize)
//    {
//        while (floatListSize - 1 > needConditions.Count)
//        {
//            needConditions.Add(Need.NeedCondition.Neutral);
//        }
//        while(floatListSize - 1 < needConditions.Count)
//        {
//            needConditions.RemoveAt(needConditions.Count - 1);
//        }
//    }

//    // Annoying logic for side by side popup and labels. Also need the popup to change the values in Need.
//    private void DisplayLabelsAndPopups(Need need, List<float> needConditionsValues)
//    {
//        for (int i = 0; i < need.needConditions.Count - 1; i++)
//        {
//            EditorGUILayout.BeginHorizontal();
//            need.needConditions[i] = (Need.NeedCondition)EditorGUILayout.EnumPopup(need.needConditions[i]);
//            GUILayout.Label(needConditionsValues[i].ToString() + " ~< " + needConditionsValues[i + 1].ToString());
//            EditorGUILayout.EndHorizontal();
//        }
//        EditorGUILayout.BeginHorizontal();
//        need.needConditions[need.needConditions.Count - 1] =
//            (Need.NeedCondition)EditorGUILayout.EnumPopup(need.needConditions[need.needConditions.Count - 1]);
//        GUILayout.Label(needConditionsValues[needConditionsValues.Count - 2].ToString() + " ~< " +
//            needConditionsValues[needConditionsValues.Count - 1].ToString());
//        EditorGUILayout.EndHorizontal();
//    }

//    // Saving the changes made in our custom dropdown list requires marking the Editor as dirty so it saves the changes.
//    private void OnDisable()
//    {
//        #if UNITY_EDITOR
//        UnityEditor.EditorUtility.SetDirty(target);
//        #endif
//    }
//}

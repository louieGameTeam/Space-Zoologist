using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(QuizGradingRubric))]
public class QuizGradingRubricDrawer : PropertyDrawer
{
    #region Private Fields
    private ParallelArrayEditor<string> editor = new ParallelArrayEditor<string>();
    #endregion

    #region Public Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty percentages = property.FindPropertyRelative(nameof(percentages));

        // Edit percentages and check for changes
        EditorGUI.BeginChangeCheck();
        editor.OnGUI(position, percentages, label, GetRubricLabels());

        // If the array changed then sort the percentages
        if(EditorGUI.EndChangeCheck())
        {
            SortPercentages(percentages);
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty percentages = property.FindPropertyRelative(nameof(percentages));
        return editor.GetPropertyHeight(percentages);
    }
    #endregion

    #region Private Methods
    private string[] GetRubricLabels()
    {
        string[] grades = System.Enum.GetNames(typeof(QuizGradeType));
        string[] labels = new string[grades.Length - 1];

        // Setup each label to imply a grade switch
        for(int i = 0; i < labels.Length; i++)
        {
            labels[i] = grades[i] + " -> " + grades[i + 1];
        }

        return labels;
    }
    private void SortPercentages(SerializedProperty percentages)
    {
        // Load the property values into an array of floats
        float[] percentageArray = new float[percentages.arraySize];
        for(int i = 0; i < percentageArray.Length; i++)
        {
            percentageArray[i] = percentages.GetArrayElementAtIndex(i).floatValue;
        }

        // Sort the percentage array
        System.Array.Sort(percentageArray);

        // Put the percentage array back into the serialized property
        for (int i = 0; i < percentageArray.Length; i++)
        {
            percentages.GetArrayElementAtIndex(i).floatValue = percentageArray[i];
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResearchEncyclopediaConfig))]
public class ResearchEncyclopediaConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty articles = serializedObject.FindProperty(nameof(articles));

        // Set out the size property
        EditorGUILayout.PropertyField(articles.FindPropertyRelative("Array.size"));
        for(int i = 0; i < articles.arraySize; i++)
        {
            SerializedProperty article = articles.GetArrayElementAtIndex(i);
            // Get the article title
            string articleTitle = article.FindPropertyRelative("id").FindPropertyRelative("title").stringValue;
            if (articleTitle == "") articleTitle = "Element " + i;
            // Set out the article with the title as the gui content label
            EditorGUILayout.PropertyField(article, new GUIContent(articleTitle), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

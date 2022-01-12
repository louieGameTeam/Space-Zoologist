using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(QuizInstance))]
public class QuizInstanceDrawer : PropertyDrawer
{
    #region Private Fields
    private QuizRuntimeTemplate runtimeTemplate;
    private ParallelArrayEditor<QuizQuestion> quizAnswersEditor = new ParallelArrayEditor<QuizQuestion>()
    {
        arrayElementLabel = (s, e) => new GUIContent(e.Question),
        arrayElementPropertyField = (r, s, g, e) =>
        {
            // Create a foldout for this element
            s.isExpanded = EditorGUIAuto.Foldout(ref r, s.isExpanded, g);

            // Layout each answer if this is expanded
            if(s.isExpanded)
            {
                // Increase indent
                EditorGUI.indentLevel++;

                for(int i = 0; i < e.Options.Length; i++)
                {
                    // Store current option
                    QuizOption option = e.Options[i];
                    // Set out a toggle for this option
                    bool toggled = EditorGUI.ToggleLeft(r, option.DisplayName, i == s.intValue);

                    // If this toggled is toggled to on then set the int value of the property
                    if (toggled) s.intValue = i;

                    // Advance the position once down
                    r.y += r.height;
                }

                // Restore indent
                EditorGUI.indentLevel--;
            }
        },
        arrayElementPropertyHeight = (s, e) =>
        {
            float height = EditorExtensions.StandardControlHeight;

            // If property is expanded add a drawer height for each option
            if(s.isExpanded)
            {
                height += EditorExtensions.StandardControlHeight * e.Options.Length;
            }

            return height;
        }
    };
    // Foldout value of the information on the quiz instance
    private bool resultsFoldout = false;
    #endregion

    #region Public Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the sub-properties
        SerializedProperty template = property.FindPropertyRelative(nameof(template));
        SerializedProperty answers = property.FindPropertyRelative(nameof(answers));

        // Set out the foldout
        property.isExpanded = EditorGUIAuto.Foldout(ref position, property.isExpanded, label);

        if (property.isExpanded)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // Layout the template
            EditorGUIAuto.PropertyField(ref position, template);

            // If an object reference exists then layout the answers
            if(template.objectReferenceValue)
            {
                // Get the quiz template
                QuizTemplate quizTemplate = template.objectReferenceValue as QuizTemplate;
                CheckRuntimeTemplate(quizTemplate);

                // Use a button to regenerate quiz questions from the template
                GUI.enabled = quizTemplate.Dynamic;
                if (GUIAuto.Button(ref position, "Generate new questions"))
                {
                    runtimeTemplate = new QuizRuntimeTemplate(quizTemplate);
                }
                GUI.enabled = true;

                quizAnswersEditor.OnGUI(position, answers, runtimeTemplate.Questions);
                position.y += quizAnswersEditor.GetPropertyHeight(answers, runtimeTemplate.Questions);

                // Get a list of all the answers as integers
                int[] answersArray = Answers(property);
                resultsFoldout = EditorGUIAuto.Foldout(ref position, resultsFoldout, "Quiz Results");

                if (resultsFoldout)
                {
                    // Indent the next part and disable it
                    EditorGUI.indentLevel++;
                    GUI.enabled = false;

                    // Compute if the important questions passed, what the score is, and what the max score was
                    bool passed = QuizInstance.PassedImportantQuestions(runtimeTemplate, answersArray);
                    int score = QuizInstance.ComputeScoreInImportantCategories(runtimeTemplate, answersArray);
                    int maxScore = runtimeTemplate.GetMaximumPossibleScoreInImportantCategories();
                    string scoreString = $"{score} / {maxScore} - {(passed ? "Pass" : "Fail")}";

                    // Show the score on important questions
                    EditorGUIAuto.PrefixedLabelField(ref position, new GUIContent("Score on Important Questions"), scoreString);

                    // Compute if the unimportant questions passed, what the score is, and what the max score was
                    passed = QuizInstance.PassedUnimportantQuestions(runtimeTemplate, answersArray);
                    score = QuizInstance.ComputeScoreInUnimportantCategories(runtimeTemplate, answersArray);
                    maxScore = runtimeTemplate.GetMaximumPossibleScoreInUnimportantCategories();
                    scoreString = $"{score} / {maxScore} - {(passed ? "Pass" : "Fail")}";

                    // Show the score on unimportant questions
                    EditorGUIAuto.PrefixedLabelField(ref position, new GUIContent("Score on Unimportant Questions"), scoreString);

                    // Show the final grade
                    EditorGUIAuto.PrefixedLabelField(ref position, new GUIContent("Final Grade"), 
                        QuizInstance.ComputeGrade(runtimeTemplate, answersArray).ToString());

                    // Change the values back to before
                    EditorGUI.indentLevel--;
                    GUI.enabled = true;
                }
            }

            // Restore indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get the sub-properties
        SerializedProperty template = property.FindPropertyRelative(nameof(template));
        SerializedProperty answers = property.FindPropertyRelative(nameof(answers));

        // Set height for one control
        float height = EditorExtensions.StandardControlHeight;

        if(property.isExpanded)
        {
            // Add space for the template drawer
            height += EditorExtensions.StandardControlHeight;

            // If an object reference exists then add room for the answers
            if(template.objectReferenceValue)
            {
                // Add height for the regenerate button
                height += EditorExtensions.StandardControlHeight;

                QuizTemplate quizTemplate = template.objectReferenceValue as QuizTemplate;
                CheckRuntimeTemplate(quizTemplate);
                height += quizAnswersEditor.GetPropertyHeight(answers, runtimeTemplate.Questions);

                // Add height for the results foldout
                height += EditorExtensions.StandardControlHeight;

                // Add space for three more controls if the results are folded out
                if (resultsFoldout) height += EditorExtensions.StandardControlHeight * 3f;
            }
        }

        return height;
    }
    #endregion

    #region Private Methods
    private int[] Answers(SerializedProperty property)
    {
        // Get the answers array in the property and create a new array for it
        property = property.FindPropertyRelative("answers");
        int[] answers = new int[property.arraySize];

        // Store each answer into the array
        for(int i = 0; i < answers.Length; i++)
        {
            answers[i] = property.GetArrayElementAtIndex(i).intValue;
        }

        return answers;
    }
    private void CheckRuntimeTemplate(QuizTemplate template)
    {
        if (runtimeTemplate == null || runtimeTemplate.Template != template)
        {
            runtimeTemplate = new QuizRuntimeTemplate(template);
        }
    }
    #endregion
}

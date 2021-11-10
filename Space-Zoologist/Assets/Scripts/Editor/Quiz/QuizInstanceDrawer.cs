using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(QuizInstance))]
public class QuizInstanceDrawer : PropertyDrawer
{
    #region Private Fields
    private ParallelArrayEditor<QuizQuestion> quizAnswersEditor = new ParallelArrayEditor<QuizQuestion>()
    {
        arrayElementLabel = (s, e) => new GUIContent(e.Question),
        arrayElementPropertyField = (r, s, g, e) =>
        {
            // Set height for just one control
            r.height = EditorExtensions.StandardControlHeight;

            // Create a foldout for this element
            s.isExpanded = EditorGUI.Foldout(r, s.isExpanded, g);
            r.y += r.height;

            // Layout each answer if this is expanded
            if(s.isExpanded)
            {
                // Increase indent
                EditorGUI.indentLevel++;

                for(int i = 0; i < e.Options.Length; i++)
                {
                    // Store current option
                    QuizOption option = e.Options[i];
                    // Get the label of this option
                    string optionLabel = $"{option.Label} ({(option.Weight > 0 ? "+" : "")}{option.Weight})";

                    // Set out a toggle for this option
                    bool toggled = EditorGUI.ToggleLeft(r, optionLabel, i == s.intValue);

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

        // Set the height for just one control
        position.height = EditorExtensions.StandardControlHeight;

        // Set out the foldout
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        position.y += position.height;

        if (property.isExpanded)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // Layout the template
            EditorGUI.PropertyField(position, template);
            position.y += position.height;

            // If an object reference exists then layout the answers
            if(template.objectReferenceValue)
            {
                // Edit all of the quiz answers using the parallel array editor that we set up
                QuizTemplate quizTemplate = template.objectReferenceValue as QuizTemplate;
                quizAnswersEditor.OnGUI(position, answers, quizTemplate.Questions);
                position.y += quizAnswersEditor.GetPropertyHeight(answers, quizTemplate.Questions);

                // Get a list of all the answers as integers
                int[] answersArray = Answers(property);
                resultsFoldout = EditorGUI.Foldout(position, resultsFoldout, new GUIContent("Quiz Results"));

                // Advance position down to next control
                position.y += position.height;

                if (resultsFoldout)
                {
                    // Indent the next part and disable it
                    EditorGUI.indentLevel++;
                    GUI.enabled = false;

                    // Compute if the important questions passed, what the score is, and what the max score was
                    bool passed = QuizInstance.PassedImportantQuestions(quizTemplate, answersArray);
                    int score = QuizInstance.ComputeScoreInImportantCategories(quizTemplate, answersArray);
                    int maxScore = quizTemplate.GetMaximumPossibleScoreInImportantCategories();
                    string scoreString = $"{score} / {maxScore} - {(passed ? "Pass" : "Fail")}";

                    // Show the score on important questions
                    Rect prefixPos = EditorGUI.PrefixLabel(position, new GUIContent("Score on Important Questions"));
                    EditorGUI.LabelField(prefixPos, scoreString);
                    position.y += position.height;

                    // Compute if the unimportant questions passed, what the score is, and what the max score was
                    passed = QuizInstance.PassedUnimportantQuestions(quizTemplate, answersArray);
                    score = QuizInstance.ComputeScoreInUnimportantCategories(quizTemplate, answersArray);
                    maxScore = quizTemplate.GetMaximumPossibleScoreInUnimportantCategories();
                    scoreString = $"{score} / {maxScore} - {(passed ? "Pass" : "Fail")}";

                    // Show the score on unimportant questions
                    prefixPos = EditorGUI.PrefixLabel(position, new GUIContent("Score on Unimportant Questions"));
                    EditorGUI.LabelField(prefixPos, scoreString);
                    position.y += position.height;

                    // Show the final position
                    prefixPos = EditorGUI.PrefixLabel(position, new GUIContent("Final Grade"));
                    EditorGUI.LabelField(prefixPos, QuizInstance.ComputeGrade(quizTemplate, answersArray).ToString());

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
                QuizTemplate quizTemplate = template.objectReferenceValue as QuizTemplate;
                height += quizAnswersEditor.GetPropertyHeight(answers, quizTemplate.Questions);

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
    #endregion
}

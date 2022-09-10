using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizAnswerUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Text used to display the question")]
    private TextMeshProUGUI questionText = null;
    [SerializeField]
    [Tooltip("Text used to display the answer")]
    private TextMeshProUGUI answerText = null;
    [SerializeField]
    [Tooltip("Text used to tell the player how correct their answer is")]
    private TextMeshProUGUI gradeText = null;
    #endregion

    #region Public Methods
    public void DisplayAnswer(QuizInstance quiz, int questionIndex)
    {
        if (questionIndex >= 0 && questionIndex < quiz.RuntimeTemplate.Questions.Length)
        {
            // Store quiz question for easy access
            QuizQuestion question = quiz.RuntimeTemplate.Questions[questionIndex];
            QuizOption answer = quiz.GetAnswer(questionIndex);
            int answerIndex = quiz.Answers[questionIndex];

            // Setup question text
            questionText.text = $"Question {questionIndex + 1}: {question.Question}";
            
            // Setup answer text
            answerText.text = $"\t";

            // Add text for each answer, highlighting the picked answer
            for (int i = 0; i < question.Options.Length; i++) 
            {
                QuizOption option = question.Options[i];

                // Give the answer a bold white color
                if (option == answer)
                {
                    answerText.text += $"<color=white>* {option.Label}</color>";
                }
                // Use the normal label for other options
                else answerText.text += option.Label;

                // Add endline and tab after each option except the last
                if (i < question.Options.Length - 1)
                {
                    answerText.text += "\n\t";
                }
            }

            // Compute the relative grade, and give flavor text for different grade levels
            if (question.RelativeGrade(answerIndex) > 0.6f)
            {
                gradeText.text = "<color=green>Consistent with existing research!</color>";
            }
            else gradeText.text = "<color=red>Not consistent with existing research...</color>";
        }
    }
    #endregion
}

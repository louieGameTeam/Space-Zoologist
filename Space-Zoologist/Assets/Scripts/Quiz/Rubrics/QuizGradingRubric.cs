using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class representing a grading system for quizzes
/// </summary>
public abstract class QuizGradingRubric : ScriptableObject
{
    public abstract QuizGrade EvaluateGrade(int importantScore, int importantMaxScore, int unimportantScore, int unimportantMaxScore);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizConversationDebugger : MonoBehaviour
{
    public QuizConversation quizConversation;

    private void Start()
    {
        CreateConversation();
    }

    public void CreateConversation()
    {
        quizConversation.Create();
    }
}

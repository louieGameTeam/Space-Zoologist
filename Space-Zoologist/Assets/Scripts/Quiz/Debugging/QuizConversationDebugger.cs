using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class QuizConversationDebugger : MonoBehaviour
{
    public QuizConversation quizConversation;
    public ConversationManager manager;

    private void Start()
    {
        (NPCConversation conversation, QuizInstance _) = quizConversation.Create();

        // Start this conversation
        manager.Initialize();
        manager.StartConversation(conversation);
    }

    public void CreateConversation()
    {
        quizConversation.Create();
    }
}

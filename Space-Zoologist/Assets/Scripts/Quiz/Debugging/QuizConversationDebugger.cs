using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class QuizConversationDebugger : MonoBehaviour
{
    public QuizConversation quizConversation;
    public ConversationManager manager;
    public DialogueManager dialogueManager;

    private void Start()
    {
        NPCConversation conversation = quizConversation.Create(dialogueManager);

        // Start this conversation
        manager.Initialize();
        manager.StartConversation(conversation);
    }

    public void CreateConversation()
    {
        quizConversation.Create(dialogueManager);
    }
}

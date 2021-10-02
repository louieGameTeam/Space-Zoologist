using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class QuizConversationDebugger : MonoBehaviour
{
    public GameObject quizConversationRoot;
    public QuizConversationBuilder quizConversation;
    public ConversationManager manager;
    public DialogueManager dialogueManager;

    private void Start()
    {
        (NPCConversation conversation, QuizInstance _) = quizConversation.Create(quizConversationRoot, dialogueManager);

        // Start this conversation
        manager.Initialize();
        manager.StartConversation(conversation);
    }

    public void CreateConversation()
    {
        quizConversation.Create(quizConversationRoot, dialogueManager);
    }
}

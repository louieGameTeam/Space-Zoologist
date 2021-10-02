using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor;
using TMPro;

[System.Serializable]
public class QuizConversationBuilder
{
    #region Public Typedefs
    [System.Serializable]
    public class QuizResponse
    {
        public NPCConversation[] responses;
        public NPCConversation Get(QuizGradeType grade) => responses[(int)grade];
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the quiz template to run the quiz for")]
    private QuizTemplate quizTemplate;

    [Space]

    [SerializeField]
    [Tooltip("Icon to display for the npc")]
    private Sprite npcIcon;
    [SerializeField]
    [Tooltip("Name of the npc giving the quiz")]
    private string npcName = "Star";
    [SerializeField]
    [Tooltip("Font to use when saying each speech node")]
    private TMP_FontAsset npcFont;

    [Space]

    [SerializeField]
    [TextArea(3, 10)]
    [Tooltip("Speech node spoken when the quiz ends")]
    private string endOfQuizText = "Alright, give me a minute to process your answers...";
    [SerializeField]
    [Tooltip("List of NPCConversations to respond with based on the quizes' grade")]
    [EditArrayWrapperOnEnum("responses", typeof(QuizGradeType))]
    private QuizResponse response;
    #endregion

    #region Public Methods
    public (NPCConversation conversation, QuizInstance quiz) Create(GameObject gameObject, DialogueManager dialogueManager)
    {
        // Create the callback that is called after any option is answered
        UnityAction OptionSelectedFunctor(QuizInstance quizInstance, int questionIndex, int optionIndex)
        {
            return () => quizInstance.AnswerQuestion(questionIndex, optionIndex);
        }

        // Create the quiz instance
        QuizInstance quiz = new QuizInstance(quizTemplate);
        
        // Try to get an npc conversation. If it exists, destroy it and add a new one
        NPCConversation conversation = gameObject.GetComponent<NPCConversation>();
        if (conversation)
        {
#if UNITY_EDITOR 
            Object.DestroyImmediate(conversation);
#else
            Destroy(conversation);
#endif
        }
        conversation = gameObject.AddComponent<NPCConversation>();

        // Create the conversation to be edited here in the code
        EditableConversation editableConversation = new EditableConversation();
        EditableSpeechNode previousSpeechNode = null;

        // A list of all nodes added to the conversation
        List<EditableConversationNode> nodes = new List<EditableConversationNode>();

        // Loop over every question and add speech and option nodes for each
        for (int i = 0; i < quizTemplate.Questions.Length; i++)
        {
            // Cache the current question
            QuizQuestion question = quizTemplate.Questions[i];

            // Create a new speech node
            EditableSpeechNode currentSpeechNode = CreateSpeechNode(conversation, editableConversation, question.Question, 0, i * 300, i == 0, null);
            nodes.Add(currentSpeechNode);

            // If a previous speech node exists, then make the options on the previous node
            // point to the speech on the current node
            if (previousSpeechNode != null)
            {
                foreach (EditableOptionNode option in previousSpeechNode.Options)
                {
                    option.Speech.SetSpeech(currentSpeechNode);
                }
            }

            // Add an option node for each quiz option
            for (int j = 0; j < question.Options.Length; j++)
            {
                // Get the current option
                QuizOption option = question.Options[j];

                // Create a new option node with the same label as the quiz option
                EditableOptionNode optionNode = CreateOptionNode(conversation, editableConversation, option.Label, j * 220, (i * 300) + 100);
                currentSpeechNode.AddOption(optionNode);
                nodes.Add(optionNode);

                // Create a dummy node. It is used to invoke events
                UnityAction optionCallback = OptionSelectedFunctor(quiz, i, j);
                EditableSpeechNode dummyNode = CreateSpeechNode(conversation, editableConversation, string.Empty, j * 220, (i * 300) + 200, false, optionCallback);
                nodes.Add(dummyNode);

                // Make the dummy node advance immediately
                dummyNode.AdvanceDialogueAutomatically = true;
                dummyNode.AutoAdvanceShouldDisplayOption = false;
                dummyNode.TimeUntilAdvance = 0f;

                // Make the option node point to the dummy node
                optionNode.SetSpeech(dummyNode);
            }

            // Update previous speech node to current before resuming
            previousSpeechNode = currentSpeechNode;
        }

        // Create the end of quiz node
        UnityAction sayNextConversation = () =>
        {
            dialogueManager.SetNewDialogue(response.Get(quiz.TotalGrade));
        };
        EditableSpeechNode endOfQuiz = CreateSpeechNode(conversation, editableConversation, endOfQuizText, 0, quizTemplate.Questions.Length * 300, false, sayNextConversation);
        nodes.Add(endOfQuiz);

        // If a previous speech node exists, 
        // then make its options point to the end of quiz node
        if (previousSpeechNode != null)
        {
            foreach (EditableOptionNode option in previousSpeechNode.Options)
            {
                option.Speech.SetSpeech(endOfQuiz);
            }
        }

        // Have all the nodes register their UIDs (whatever the frick THAT means)
        foreach(EditableConversationNode node in nodes)
        {
            node.RegisterUIDs();
        }

        // Serialize the editable conversation back into the NPCConversation and return the result
        conversation.Serialize(editableConversation);
        return (conversation, quiz);
    }
    #endregion

    #region Private Methods
    private EditableSpeechNode CreateSpeechNode(NPCConversation conversation, EditableConversation editableConversation, string text, float xPos, float yPos, bool isRoot, UnityAction callback)
    {
        // Create a new speech node
        EditableSpeechNode speechNode = new EditableSpeechNode()
        {
            Text = text,
            Name = npcName,
            Icon = npcIcon,
            TMPFont = npcFont,
            ID = conversation.CurrentIDCounter,
            EditorInfo = new EditableConversationNode.EditorArgs()
            {
                xPos = xPos,
                yPos = yPos,
                isRoot = isRoot
            }
        };

        // Setup the node event.
        NodeEventHolder nodeEvent = conversation.GetNodeData(conversation.CurrentIDCounter);
        nodeEvent.Icon = npcIcon;
        nodeEvent.TMPFont = npcFont;

        // If the callback is not null that add it to the event
        if (callback != null) nodeEvent.Event.AddListener(callback);

        // Add this to the list of speech nodes
        editableConversation.SpeechNodes.Add(speechNode);

        // Update the counter
        conversation.CurrentIDCounter++;

        // Return the speech node
        return speechNode;
    }
    private EditableOptionNode CreateOptionNode(NPCConversation conversation, EditableConversation editableConversation, string text, float xPos, float yPos)
    {
        // Create a new option node with the same label as the quiz option
        EditableOptionNode optionNode = new EditableOptionNode()
        {
            Text = text,
            TMPFont = npcFont,
            ID = conversation.CurrentIDCounter,
            EditorInfo = new EditableConversationNode.EditorArgs()
            {
                xPos = xPos,
                yPos = yPos
            }
        };        

        // Add this option node to the editable conversation
        editableConversation.Options.Add(optionNode);

        // Update the current id
        conversation.CurrentIDCounter++;

        // Return the new node
        return optionNode;
    }
    #endregion
}

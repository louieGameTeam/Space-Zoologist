using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using TMPro;

public class QuizConversation : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the quiz template to run the quiz for")]
    private QuizTemplate quizTemplate;
    [SerializeField]
    [Tooltip("Icon to display for the npc")]
    private Sprite npcIcon;
    [SerializeField]
    [Tooltip("Name of the npc giving the quiz")]
    private string npcName = "Star";
    [SerializeField]
    [Tooltip("Font to use when saying each speech node")]
    private TMP_FontAsset npcFont;
    [SerializeField]
    [Tooltip("Speech node spoken when the quiz ends")]
    private string endOfQuizText = "Alright, give me a minute to process your answers...";
    #endregion

    #region Public Methods
    public (NPCConversation conversation, QuizInstance quiz) Create()
    {
        // Create the quiz instance
        QuizInstance quiz = new QuizInstance(quizTemplate);
        
        // Get the npc conversation on this object and clear it out
        NPCConversation conversation = gameObject.GetOrAddComponent<NPCConversation>();
        conversation.Clear();        

        // Create the conversation to be edited here in the code
        EditableConversation editableConversation = new EditableConversation();
        EditableSpeechNode previousSpeechNode = null;

        // Loop over every question and add speech and option nodes for each
        for(int i = 0; i < quizTemplate.Questions.Length; i++)
        {
            // Cache the current question
            QuizQuestion question = quizTemplate.Questions[i];

            // Create a new speech node
            EditableSpeechNode currentSpeechNode = new EditableSpeechNode()
            {
                Text = question.Question,
                Name = npcName,
                Icon = npcIcon,
                TMPFont = npcFont,
                ID = conversation.CurrentIDCounter
            };

            // Set some editor info so it looks nice and neat
            currentSpeechNode.EditorInfo.yPos = i * 200;
            currentSpeechNode.EditorInfo.isRoot = i == 0;

            // If a previous speech node exists, then make the options on the previous node
            // point to the speech on the current node
            if (previousSpeechNode != null)
            {
                foreach (EditableOptionNode option in previousSpeechNode.Options)
                {
                    option.SetSpeech(currentSpeechNode);
                }
            }

            // Add an option for each quiz option
            for(int j = 0; j < question.Options.Length; j++)
            {
                // Get the current option
                QuizOption option = question.Options[j];

                // Create a new option node with the same label as the quiz option
                EditableOptionNode optionNode = new EditableOptionNode()
                {
                    Text = option.Label,
                    TMPFont = npcFont
                };

                // Set some editor arguments so it looks neat in the editor window
                optionNode.EditorInfo.xPos = j * 100;
                optionNode.EditorInfo.yPos = (i * 200) + 100;

                // Add the option to the speech node
                currentSpeechNode.AddOption(optionNode);
            }

            // Add the current speech node to the editable conversation
            editableConversation.SpeechNodes.Add(currentSpeechNode);
            
            // Update previous speech node to current before resuming
            previousSpeechNode = currentSpeechNode;
            // Because... meh?!
            conversation.CurrentIDCounter++;
        }

        // Create the end of quiz node
        EditableSpeechNode endOfQuiz = new EditableSpeechNode()
        {
            Text = endOfQuizText,
            Name = npcName,
            Icon = npcIcon,
            TMPFont = npcFont,
            ID = conversation.CurrentIDCounter
        };

        // Set y position for neater look in the window
        endOfQuiz.EditorInfo.yPos = quizTemplate.Questions.Length * 200;

        // If a previous speech node exists, 
        // then make its options point to the end of quiz node
        if (previousSpeechNode != null)
        {
            foreach (EditableOptionNode option in previousSpeechNode.Options)
            {
                option.SetSpeech(endOfQuiz);
            }
        }

        // Serialize the editable conversation back into the NPCConversation and return the result
        conversation.Serialize(editableConversation);
        return (conversation, quiz);
    }
    #endregion
}

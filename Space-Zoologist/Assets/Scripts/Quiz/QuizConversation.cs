using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;
using DialogueEditor;
using TMPro;

public class QuizConversation : MonoBehaviour
{
    #region Public Properties
    public QuizTemplate Template => template;
    public QuizInstance CurrentQuiz => currentQuiz;
    public UnityEvent OnConversationEnded => onConversationEnded;
    #endregion

    #region Public Typedefs
    [System.Serializable]
    public class NPCConversationArray
    {
        public NPCConversation[] responses;
        public NPCConversation Get(QuizGrade grade) => responses[(int)grade];
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the quiz template to run the quiz for")]
    [FormerlySerializedAs("quizTemplate")]
    private QuizTemplate template;

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
    [Tooltip("Conversation to say at the beginning")]
    private NPCConversation openingConversation;
    [SerializeField]
    [TextArea(3, 10)]
    [Tooltip("Speech node spoken when the quiz ends")]
    private string endOfQuizText = "Alright, give me a minute to process your answers...";
    [SerializeField]
    [Tooltip("List of NPCConversations to respond with based on the quizes' grade")]
    [EditArrayWrapperOnEnum("responses", typeof(QuizGrade))]
    private NPCConversationArray response;
    [SerializeField]
    [Tooltip("If true, the quiz conversation will be re-spoken if the player fails the quiz")]
    private bool requizOnFail = false;

    [Space]

    [SerializeField]
    [Tooltip("Event invoked when the quiz conversation is finished")]
    private UnityEvent onConversationEnded;
    #endregion

    #region Private Fields
    private QuizInstance currentQuiz;
    private NPCConversation currentResponse;
    // Conversation that the NPC speaks to say all of the questions
    private NPCConversation currentQuizConversation;
    private readonly string[] optionLabels = new string[]
    {
        "Not at all useful",
        "Somewhat useful",
        "Very useful"
    };
    #endregion

    #region Public Methods
    public void Setup()
    {
        if (GameManager.Instance)
        {
            DialogueManager dialogueManager = GameManager.Instance.m_dialogueManager;

            // First, say the opening conversation
            dialogueManager.SetNewDialogue(openingConversation);

            // Then, say the quiz part of the conversation
            SayQuizConversationNext();
        }
    }
    public NPCConversation Create(DialogueManager dialogueManager)
    {
        // Build a new quiz. This will result in regenerating new questions from any randomized pools
        GenerateQuizInstance();

        // Create the callback that is called after any option is answered
        UnityAction OptionSelectedFunctor(int questionIndex, int optionIndex)
        {
            return () => currentQuiz.AnswerQuestion(questionIndex, optionIndex);
        }
        // Say the conversation that corresponds to the grade that the player got on the quiz
        void SayResponse()
        {
            // Destroy any previous response
            if (currentResponse) Destroy(currentResponse);
            // Instantiate a new response
            currentResponse = response.Get(CurrentQuiz.Grade).InstantiateAndSay();

            // If we should requiz when we fail, then we must say the quiz after the response
            if (requizOnFail && CurrentQuiz.Grade != QuizGrade.Excellent)
            {
                SayQuizConversationNext();
            }
            // If we will not requiz, then invoke my conversation ended event when this conversation is done
            else
            {
                // Set the quiz on the reports data to the quiz that we just finished
                GameManager.Instance.NotebookUI.Data.Reports.SetQuiz(LevelID.Current(), currentQuiz);

                // Invoke the quiz conversation ended event when the response is over
                currentResponse.OnConversationEnded(onConversationEnded.Invoke);
            }
        }

        // Try to get an npc conversation. If it exists, destroy it and add a new one
        NPCConversation conversation = gameObject.GetComponent<NPCConversation>();
        if (conversation)
        {
#if UNITY_EDITOR 
            DestroyImmediate(conversation);
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
        for (int i = 0; i < currentQuiz.RuntimeTemplate.Questions.Length; i++)
        {
            // Cache the current question
            QuizQuestion question = currentQuiz.RuntimeTemplate.Questions[i];

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
                UnityAction optionCallback = OptionSelectedFunctor(i, j);
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
        EditableSpeechNode endOfQuiz = CreateSpeechNode(conversation, editableConversation, endOfQuizText, 0, currentQuiz.RuntimeTemplate.Questions.Length * 300, false, SayResponse);
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
        conversation.RuntimeSave(editableConversation);
        return conversation;
    }
    #endregion

    #region Private Methods
    private void SayQuizConversationNext()
    {
        DialogueManager dialogue = GameManager.Instance.m_dialogueManager;

        // If there is a current quiz conversation, then destroy it
        if (currentQuizConversation) Destroy(currentQuizConversation);
        // Set the current quiz conversation
        currentQuizConversation = Create(dialogue);
        // Tell the dialogue manager to say the quiz conversation after the currently running conversation
        dialogue.SetNewQuiz(currentQuizConversation);
    }
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
    private void GenerateQuizInstance()
    {
        // Get the list of reviews for the current attempt of this level
        ReviewedResourceRequestList reviewsList = GameManager
            .Instance
            .NotebookUI
            .Data
            .Concepts
            .GetEntryWithLatestAttempt(LevelID.Current())
            .reviews;

        // If there are reviewed requests then create a quiz with additional questions
        if (reviewsList.Reviews.Count > 0)
        {
            // Filter only reviews that were granted,
            // and combine reviews that addressed and requested the same item
            ReviewedResourceRequest[] filteredReviews = reviewsList
                .Reviews
                .Where(ResourceRequestGeneratesQuestion)
                .Distinct(new ReviewedResourceRequest.ItemComparer())
                .ToArray();

            // Check to make sure there are some reviews to quiz on
            if (filteredReviews.Length > 0)
            {
                // Create an array with all the quiz questions
                QuizQuestion[] requestQuestions = new QuizQuestion[filteredReviews.Length];

                // Fill in the info for each question
                for (int i = 0; i < requestQuestions.Length; i++)
                {
                    ResourceRequest request = filteredReviews[i].Request;

                    // Set the category to the item addressed by the request
                    QuizCategory category = new QuizCategory(request.ItemAddressed, request.NeedAddressed);

                    // Generate the quiz options
                    QuizOption[] options = GenerateQuizOptions(request, category);

                    // Setup the format for the question
                    string question = $"Was the requested {request.ItemRequested.Data.Name.Get(ItemName.Type.Colloquial)} " +
                        $"useful for improving the {request.ItemAddressed.Data.Name.Get(ItemName.Type.Colloquial)}" +
                        $" {request.NeedAddressed} need?";

                    // Create the question
                    requestQuestions[i] = new QuizQuestion(question, category, options);
                }

                // Set the current quiz with additional request questions
                currentQuiz = new QuizInstance(template, requestQuestions);
            }
            else currentQuiz = new QuizInstance(template);
        }
        // If there are no reviwed requests
        // then create a quiz without additional questions
        else currentQuiz = new QuizInstance(template);
    }
    private QuizOption[] GenerateQuizOptions(ResourceRequest request, QuizCategory category)
    {
        // Create the options
        QuizOption[] options = new QuizOption[3];

        // Get the usefulness of the request
        int usefulness = request.Usefulness;

        // The option at the usefulness level has full score
        options[usefulness] = new QuizOption(optionLabels[usefulness], 2);

        if (usefulness != 1)
        {
            // Middle option has score of 1
            options[1] = new QuizOption(optionLabels[1], 1);

            // The other usefulness option has score of 0
            usefulness = (usefulness + 2) % 4;
            options[usefulness] = new QuizOption(optionLabels[usefulness], 0);
        }
        else
        {
            options[2] = new QuizOption(optionLabels[2], 1);
            options[0] = new QuizOption(optionLabels[0], 0);
        }

        return options;
    }
    private bool ResourceRequestGeneratesQuestion(ReviewedResourceRequest review)
    {
        // Get the categories of all fixed quesitons
        IEnumerable<QuizCategory> testedCategories = template.AllQuestions
            .Select(q => q.Category);
        // Create the category this request represents
        QuizCategory myCategory = new QuizCategory(review.Request.ItemAddressed, review.Request.NeedAddressed);

        // Resource request will generate a question
        // if the status is not denied or invalid,
        // and the category of the review is somewhere in the quiz
        return review.CurrentStatus != ReviewedResourceRequest.Status.Denied &&
            review.CurrentStatus != ReviewedResourceRequest.Status.Invalid && 
            testedCategories.Contains(myCategory);
    }
    #endregion
}

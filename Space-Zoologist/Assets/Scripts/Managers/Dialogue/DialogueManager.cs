using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using UnityEngine.UI;

/// <summary>
/// Handles conversations
/// </summary>
public class DialogueManager : MonoBehaviour
{
    #region Public Typedefs
    [System.Serializable]
    public struct ConversationUIData
    {
        public Sprite dialogueBackground;
        public float dialoguePanelHeight;
        public float nameTextYPos;
        public RectOffset speechTextRectOffsets;

        // Set the conversation UI to the data in this structure
        public void SetConversationUI(ConversationManager manager)
        {
            manager.BackgroundImage = dialogueBackground;
            manager.DialogueBackground.sprite = dialogueBackground;
            manager.DialoguePanel.sizeDelta = new Vector2(manager.DialoguePanel.sizeDelta.x, dialoguePanelHeight);

            manager.NameText.rectTransform.anchoredPosition = new Vector2(manager.NameText.rectTransform.anchoredPosition.x, nameTextYPos);
            manager.DialogueText.rectTransform.SetOffsets(speechTextRectOffsets);
        }
    }
    #endregion

    #region Public Properties
    public int CountQueuedConversations => queuedConversations.Count;
    #endregion

    #region Private Fields
    private NPCConversation currentDialogue = default;
    [SerializeField] private bool HideNPC = default;
    private NPCConversation startingConversation = default;
    private NPCConversation defaultConversation = default;
    [SerializeField] GameObject ConversationManagerGameObject = default;
    private Queue<NPCConversation> queuedConversations = new Queue<NPCConversation>();
    private Queue<bool> needForDeserialization = new Queue<bool>();

    private bool ContinueSpeech = false;

    [SerializeField]
    [Tooltip("How to display the conversation while NPC is active")]
    private ConversationUIData npcActive;
    [SerializeField]
    [Tooltip("How to display the conversation while the NPC is inactive")]
    private ConversationUIData npcInactive;
    #endregion

    #region Monobehaviour Messages
    public void OnValidate()
    {
        ConversationManager manager = FindObjectOfType<ConversationManager>();

        if(manager)
        {
            if (manager.NpcIcon.enabled) npcActive.SetConversationUI(manager);
            else npcInactive.SetConversationUI(manager);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Initialize stuffs here
    /// </summary>
    public void Initialize()
    {
        startingConversation = GameManager.Instance.LevelData.StartingConversation;
        defaultConversation = GameManager.Instance.LevelData.DefaultConversation;
        ConversationManager.OnConversationEnded += ConversationEnded;
        if (this.startingConversation != null)
        {
            currentDialogue = this.startingConversation;
        }
        else
        {
            UpdateCurrentDialogue();
        }
        if (ConversationManager.Instance != null)
        {
            StartNewConversation();
            //ConversationManagerGameObject.SetActive(false);
        }

        // Use this to say the ending conversation when the level starts
        //GameManager.Instance.LevelData.PassedConversation.Speak(this);
        //StartInteractiveConversation();
    }
    public void SetNewDialogue(NPCConversation newDialogue)
    {
        if (queuedConversations.Contains(newDialogue))
        {
            return;
        }
        queuedConversations.Enqueue(newDialogue);
        needForDeserialization.Enqueue(true);
    }
    public void SetNewQuiz(NPCConversation newDialogue)
    {
        if (queuedConversations.Contains(newDialogue))
        {
            return;
        }
        queuedConversations.Enqueue(newDialogue);
        needForDeserialization.Enqueue(false);
    }

    public void UpdateCurrentDialogue()
    {
        if (queuedConversations.Count > 0)
        {
            currentDialogue = queuedConversations.Dequeue();
        }
        else
        {
            currentDialogue = defaultConversation;
        }
    }

    /// <summary>
    /// Start the interactive conversation via key press or something else
    /// </summary>
    public void StartInteractiveConversation()
    {
        if (ContinueSpeech)
        {
            ConversationManagerGameObject.SetActive(!ConversationManagerGameObject.activeSelf);
        }
        else
        {
            if (!ConversationManagerGameObject.activeSelf)
            {
                AudioManager.instance?.PlayOneShot(SFXType.Notification);
                UpdateCurrentDialogue();
                // if there is a queued conversation and it forbid deserialization, do runtime load
                if (needForDeserialization.Count >= 1 && needForDeserialization.Dequeue() == false) {
                    StartNewConversationWithoutDeserialization();
                }
                else
                    StartNewConversation();
            }
            else
            {
                ConversationManagerGameObject.SetActive(false);
            }
        }
    }

    public void SetNPCActive(bool active)
    {
        ConversationManager manager = ConversationManager.Instance;
        manager.NpcIcon.enabled = active;

        if (active) npcActive.SetConversationUI(manager);
        else npcInactive.SetConversationUI(manager);
    }
    #endregion

    #region Private Methods
    private void ConversationEnded()
    {
        if(queuedConversations.Count > 0)
        {
            UpdateCurrentDialogue();
            bool need = needForDeserialization.Dequeue();
            if (need)
                StartNewConversation();
            else
                StartNewConversationWithoutDeserialization();
        }
        else
        {
            ContinueSpeech = false;
            // inconsistent access to the gameObject causes missing ref exception
            // after scene transitions. Moved to ConversationManager.TurnOffUI()
            //ConversationManagerGameObject.SetActive(false);
            GameManager.Instance.m_menuManager.ToggleUI(true);
        }
    }
    private void StartNewConversation()
    {
        GameManager.Instance.m_menuManager.ToggleUI(false);
        ConversationManagerGameObject.SetActive(true);
        ConversationManager.Instance.StartConversation(currentDialogue);
        ContinueSpeech = true;
    }
    private void StartNewConversationWithoutDeserialization()
    {
        GameManager.Instance.m_menuManager.ToggleUI(false);
        ConversationManagerGameObject.SetActive(true);
        ConversationManager.Instance.StartConversationWithoutDeserialization(currentDialogue);
        ContinueSpeech = true;
    }
    #endregion
}
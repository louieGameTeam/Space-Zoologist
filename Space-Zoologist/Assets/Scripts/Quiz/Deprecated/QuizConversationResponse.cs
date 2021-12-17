using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

[System.Serializable]
public class QuizConversationResponse
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the conversation spoken when the response is activated")]
    private NPCConversation conversation;
    [SerializeField]
    [Tooltip("Name of the next level to load when this response is finished")]
    private string nextLevelName = "Level1E1";
    #endregion

    #region Public Methods
    public void Respond()
    {
        if(GameManager.Instance)
        {
            // Deserialize the conversation for modification
            EditableConversation editableConversation = conversation.DeserializeForEditor();

            // Filter all speech nodes that have no speech node after and have no options after
            IEnumerable<EditableSpeechNode> leaves = editableConversation.SpeechNodes
                .Where(node => node.SpeechUID == EditableConversation.INVALID_UID && (node.OptionUIDs == null || node.OptionUIDs.Count <= 0));

            // For each event associated with the leaf, make that leaf end the conversation when finished
            foreach(EditableSpeechNode leaf in leaves)
            {
                NodeEventHolder eventHolder = conversation.GetNodeData(leaf.ID);
                eventHolder.Event.AddListener(OnConversationEnded);
            }

            // Say the given conversation on the dialogue manager
            DialogueManager dialogue = GameManager.Instance.m_dialogueManager;
            dialogue.SetNewDialogue(conversation);
        }
    }
    #endregion

    #region Private Methods
    private void OnConversationEnded()
    {
        // Try to get the level data loader
        LevelDataLoader loader = Object.FindObjectOfType<LevelDataLoader>();

        // If a loader was found then use it to load the given level
        if (loader)
        {
            loader.LoadLevel(nextLevelName);
        }
    }
    #endregion
}

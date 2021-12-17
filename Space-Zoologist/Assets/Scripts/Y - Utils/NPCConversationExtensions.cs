using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DialogueEditor;

public static class NPCConversationExtensions
{
    public static NPCConversation InstantiateAndSay(this NPCConversation conversation)
    {
        NPCConversation copyConversation = Object.Instantiate(conversation);

        // If instantiation is successful then try to say the conversation
        if (copyConversation)
        {
            // Try to get the game manager
            GameManager gameManager = GameManager.Instance;

            // If game manager exists then try to get the dialogue manager
            if (gameManager)
            {
                DialogueManager dialogueManager = gameManager.m_dialogueManager;

                // If dialogue manager exists then say the copied conversation
                if (dialogueManager)
                {
                    dialogueManager.SetNewDialogue(copyConversation);
                }
                else Debug.Log($"Cannot say conversation {copyConversation} " +
                    $"because instance of {nameof(GameManager)} " +
                    $"has no {nameof(DialogueManager)}");
            }
            else Debug.Log($"Cannot say conversation {copyConversation} " +
                $"because no instance of {nameof(GameManager)} could be found");

            return copyConversation;
        }
        else
        {
            Debug.Log($"Failed to instantiate conversation {conversation}");
            return null;
        }
    }

    public static void OnConversationEnded(this NPCConversation conversation, UnityAction action)
    {
        // Deserialize the conversation for modification
        EditableConversation editableConversation = conversation.DeserializeForEditor();

        // Filter all speech nodes that have no speech node after and have no options after
        IEnumerable<EditableSpeechNode> leaves = editableConversation.SpeechNodes
            .Where(node => node.SpeechUID == EditableConversation.INVALID_UID && (node.OptionUIDs == null || node.OptionUIDs.Count <= 0));

        // For each event associated with the leaf, make that leaf end the conversation when finished
        foreach (EditableSpeechNode leaf in leaves)
        {
            NodeEventHolder eventHolder = conversation.GetNodeData(leaf.ID);
            eventHolder.Event.AddListener(action);
        }
    }
}

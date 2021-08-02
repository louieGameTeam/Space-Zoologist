using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;

public class DialogueParser : MonoBehaviour
{
    [System.Serializable]
    public class IDToIcon
    {
        public string id;
        public Sprite icon;
    }
    public bool tryParseConversation = false; // will disable itself once a conversation is spawned
    public Text text;
    public string defaultName;
    public Sprite defaultIcon;
    public TMPro.TMP_FontAsset defaultFont;
    public IDToIcon[] iconIDInput;
    public void OnValidate()
    {
        if (text == null || !tryParseConversation) return;

        tryParseConversation = false; // try parsing once


        GameObject gameObject = new GameObject("New Conversation");
        gameObject.transform.parent = transform;

        NPCConversation NPCConversation = gameObject.AddComponent<NPCConversation>();

        // Load in id and icon info
        Dictionary<string, Sprite> idToIconDictionary = new Dictionary<string, Sprite>();
        if (iconIDInput.Length > 0)
        {
            foreach (var pair in iconIDInput)
            {
                idToIconDictionary.Add(pair.id, pair.icon);
            }
        }

        // Preprocess
        string toParse = text.text;
        EditableConversation conversation = new EditableConversation();
        string[] nodeTexts = toParse.Split('\n');

        // Initialize root node
        EditableSpeechNode root = new EditableSpeechNode();
        root.Text = nodeTexts[0].Substring(nodeTexts[0].IndexOf(':', 1) + 1).Trim();
        root.Name = defaultName;
        root.Icon = defaultIcon;
        string iconID = nodeTexts[0].Substring(1, nodeTexts[0].IndexOf(':', 1) - 1);
        if (idToIconDictionary.ContainsKey(iconID))
        {
            root.Icon = idToIconDictionary[iconID];
            NPCConversation.GetNodeData(NPCConversation.CurrentIDCounter).Icon = root.Icon;
        }
        root.ID = NPCConversation.CurrentIDCounter++;
        root.TMPFont = defaultFont;
        root.EditorInfo.isRoot = true;
        conversation.SpeechNodes.Add(root);
        bool previousNodeIsSpeech = true;

        EditableSpeechNode previousSpeech = root;
        List<EditableOptionNode> previousOptions = new List<EditableOptionNode>();
        List<EditableConversationNode> nodes = new List<EditableConversationNode>();

        nodes.Add(root);

        for (int i = 1; i < nodeTexts.Length; i++)
        {
            string currentText = nodeTexts[i];
            if (currentText.Length == 0) continue;

            EditableConversationNode node;

            // First char is a colon, this is a speech
            if (currentText[0] == ':')
            {
                EditableSpeechNode speechNode = new EditableSpeechNode();
                speechNode.Name = defaultName;

                // text enclosed by colons is id of the icon
                iconID = currentText.Substring(1, currentText.IndexOf(':', 1) - 1);
                if (idToIconDictionary.ContainsKey(iconID))
                {
                    speechNode.Icon = idToIconDictionary[iconID];
                    NPCConversation.GetNodeData(NPCConversation.CurrentIDCounter).Icon = speechNode.Icon;
                }
                else
                {
                    // unknown id
                    speechNode.Icon = defaultIcon;
                }

                // connect previous nodes to speech
                if (previousNodeIsSpeech)
                {
                    speechNode.parents.Add(previousSpeech);
                    previousSpeech.SetSpeech(speechNode);
                }
                else
                {
                    // Connect previous immediate options to node
                    foreach (var option in previousOptions)
                    {
                        speechNode.parents.Add(option);
                        option.SetSpeech(speechNode);
                    }
                    previousOptions.Clear();
                }

                previousNodeIsSpeech = true;
                previousSpeech = speechNode;
                conversation.SpeechNodes.Add(speechNode);

                node = speechNode;
            }
            else
            {
                EditableOptionNode optionNode = new EditableOptionNode();

                optionNode.parents.Add(previousSpeech);
                previousSpeech.AddOption(optionNode);
                previousNodeIsSpeech = false;
                previousOptions.Add(optionNode);
                conversation.Options.Add(optionNode);

                node = optionNode;
            }
            // Common info
            node.EditorInfo.yPos = 100 * i;
            node.Text = currentText.Substring(currentText.IndexOf(':', 1) + 1).Trim();
            node.TMPFont = defaultFont;
            node.ID = NPCConversation.CurrentIDCounter++;
            nodes.Add(node);
        }

        foreach (EditableConversationNode node in nodes)
        {
            node.RegisterUIDs();
        }

        NPCConversation.Serialize(conversation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;

public class DialogueParser : MonoBehaviour
{
    public Text text;
    public NPCConversation NPCConversation;
    public string defaultName;
    public Sprite defaultIcon;
    public TMPro.TMP_FontAsset defaultFont;
    public void OnValidate()
    {
        if (text == null) return;
        if (NPCConversation == null) {
            NPCConversation = gameObject.GetComponent<NPCConversation>();
        }
        if (NPCConversation == null)
        {
            NPCConversation = gameObject.AddComponent<NPCConversation>();
        }

        string toParse = text.text;
        EditableConversation conversation = new EditableConversation();
        string[] nodeTexts = toParse.Split('\n');

        //Initialize root node
        EditableSpeechNode root = new EditableSpeechNode();
        root.Text = nodeTexts[0].Substring(nodeTexts[0].IndexOf(':', 1) + 1).Trim();
        root.Name = defaultName;
        root.Icon = defaultIcon;
        root.TMPFont = defaultFont;
        root.EditorInfo.isRoot = true;
        conversation.SpeechNodes.Add(root);

        bool previousNodeIsSpeech = true;
        EditableSpeechNode previousSpeech = root;
        EditableOptionNode previousOption = new EditableOptionNode();
        List<EditableConversationNode> nodes = new List<EditableConversationNode>();

        for (int i = 1; i < nodeTexts.Length; i++) {
            string currentText = nodeTexts[i];
            if (currentText.Length == 0) continue;
            if (currentText[0] == ':')
            {
                EditableSpeechNode speechNode = new EditableSpeechNode();
                speechNode.EditorInfo.yPos = 100 * i;
                speechNode.Text = currentText.Substring(currentText.IndexOf(':', 1)+1).Trim();
                speechNode.Name = defaultName;
                speechNode.Icon = defaultIcon;
                speechNode.TMPFont = defaultFont;
                speechNode.ID = NPCConversation.CurrentIDCounter++;
                if (previousNodeIsSpeech)
                {
                    speechNode.parents.Add(previousSpeech);
                    previousSpeech.SetSpeech(speechNode);
                }
                else {
                    speechNode.parents.Add(previousOption);
                    previousOption.SetSpeech(speechNode);
                }
                previousNodeIsSpeech = true;
                previousSpeech = speechNode;
                nodes.Add(speechNode);
                conversation.SpeechNodes.Add(speechNode);
            }
            else {
                EditableOptionNode optionNode = new EditableOptionNode();
                optionNode.EditorInfo.yPos = 100 * i;
                optionNode.Text = currentText.Substring(currentText.IndexOf(':', 1) + 1);
                optionNode.parents.Add(previousSpeech);
                optionNode.ID = NPCConversation.CurrentIDCounter++;
                previousSpeech.AddOption(optionNode);
                previousNodeIsSpeech = false;
                previousOption = optionNode;
                nodes.Add(optionNode);
                conversation.Options.Add(optionNode);
            }
        }
        foreach (EditableConversationNode node in nodes) {
            node.RegisterUIDs();
        }
        NPCConversation.Serialize(conversation);
    }
}

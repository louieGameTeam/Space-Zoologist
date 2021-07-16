using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using UnityEngine.Events;

public class DialogueGenerator : MonoBehaviour
{
    [SerializeField] DialogueEditor.NPCConversation nPCConversation = default;
    [SerializeField] DialogueManager dialogueManager = default;
    [SerializeField] string DialogueJSON = default;
    [SerializeField] bool parse = default;
    // Start is called before the first frame update

    // Testing out possibility of automatically parsing dialogue
    // TODO define structure for dialogue and parse into this script
    private void OnValidate()
    {
        DialogueEditor.EditableConversation ec = nPCConversation.DeserializeForEditor();
        for (int i = 0; i < ec.SpeechNodes.Count; i++)
        {
            ec.SpeechNodes[i].Text = i + " success!";
        }
        nPCConversation.Serialize(ec);
    }
}

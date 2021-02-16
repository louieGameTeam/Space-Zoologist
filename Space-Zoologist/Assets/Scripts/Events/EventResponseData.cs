using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventResponseData
{
    [SerializeField] public int PopulationTriggerSize = default;
    [SerializeField] public string resourceName = default;
    [SerializeField] public int amount = default;
    [SerializeField] public DialogueEditor.NPCConversation NPCConversation = default;
}

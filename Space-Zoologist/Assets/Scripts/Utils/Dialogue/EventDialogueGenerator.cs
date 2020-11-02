using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueEditor;

/// <summary>
/// This handles dialogues related to events
/// </summary>
public class EventDialogueGenerator : MonoBehaviour
{
    [SerializeField] private NPCConversation eventTriggeredConversation;

    [SerializeField] private TextMeshProUGUI dialogueTextMeshPro = default;
    [SerializeField] private DialogueSheetLoader dialgoueSheetLoader = default;

    private Dictionary<string, List<string>> eventDialogue = new Dictionary<string, List<string>>();
    private System.Random random = new System.Random();

    private EventType lastEventType = default;

    /// <summary>
    /// Subcribe to events that will trigger message.
    /// </summary>
    private void Start()
    {
        this.dialgoueSheetLoader.LoadEventDialogue(this.eventDialogue);

        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountIncreased, () =>
        {
            this.lastEventType = EventType.PopulationCountIncreased;
            ConversationManager.Instance.StartConversation(eventTriggeredConversation);
        });
    }

    /// <summary>
    /// Display a random dialogue associates with this event.
    /// </summary>
    public void FillInDialogue()
    {
        dialogueTextMeshPro.text = this.eventDialogue[this.lastEventType.ToString()][this.random.Next(this.eventDialogue[this.lastEventType.ToString()].Count)];
    }
}

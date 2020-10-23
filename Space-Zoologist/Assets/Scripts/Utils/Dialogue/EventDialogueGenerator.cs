using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueEditor;

public class EventDialogueGenerator : MonoBehaviour
{
    [SerializeField] private NPCConversation eventTriggeredConversation;

    [SerializeField] private TextMeshProUGUI dialogueTextMeshPro = default;
    [SerializeField] private DialogueSheetLoader dialgoueSheetLoader = default;

    private Dictionary<string, List<string>> eventDialogue = new Dictionary<string, List<string>>();
    private System.Random random = new System.Random();

    private EventType lastEventType = default;

    // Subcribe to events that will trigger message
    void Start()
    {
        this.dialgoueSheetLoader.LoadEventDialogue(this.eventDialogue);

        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountIncreased, () =>
        {
            this.lastEventType = EventType.PopulationCountIncreased;
            ConversationManager.Instance.StartConversation(eventTriggeredConversation);
        });
    }

    public void FillInDialogue()
    {
        dialogueTextMeshPro.text = this.eventDialogue[this.lastEventType.ToString()][this.random.Next(this.eventDialogue[this.lastEventType.ToString()].Count)];
    }
}

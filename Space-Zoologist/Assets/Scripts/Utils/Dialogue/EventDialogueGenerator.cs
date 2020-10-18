using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventDialogueGenerator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueTextMeshPro = default;
    [SerializeField] private DialogueSheetLoader dialgoueSheetLoader = default;

    private Dictionary<string, List<string>> eventDialogue = new Dictionary<string, List<string>>();
    private System.Random random = new System.Random();

    // Subcribe to event's that will trigger message
    void Start()
    {
        this.dialgoueSheetLoader.LoadEventDialogue(this.eventDialogue);

        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountIncreased, () =>
        {
            dialogueTextMeshPro.text = this.eventDialogue[EventType.PopulationCountIncreased.ToString()][this.random.Next(this.eventDialogue[EventType.PopulationCountIncreased.ToString()].Count)];
        });
    }
}

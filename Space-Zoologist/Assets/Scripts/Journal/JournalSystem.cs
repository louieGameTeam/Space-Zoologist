using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: figure out how duplicate entries handled
public class JournalSystem : MonoBehaviour, ISelectableItem
{
    [SerializeField] private GameObject JournalItemPrefab = default;
    [SerializeField] private GameObject JournalItemPopupPrefab = default;
    [SerializeField] private GameObject JournalSearch = default;
    [Expandable] public List<ScriptableObject> TestItems = default;
    // Could potentially use a dictionary but that doesn't allow duplicate entries 
    private List<KeyValuePair<string, GameObject>> Journal = new List<KeyValuePair<string, GameObject>>();
    private readonly ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();

    public void Start()
    {
        this.OnItemSelectedEvent.AddListener(OnItemSelected);
    }

    public void TestJournal()
    {
        foreach(ScriptableObject testItem in this.TestItems)
        {
            InitializeItem(testItem);
        }
    }
    // Called when a new species (or item?) is found
    public void InitializeItem(ScriptableObject itemDiscovered)
    {
        GameObject newItem = Instantiate(this.JournalItemPrefab, this.transform);
        SelectableItem selectableItem = newItem.GetComponent<SelectableItem>();
        selectableItem.InitializeItem(itemDiscovered, this.OnItemSelectedEvent);
        this.Journal.Add(new KeyValuePair<string, GameObject>(selectableItem.ItemInfo.ItemName, newItem));
    }

    // Iterative search to handle duplicate case
    private void SearchJournal()
    {
        string ItemName = JournalSearch.GetComponent<InputField>().text.ToString();
        if (ItemName.Equals("") || ItemName.Equals("Search"))
        {
            foreach (var journalEntry in this.Journal)
            {
                journalEntry.Value.SetActive(true);
            }
            JournalSearch.GetComponent<InputField>().text = "Search";
        }
        else
        {
            foreach(var journalEntry in this.Journal)
            {
                if (journalEntry.Key.Equals(ItemName, System.StringComparison.OrdinalIgnoreCase))
                {
                    journalEntry.Value.SetActive(true);
                }
                else
                {
                    journalEntry.Value.SetActive(false);
                }
            }
        }
    }

    private void DisplayItem(GameObject itemToDispaly)
    {
        SelectableItem item = itemToDispaly.GetComponent<SelectableItem>();
        this.JournalItemPopupPrefab.transform.GetChild(0).GetComponent<Text>().text = item.ItemInfo.ItemName;
        this.JournalItemPopupPrefab.GetComponent<Button>().onClick.AddListener(ClosePopup);
        // Should we be keeping track of what the date is in game?
        // this.gameObject.transform.GetChild(1).GetComponent<Text>().text = currentDate?
        this.JournalItemPopupPrefab.transform.GetChild(2).GetComponent<Text>().text = item.ItemInfo.ItemDescription;
        this.JournalItemPopupPrefab.transform.GetChild(3).GetComponent<Image>().sprite = item.ItemInfo.Sprite;
        this.JournalItemPopupPrefab.SetActive(true);
    }

    public void ClosePopup()
    {
        this.JournalItemPopupPrefab.SetActive(false);
    }

    public void OnItemSelected(GameObject itemSelected)
    {
        ClosePopup();
        DisplayItem(itemSelected);
    }
}

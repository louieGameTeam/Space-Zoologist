using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Holds information about item and handles ItemSelection through custom UnityEvent called ItemSelectedEvent
/// </summary>
/*
 * https://stackoverflow.com/questions/44734580/why-choose-unityevent-over-native-c-sharp-events
 * states that native c# events are better than unity events in some cases so if we decide to go with this implementation, might want to change
 */
public class ItemSelectedEvent : UnityEvent<GameObject> { }
public class SelectableItem : MonoBehaviour
{
    // Modify to be more generic?
    public SelectableItemSO ItemInfo { get; set; }
    public ScriptableObject OriginalItem { get; set; }
    private ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();

    /// <summary>
    /// Currently supports Species and SelectableItemSO.
    /// Make sure there is a method listeneing to this ItemSelectedEvent 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    // TODO: is there a better way to handle generics?
    // TODO: figure out how other scriptable objects prices should be set
    // Possibly a custom gui thing 
    public void Initialize(ScriptableObject item, ItemSelectedEvent action)
    {
        // Populates specific prefab with information based off of Scriptable Object type
        if (item is Species)
        {
            this.ItemInfo = ScriptableObject.CreateInstance<SelectableItemSO>();
            this.ItemInfo.ItemName = ((Species)item).SpeciesName;
            this.ItemInfo.Sprite = ((Species)item).Sprite;
            ColorBlock cb = this.gameObject.GetComponent<Button>().colors;
            cb.normalColor = Color.cyan;
            this.gameObject.GetComponent<Button>().colors = cb;
        }
        else if (item is SelectableItemSO)
        {
            this.ItemInfo = (SelectableItemSO)item;
            ColorBlock cb = this.gameObject.GetComponent<Button>().colors;
            cb.normalColor = this.ItemInfo.ItemCategory;
            this.gameObject.GetComponent<Button>().colors = cb;
        }
        else if (item is TerrainTile)
        {
            this.ItemInfo = ScriptableObject.CreateInstance<SelectableItemSO>();
            this.ItemInfo.ItemName = ((TerrainTile)item).TileName;
            this.ItemInfo.ItemDescription = "landscaping";
            ColorBlock cb = this.gameObject.GetComponent<Button>().colors;
            cb.normalColor = Color.yellow;
            this.gameObject.GetComponent<Button>().colors = cb;
            this.ItemInfo.ItemCost = 1f;
            this.OriginalItem = item;
            // TODO: figure out how images can be added
        }
        this.gameObject.transform.GetChild(0).GetComponent<Text>().text = this.ItemInfo.ItemName;
        this.gameObject.transform.GetChild(1).GetComponent<Text>().text = this.ItemInfo.ItemCost.ToString();
        this.gameObject.transform.GetChild(3).GetComponent<Image>().sprite = this.ItemInfo.Sprite;
        this.OnItemSelectedEvent = action;
    }

    public void ItemSelected()
    {
        this.OnItemSelectedEvent.Invoke(this.gameObject);
    }
}

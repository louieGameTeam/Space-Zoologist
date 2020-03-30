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
 * states that native c# events are better than unity events so if we decide to go with this implementation, might want to change
 */
public class ItemSelectedEvent : UnityEvent<GameObject> { }
public class SelectableItem : MonoBehaviour
{
    public SelectableItemSO ItemInfo { get; set; }
    private ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();

    /// <summary>
    /// Currently supports Species and SelectableItemSO.
    /// Make sure there is a method listeneing to this ItemSelectedEvent 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    // TODO: is there a better way to handle generics?
    public void InitializeItem(ScriptableObject item, ItemSelectedEvent action)
    {   if (item is Species)
        {
            this.ItemInfo = ScriptableObject.CreateInstance<SelectableItemSO>();
            this.ItemInfo.ItemName = ((Species)item).SpeciesName;
            this.ItemInfo.Sprite = ((Species)item).Sprite;
            // TODO: setup a way to create the description based off of the needs of the item
        }
        else if (item is SelectableItemSO)
        {
            this.ItemInfo = (SelectableItemSO)item;
        }
        this.gameObject.transform.GetChild(0).GetComponent<Text>().text = this.ItemInfo.ItemName;
        this.gameObject.transform.GetChild(1).GetComponent<Text>().text = this.ItemInfo.ItemCost.ToString();
        this.gameObject.transform.GetChild(2).GetComponent<Text>().text = this.ItemInfo.ItemDescription;
        this.gameObject.transform.GetChild(3).GetComponent<Image>().sprite = this.ItemInfo.Sprite;
        this.OnItemSelectedEvent = action;
    }

    public void ItemSelected()
    {
        this.OnItemSelectedEvent.Invoke(this.gameObject);
    }
}

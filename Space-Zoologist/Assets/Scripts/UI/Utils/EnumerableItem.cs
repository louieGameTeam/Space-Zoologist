using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class EnumerableItem
{
    // Publicly accessible, only privately set
    public object Data { get; private set; }
    public string TextDisplay { get; private set; }
    public Sprite ImageDisplay { get; private set; }

    // Constructors
    public EnumerableItem(object Data, string TextDisplay, Sprite ImageDisplay)
    {
        this.Data = Data;
        this.TextDisplay = TextDisplay;
        this.ImageDisplay = ImageDisplay;
    }
    public EnumerableItem(object Data, string TextDisplay) : this(Data, TextDisplay, null) { }
    public EnumerableItem(object Data, Sprite ImageDisplay) : this(Data, null, ImageDisplay) { }

    // Build an array of enumerable items from a list of items
    public static EnumerableItem[] BuildFrom<T>(List<T> enumerables)
    {
        EnumerableItem[] items = new EnumerableItem[enumerables.Count];
        for(int i = 0; i < enumerables.Count; i++)
        {
            items[i] = new EnumerableItem(enumerables[i], enumerables[i].ToString());
        }
        return items;
    }

    // Set the display on components
    public void SetDisplay(TextMeshProUGUI text, Image image)
    {
        text.text = TextDisplay;
        image.sprite = ImageDisplay;

        // Enable display objects depending on whether display items are available
        image.enabled = ImageDisplay != null;
        text.enabled = TextDisplay != null;
    }

    // Simple function to convert the data to the given type
    public T ConvertData<T>()
    {
        return (T)Data;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RichTextTag
{
    // Public accessors
    public string Name => name;
    public string Value => value;
    public string OpeningTag => "<" + name + "=" + value + ">";
    public string ClosingTag => "</" + name + ">";
    // Total length of all text in the tag
    public int Length => OpeningTag.Length + ClosingTag.Length;

    // Private editor data
    [SerializeField]
    [Tooltip("Name of the tag")]
    private string name;
    [SerializeField]
    [Tooltip("Value of the tag")]
    private string value;

    public RichTextTag(string name, string value)
    {
        this.name = name;
        this.value = value;
    }

    // Apply a rich text tag to a substring in this string, 
    // and return the string with the rich text markups
    public string Apply(string baseString, int start, int length)
    {
        baseString = baseString.Insert(start, OpeningTag);
        baseString = baseString.Insert(start + length + OpeningTag.Length, ClosingTag);
        return baseString;
    }
}

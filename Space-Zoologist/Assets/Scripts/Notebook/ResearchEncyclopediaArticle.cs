using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEncyclopediaArticle
{
    // Public accessors of private data
    public string Title => title;
    public string Author => author;
    public string Text => text;
    public Sprite Image => image;

    // Private data
    [SerializeField]
    private string title;
    [SerializeField]
    private string author;
    [SerializeField]
    [TextArea] 
    private string text;
    [SerializeField]
    private Sprite image;
}

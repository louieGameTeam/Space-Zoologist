using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEncyclopediaArticle
{
    // Public accessors of private data
    public ResearchEncyclopediaArticleID ID => id;
    public string Text => text;
    public Sprite Image => image;

    // Private editor data
    [SerializeField]
    [Tooltip("Identification for this encyclopedia article")]
    private ResearchEncyclopediaArticleID id;
    [SerializeField]
    [TextArea] 
    [Tooltip("Text in the article")]
    private string text;
    [SerializeField]
    [Tooltip("Image that can display in the article")]
    private Sprite image;
}

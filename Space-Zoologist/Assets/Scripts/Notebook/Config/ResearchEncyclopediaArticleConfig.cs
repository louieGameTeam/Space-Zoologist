using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEncyclopediaArticleConfig
{
    #region Public accessors of private data
    public ResearchEncyclopediaArticleID ID => id;
    public string Text
    {
        get
        {
            string t = text.Replace("{", "");
            return t.Replace("}", "");
        }
    }
    public Sprite[] Sprites => sprites;
    // Raw text including curly braces
    public string RawText => text;
    #endregion

    #region Private editor data
    [SerializeField]
    [Tooltip("Identification for this encyclopedia article")]
    private ResearchEncyclopediaArticleID id = ResearchEncyclopediaArticleID.Empty;
    [SerializeField]
    [TextArea(3, 20)] 
    [Tooltip("Text in the article")]
    private string text = "";
    [SerializeField]
    [Tooltip("Image that can display in the article")]
    private Sprite[] sprites = null;
    #endregion
}

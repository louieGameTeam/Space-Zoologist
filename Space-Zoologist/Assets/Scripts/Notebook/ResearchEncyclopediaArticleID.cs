using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ResearchEncyclopediaArticleID
{
    // Public accessors
    public string Title => title;
    public string Author => author;

    // Private editor data
    [SerializeField]
    [Tooltip("Title of the article")]
    private string title;
    [SerializeField]
    [Tooltip("Author of the article")]
    private string author;

    // Constructor to set the values
    public ResearchEncyclopediaArticleID(string title, string author)
    {
        this.title = title;
        this.author = author;
    }

    public static bool operator==(ResearchEncyclopediaArticleID a, ResearchEncyclopediaArticleID b)
    {
        return a.title == b.title && a.author == b.author;
    }
    public static bool operator!=(ResearchEncyclopediaArticleID a, ResearchEncyclopediaArticleID b)
    {
        return !(a == b);
    }
    public override bool Equals(object obj)
    {
        // If other object is null, it cannot be equal to this struct
        if (obj == null) return false;
        // If the types are equal, use the operator
        else if (obj.GetType() == typeof(ResearchEncyclopediaArticleID))
        {
            return this == (ResearchEncyclopediaArticleID)obj;
        }
        // If the types are not equal, the other object cannot be equal to this one
        else return false;
    }
    public override int GetHashCode()
    {
        return title.GetHashCode() + author.GetHashCode();
    }
    public override string ToString()
    {
        string str = "\"" + title + "\"";
        if (author != "") str += " by " + author;
        return str;
    }
}

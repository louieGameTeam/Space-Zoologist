using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
/// <summary>
/// A single bookmark in the NotebookUI. It has a label and 
/// a list of bookmark data that handle navigation
/// </summary>
public struct Bookmark
{
    #region Public Properties
    // Public accessors
    public string Label => label;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Label to display for the bookmark")]
    private string label;
    [SerializeField]
    [Tooltip("List of data to set for the bookmark")]
    private BookmarkData[] datas;
    #endregion

    #region Constructors
    public Bookmark(string label, params BookmarkData[] datas)
    {
        this.label = label;
        this.datas = datas;
    }
    #endregion

    #region Public Methods
    public void Navigate(Dictionary<string, BookmarkTarget> nameTargetMap)
    {
        foreach (BookmarkData data in datas)
        {
            data.SetTargetData(nameTargetMap);
        }
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        else if (obj.GetType() == GetType()) return this == (Bookmark)obj;
        else return false;
    }
    public override int GetHashCode()
    {
        return datas.GetHashCode();
    }
    #endregion

    #region Operators
    public static bool operator ==(Bookmark a, Bookmark b) => a.datas.SequenceEqual(b.datas);
    public static bool operator !=(Bookmark a, Bookmark b) => !(a == b);
    #endregion
}

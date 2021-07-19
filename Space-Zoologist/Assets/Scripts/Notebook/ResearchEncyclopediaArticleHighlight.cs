using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ResearchEncyclopediaArticleHighlight : System.IComparable<ResearchEncyclopediaArticleHighlight>
{
    // Public accessors for private data
    public int Start => start;
    public int End => end;
    public int Length => end - start;

    [SerializeField]
    [Tooltip("Start position of the highlight in the article")]
    private int start;
    [SerializeField]
    [Tooltip("End position of the highlight in the article")]
    private int end;

    public ResearchEncyclopediaArticleHighlight(int start, int end)
    {
        this.start = start;
        this.end = end;
    }

    public bool Overlap(ResearchEncyclopediaArticleHighlight other)
    {
        // There is some overlap either if the other start is inside my range,
        // or if the other end is inside my range
        return other.start >= start && other.start <= end ||
            other.end >= start && other.end <= end;
    }

    // Combine two research highlights by bridging the gap between them
    public ResearchEncyclopediaArticleHighlight Combine(ResearchEncyclopediaArticleHighlight other)
    {
        int newStart = Mathf.Min(start, other.start);
        int newEnd = Mathf.Max(end, other.end);
        return new ResearchEncyclopediaArticleHighlight(newStart, newEnd);
    }

    public int CompareTo(ResearchEncyclopediaArticleHighlight other)
    {
        return start.CompareTo(other.start);
    }
}

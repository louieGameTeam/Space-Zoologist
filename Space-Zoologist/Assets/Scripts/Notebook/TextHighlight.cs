using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TextHighlight : System.IComparable<TextHighlight>
{
    // Public accessors for private data
    public int Start => start;
    public int End => end;
    public int Length => end - start;
    public bool Valid => start < end;

    [SerializeField]
    [Tooltip("Start position of the highlight in the article")]
    private int start;
    [SerializeField]
    [Tooltip("End position of the highlight in the article")]
    private int end;

    public TextHighlight(int start, int end)
    {
        this.start = start;
        this.end = end;
    }

    public bool InRange(int i)
    {
        return i >= start && i <= end;
    }
    public bool Overlap(TextHighlight other)
    {
        return InRange(other.start) || InRange(other.end) || other.InRange(start) || other.InRange(end);
    }
    // True if the other highlight is fully contained in this highlight
    public bool Contains(TextHighlight other)
    {
        return other.start >= start && other.end <= end;
    }
    // Combine two research highlights by bridging the gap between them
    public TextHighlight Union(TextHighlight other)
    {
        int newStart = Mathf.Min(start, other.start);
        int newEnd = Mathf.Max(end, other.end);
        return new TextHighlight(newStart, newEnd);
    }
    // Intersect two highlights by return the highlight that they both highlight over
    // NOTE: the resulting highlight is invalid (start >= end) if there is no overlap
    public TextHighlight Intersect(TextHighlight other)
    {
        int newStart = Mathf.Max(start, other.start);
        int newEnd = Mathf.Min(end, other.end);
        return new TextHighlight(newStart, newEnd);
    }
    // Return a list with at most two highlights representing the highlight on one side of the negation
    // followed by the highlight on the other side of the negation
    public List<TextHighlight> Negate(TextHighlight other)
    {
        if(Overlap(other))
        {
            List<TextHighlight> highlights = new List<TextHighlight>();

            TextHighlight intersect = Intersect(other);
            // The highlight to the left of the intersection
            TextHighlight left = new TextHighlight(start, intersect.start);
            // The highlight to the right of the intersection
            TextHighlight right = new TextHighlight(intersect.end, end);

            // Check the highlights that are valid before adding them to the list
            if (left.Valid) highlights.Add(left);
            if (right.Valid) highlights.Add(right);

            return highlights;
        }
        else
        {
            return new List<TextHighlight> { this };
        }
    }

    public int CompareTo(TextHighlight other)
    {
        return start.CompareTo(other.start);
    }

    public override string ToString()
    {
        return "Highlight at [" + start + ", " + end + "]";
    }
}

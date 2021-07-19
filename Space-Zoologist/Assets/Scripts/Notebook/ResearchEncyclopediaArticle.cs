using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEncyclopediaArticle
{
    // Public accessors of private data
    public ResearchEncyclopediaArticleID ID => id;
    public string Text => trueText;
    public Sprite Image => image;
    public List<ResearchEncyclopediaArticleHighlight> Highlights => highlights;

    // Private editor data
    [SerializeField]
    [Tooltip("Identification for this encyclopedia article")]
    private ResearchEncyclopediaArticleID id;
    [SerializeField]
    [TextArea(3, 20)] 
    [Tooltip("Text in the article")]
    private string text;
    [SerializeField]
    [Tooltip("Image that can display in the article")]
    private Sprite image;

    [Tooltip("List of all the highlights made in this article")]
    private List<ResearchEncyclopediaArticleHighlight> highlights = new List<ResearchEncyclopediaArticleHighlight>();

    // The text that is publicly accessible
    // Excludes curly braces because those are used to render initial highlights
    private string trueText;

    public void Setup()
    {
        // Clear all highlights and reset the true text to the written text
        highlights.Clear();
        trueText = text;

        // Set start and end indices
        int start = -1;
        // Previous brace found
        char prevBrace = ' ';

        for(int i = 0; i < trueText.Length; i++)
        {
            if(trueText[i] == '{')
            {
                // If previous brace also opened, then we have invalid syntax
                if(prevBrace == '{')
                {
                    ReportInitialHighlightError(i, "an opening curly brace", "a closing curly brace");
                    return;
                }

                // Set start index and previous brace found
                start = i;
                prevBrace = '{';

                // Remove the curly brace
                trueText = trueText.Remove(i, 1);
                i--;
            }
            else if(trueText[i] == '}')
            {
                // If previous brace also closed, we have invalid syntax
                if(prevBrace == '}' || prevBrace == ' ')
                {
                    ReportInitialHighlightError(i, "a closing curly brace", "an opening curly brace");
                    return;
                }

                // Remove the curly brace
                trueText = trueText.Remove(i, 1);
                i--;

                // Add the highlight now that we found the closing brace
                highlights.Add(new ResearchEncyclopediaArticleHighlight(start, i));
                prevBrace = '}';
            }
        }
    }

    public void RequestHighlight(int start, int end)
    {
        ResearchEncyclopediaArticleHighlight requestedHighlight = new ResearchEncyclopediaArticleHighlight(start, end);

        // Check for overlap with any of the existing highlights
        for(int i = 0; i < highlights.Count; i++)
        {
            // If there is overlap, then combine them
            if(highlights[i].Overlap(requestedHighlight))
            {
                highlights[i] = highlights[i].Combine(requestedHighlight);
                return;
            }
        }

        // If the requested highlight did not overlap any other highlights,
        // then append it to the end and re-sort the highlights
        highlights.Add(requestedHighlight);
        highlights.Sort();
    }

    private void ReportInitialHighlightError(int index, string foundDescriptor, string expectedDescriptor)
    {
        // Set the start index either to the beginning of the string
        // or ten less than index, whichever is bigger
        int startIndex = Mathf.Max(0, index - 20);
        // Set length to 20 or the difference between the start index
        // and end of string, whichever is smaller
        int length = Mathf.Min(40, trueText.Length - startIndex);
        // Get the substring of the text
        string reportString = trueText.Substring(startIndex, length);

        // Apply the color red to the part of the tag that is invalid
        RichTextTag invalidBraceTag = new RichTextTag("color", "red");
        reportString = invalidBraceTag.Apply(reportString, index - startIndex, 1);

        // Add ellipses if this is not the start or end of the whole string
        if (startIndex > 0) reportString = "..." + reportString;
        if (startIndex + length < trueText.Length) reportString += "...";

        // Add double quotes around the report string
        reportString = "\"" + reportString;
        reportString += "\"";

        Debug.LogWarning("Found " + foundDescriptor + " where " + expectedDescriptor + " was expected\n" +
            "\tArticle: " + id.ToString() + "\n" +
            "\tPosition: " + reportString + "\n");
    }
}

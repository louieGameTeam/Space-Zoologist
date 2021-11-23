using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEncyclopediaArticleData : NotebookDataModule
{
    #region Public Properties
    public List<TextHighlight> Highlights => highlights;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of highlights in this encyclopedia article")]
    private List<TextHighlight> highlights = new List<TextHighlight>();
    #endregion

    #region Constructors
    public ResearchEncyclopediaArticleData(NotebookConfig config, ResearchEncyclopediaArticleConfig articleConfig) : base(config)
    {
        // Get the text
        string text = articleConfig.RawText;

        // Set start and end indices
        int start = -1;
        // Previous brace found
        char prevBrace = ' ';

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '{')
            {
                // If previous brace also opened, then we have invalid syntax
                if (prevBrace == '{')
                {
                    ReportInitialHighlightError(i, text, "an opening curly brace", "a closing curly brace", articleConfig);
                    return;
                }

                // Set start index and previous brace found
                start = i;
                prevBrace = '{';

                // Remove the curly brace
                text = text.Remove(i, 1);
                i--;
            }
            else if (text[i] == '}')
            {
                // If previous brace also closed, we have invalid syntax
                if (prevBrace == '}' || prevBrace == ' ')
                {
                    ReportInitialHighlightError(i, text, "a closing curly brace", "an opening curly brace", articleConfig);
                    return;
                }

                // Remove the curly brace
                text = text.Remove(i, 1);

                // Add the highlight now that we found the closing brace
                highlights.Add(new TextHighlight(start, i));
                prevBrace = '}';
            }
        }
    }
    #endregion

    #region Public Methods
    public void RequestHighlightAdd(int start, int end)
    {
        // Add the highlight to the list and sort it
        TextHighlight requestedHighlight = new TextHighlight(start, end);
        highlights.Add(requestedHighlight);
        highlights.Sort();

        // Clean the highlights
        int i = 0;

        // Loop until we check up to (but not including) the last highlight
        while (i < highlights.Count - 1)
        {
            // If this highlight overlaps the next one, we combine them and remove the next one
            if (highlights[i].Overlap(highlights[i + 1]))
            {
                highlights[i] = highlights[i].Union(highlights[i + 1]);
                highlights.RemoveAt(i + 1);

                // We continue without incrementing because we need to check this same highlight again
                // just in case it contained multiple highlights
                continue;
            }
            else i++;
        }
    }
    public void RequestHighlightRemove(int start, int end)
    {
        TextHighlight negator = new TextHighlight(start, end);

        int i = 0;
        while (i < highlights.Count)
        {
            // Negate the current highlight
            List<TextHighlight> negation = highlights[i].Negate(negator);

            // Remove the highlight and replace it with the results of the negation
            highlights.RemoveAt(i);
            highlights.InsertRange(i, negation);

            // Advance past the negation results just added
            i += negation.Count;
        }
    }
    #endregion

    #region Private Methods
    private void ReportInitialHighlightError(int index, string text, string foundDescriptor, string expectedDescriptor, ResearchEncyclopediaArticleConfig config)
    {
        // Set the start index either to the beginning of the string
        // or ten less than index, whichever is bigger
        int startIndex = Mathf.Max(0, index - 20);
        // Set length to 20 or the difference between the start index
        // and end of string, whichever is smaller
        int length = Mathf.Min(40, text.Length - startIndex);
        // Get the substring of the text
        string reportString = text.Substring(startIndex, length);

        // Apply the color red to the part of the tag that is invalid
        RichTextTag invalidBraceTag = new RichTextTag("color", "red");
        reportString = invalidBraceTag.Apply(reportString, index - startIndex, 1);

        // Add ellipses if this is not the start or end of the whole string
        if (startIndex > 0) reportString = "..." + reportString;
        if (startIndex + length < text.Length) reportString += "...";

        // Add double quotes around the report string
        reportString = "\"" + reportString;
        reportString += "\"";

        Debug.LogWarning("Found " + foundDescriptor + " where " + expectedDescriptor + " was expected\n" +
            "\tArticle: " + config.ID.ToString() + "\n" +
            "\tPosition: " + reportString + "\n");
    }
    #endregion

}

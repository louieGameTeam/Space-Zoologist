using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHighlightingScheduler : MonoBehaviour
{
    #region Private Properties
    // This lazy loading property covers the accidental case 
    // where the highlight is made a child of an object that is destroyed
    // (such as when it highlights a TMP_Dropdown option)
    private TutorialHighlight Highlight
    {
        get
        {
            if(!highlight)
            {
                highlight = Instantiate(highlightPrefab);
            }
            return highlight;
        }
    }
    #endregion

    #region Private Fields
    private TutorialHighlight highlightPrefab;
    // Current highlight being used
    private TutorialHighlight highlight;
    private List<ConditionalHighlight> activeHighlights = new List<ConditionalHighlight>();
    #endregion

    #region Public Methods
    public void Setup(TutorialHighlight highlightPrefab)
    {
        this.highlightPrefab = highlightPrefab;
    }    

    public void SetHighlights(params ConditionalHighlight[] highlights)
    {
        activeHighlights.Clear();
        activeHighlights.AddRange (highlights);
    }

    public void ClearHighlights()
    {
        activeHighlights.Clear();
        Highlight.StopAnimating();
    }
    #endregion

    #region Monobehaviour Messages
    private void Update()
    {
        bool anyHighlightsActive = false;
        int index = 0;

        while(!anyHighlightsActive && index < activeHighlights.Count)
        {
            anyHighlightsActive |= activeHighlights[index].Target(Highlight);
            index++;
        }

        // Disable highlight when no highlights are active 
        // and enable it when some highlights are active
        Highlight.Root.gameObject.SetActive(anyHighlightsActive);
    }
    #endregion
}

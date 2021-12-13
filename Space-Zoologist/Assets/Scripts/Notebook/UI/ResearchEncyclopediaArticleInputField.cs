using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using TMPro;

public class ResearchEncyclopediaArticleInputField : NotebookUIChild, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Public Typedefs
    // So that the event appears in the editor
    [System.Serializable]
    public class IntIntEvent : UnityEvent<int, int> { }
    #endregion

    #region Public Properties
    public bool IsHighlighting => highlightPicker.FirstValuePicked;
    // Public accessors of private data
    public IntIntEvent OnHighlightConfirm => onHighlightConfirm;
    #endregion

    #region Private Properties
    private bool ArticleExists => articleConfig != null && articleData != null;
    #endregion

    #region Private Editor Fields
    // Private editor data
    [SerializeField]
    [Tooltip("Text field used to display the encyclopedia article")]
    private TMP_InputField textField;
    [SerializeField]
    [Tooltip("Parent of the image objects for this article")]
    private RectTransform imageParent;
    [SerializeField]
    [Tooltip("Prefab instantiated to display the encyclopedia images")]
    private ImagePreviewManager imagePrefab;
    [SerializeField]
    [Tooltip("Group of toggles used to determine whether we are highlighting or not")]
    private BoolToggleGroupPicker highlightPicker;
    [SerializeField]
    [Tooltip("Texture of the cursor while highlighting")]
    private CursorTexture highlightAddTexture;
    [SerializeField]
    [Tooltip("Texture of the cursor while removing highlights")]
    private CursorTexture highlightRemoveTexture;
    [SerializeField]
    [Tooltip("List of tags used to render highlighted encyclopedia article text")]
    private List<RichTextTag> highlightTags;
    [SerializeField]
    [Tooltip("Event invoked when the user finishes dragging")]
    private IntIntEvent onHighlightConfirm;
    #endregion

    #region Private Fields
    // Reference to the encyclopedia article that is currently being rendered,
    // if "null" no article is rendered
    private ResearchEncyclopediaArticleConfig articleConfig;
    private ResearchEncyclopediaArticleData articleData;
    #endregion

    #region UI Events
    public void OnEndDrag(PointerEventData data)
    {
        if(articleConfig != null)
        {
            // Use selection position on the input field to determine position of highlights
            int start = textField.selectionAnchorPosition;
            int end = textField.selectionFocusPosition;

            // If selection has no length, exit the function
            if (start == end) return;

            // If start is bigger than end, swap them
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            // Add/remove highlight depending on the state of the toggle
            if (highlightPicker.FirstValuePicked) articleData.RequestHighlightAdd(start, end);
            else articleData.RequestHighlightRemove(start, end);

            // Udpate the text for this article
            UpdateArticleDisplay();

            // Deactivate the input field
            textField.DeactivateInputField(true);
        }
    }
    // On pointer enter, set the correct cursor
    public void OnPointerEnter(PointerEventData data)
    {
        if (highlightPicker.FirstValuePicked) highlightAddTexture.SetCursor();
        else highlightRemoveTexture.SetCursor();
    }
    // On pointer exit, restore the default cursor
    public void OnPointerExit(PointerEventData data)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    #endregion

    #region Public Methods
    public void UpdateArticle(ResearchEncyclopediaArticleConfig config, ResearchEncyclopediaArticleData data)
    {
        articleConfig = config;
        articleData = data;
        UpdateArticleDisplay();
    }
    public void UpdateArticleDisplay()
    {
        // If an article was given, set the text with the highlights
        if (articleConfig != null) 
        { 
            textField.text = RichEncyclopediaArticleText();

            // Destroy all images
            foreach (Transform child in imageParent) Destroy(child.gameObject);
            // Create an image object for each sprite
            foreach (Sprite sprite in articleConfig.Sprites)
            {
                ImagePreviewManager preview = Instantiate(imagePrefab, imageParent);
                preview.BaseImage.sprite = sprite;
            }
        }
        // No article given implies this encyclopedia has no entries
        else textField.text = "<color=#aaa>This encyclopedia has no entries</color>";
    }
    public string RichEncyclopediaArticleText()
    {
        string richText = articleConfig.Text;
        int indexAdjuster = 0;    // Adjust the index for each highlight
        int indexIncrementer = 0; // Length of all the tags used in each highlight

        // Compute the index incrementer by incrementing tag lengths
        foreach (RichTextTag tag in highlightTags)
        {
            indexIncrementer += tag.Length;
        }
        // Go through all highlights
        foreach (TextHighlight highlight in articleData.Highlights)
        {
            richText = RichTextTag.ApplyMultiple(highlightTags, richText, highlight.Start + indexAdjuster, highlight.Length);
            // Increase the global index adjuster
            indexAdjuster += indexIncrementer;
        }

        return richText;
    }
    #endregion
}

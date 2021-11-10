using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Click on the given image to enlarge the image
/// as an overlay over all elements in the root canvas
/// </summary>
public class ImagePreviewManager : MonoBehaviour
{
    #region Public Properties
    public Image BaseImage => baseImage;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the image to enlarge when clicked")]
    private Image baseImage;
    [SerializeField]
    [Tooltip("Reference to the button that causes the button to become enlarged")]
    private Button previewButton;
    [SerializeField]
    [Tooltip("Reference to the image prefab instantiated and used as a preview for this image")]
    private ImagePreview previewPrefab;
    #endregion

    #region Private Fields
    private ImagePreview activePreview;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        previewButton.onClick.AddListener(CreatePreview);   
    }
    #endregion

    #region Private Methods
    private void CreatePreview()
    {
        if(!activePreview)
        {
            // Try to get a canvas in parent objects
            Canvas root = GetComponentInParent<Canvas>();

            // Make sure that we have a canvas in a parent somewhere
            if(root)
            {
                // Get the root canvas in this object
                root = root.rootCanvas;

                // Instantiate a preview as a child of the root
                activePreview = Instantiate(previewPrefab, root.transform);
                activePreview.Setup(baseImage.sprite);
            }
        }
    }
    #endregion
}

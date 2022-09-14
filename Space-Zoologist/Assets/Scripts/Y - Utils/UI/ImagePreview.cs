using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePreview : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Root object of the image preview")]
    private GameObject root;
    [SerializeField]
    [Tooltip("Reference to the image used to preview the sprite")]
    private Image image;
    [SerializeField]
    [Tooltip("Buttons used to close the image preview")]
    private Button[] closeButtons;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        foreach(Button button in closeButtons)
        {
            button.onClick.AddListener(RemovePreview);
        }
    }
    #endregion

    #region Public Methods
    public void Setup(Sprite sprite)
    {
        image.sprite = sprite;
    }
    #endregion

    #region Private Methods
    private void RemovePreview()
    {
        Destroy(root);
    }
    #endregion
}

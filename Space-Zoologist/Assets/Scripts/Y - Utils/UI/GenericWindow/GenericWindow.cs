using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GenericWindow : MonoBehaviour
{
    #region Public Properties
    public RectTransform Window => window;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform for the window that shrinks in and out of view")]
    private RectTransform window;

    [Space]

    [SerializeField]
    [Tooltip("Used to display the text in the window")]
    private TextMeshProUGUI messageText;
    [SerializeField]
    [Tooltip("Primary button in the window")]
    private GenericButton primaryButton;
    [SerializeField]
    [Tooltip("Secondary button in the window")]
    private GenericButton secondaryButton;
    #endregion

    #region Private Fields
    private static string defaultPrefabName => nameof(GenericWindow);
    private static string defaultPrefabPath => defaultPrefabName;
    #endregion

    #region Public Methods
    public void Setup(GenericWindowData data)
    {
        messageText.tag = data.Message;
        primaryButton.Setup(data.PrimaryButtonData);

        // Enable-Disable secondary button based on if it has a secondary button
        secondaryButton.Button.gameObject.SetActive(data.HasSecondaryButton);

        // Setup the secondary button if needed
        if (data.HasSecondaryButton) secondaryButton.Setup(data.SecondaryButtonData);
    }
    public static GenericWindow InstantiateFromResource(Transform parent, string prefabPath = null)
    {
        if (string.IsNullOrWhiteSpace(prefabPath)) prefabPath = defaultPrefabPath;
        return ResourcesExtensions.InstantiateFromResources<GenericWindow>(prefabPath, parent);
    }
    #endregion
}

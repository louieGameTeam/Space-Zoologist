using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectEnclosureUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Component used to display the name of the level")]
    private TextMeshProUGUI title;
    [SerializeField]
    [Tooltip("Component used to display the image of the level")]
    private Image image;
    [SerializeField]
    [Tooltip("Button used to take the player to this level enclosure")]
    private Button button;
    #endregion

    #region Private Fields
    private LevelData enclosure;
    #endregion

    #region Public Methods
    public void Setup(LevelData enclosure)
    {
        // Set this enclosure to the one given
        this.enclosure = enclosure;

        // Set the title and image
        title.text = enclosure.Level.Name;
        image.sprite = enclosure.Level.Sprite;

        // Load the level when the button is clicked
        // TODO: disable the button if this level is not all the way finished
        button.onClick.AddListener(LoadLevel);
    }
    public void LoadLevel() => LevelDataLoader.LoadLevel(enclosure);
    #endregion
}

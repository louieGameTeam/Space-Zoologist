using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The verbose inspector takes the same selection as the in-game inspector
/// and displays a lot more information about it.  Great for debugging the game
/// </summary>
public class VerboseInspector : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("If true, this verbose inspector tries to find an inspector " +
        "in the scene to connect to as soon as the scene begins")]
    private bool connectOnAwake = false;
    [SerializeField]
    [Tooltip("Text used to display the tile data currently selected by the inspector")]
    private TextMeshProUGUI tileDataText;
    #endregion

    #region Private Fields
    private Inspector inspector;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        if (connectOnAwake)
        {
            ConnectInspector(FindObjectOfType<Inspector>());
        }
    }
    #endregion

    #region Public Methods
    public void ConnectInspector(Inspector inspector)
    {
        // Prevent re-connecting the same inspector
        if (this.inspector != inspector)
        {
            // If an inspector was previously connected, 
            // then remove this script's listener from it
            if (this.inspector)
            {
                this.inspector.SelectionChangedEvent.RemoveListener(UpdateText);
            }

            // Listen for the selection changed event on the new inspector
            if (inspector)
            {
                inspector.SelectionChangedEvent.AddListener(UpdateText);
            }

            // Set this inspector and update the text
            this.inspector = inspector;
            UpdateText();
        }
    }
    #endregion

    #region Private Methods
    private void UpdateText()
    {
        GameManager gameManager = GameManager.Instance;

        // Check if there is a game manager and a connected inspector
        if(gameManager && inspector)
        {
            // Get the tile data at the inspector's position and display all the data as a JSON
            GridSystem.TileData tileData = gameManager.m_gridSystem.GetTileData(inspector.selectedPosition);
            tileDataText.text = JsonUtility.ToJson(tileData, true);
        }
        else
        {
            tileDataText.text = "No inspector connected";
        }
    }
    #endregion
}

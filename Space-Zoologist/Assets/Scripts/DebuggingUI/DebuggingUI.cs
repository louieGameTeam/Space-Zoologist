using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebuggingUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Button that activates debugging UI")]
    private KeyCode activationCode = KeyCode.BackQuote;
    [SerializeField]
    [Tooltip("Main panel that appears / disappears by pressing the back quote button")]
    private GameObject mainPanel;
    [SerializeField]
    [Tooltip("Reference to the button that lets the player instantly win the game")]
    private Button winButton;
    [SerializeField]
    [Tooltip("Button that unlocks all levels when clicked")]
    private Button toggleLevelOverrideButton;
    [SerializeField]
    [Tooltip("Button that toggles the verbose inspector")]
    private Button toggleVerboseInspectorButton;
    [SerializeField]
    [Tooltip("Reference to a verbose inspector")]
    private VerboseInspector verboseInspector;
    #endregion

    #region Private Fields
    private static DebuggingUI instance;
    private static readonly string prefabPath = nameof(DebuggingUI);
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        mainPanel.SetActive(false);
        winButton.onClick.AddListener(Win);
        toggleLevelOverrideButton.onClick.AddListener(ToggleLevelSelectOverride);
        toggleVerboseInspectorButton.onClick.AddListener(ToggleVerboseInspector);
    }
    private void Update()
    {
        if (Input.GetKeyDown(activationCode))
        {
            mainPanel.SetActive(!mainPanel.activeInHierarchy);
        }
    }
    #endregion

    #region Loading Methods
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CreateDebuggingUI()
    {
        if (!instance)
        {
            instance = ResourcesExtensions.InstantiateFromResources<DebuggingUI>(prefabPath, null);
            DontDestroyOnLoad(instance.gameObject);

            // Update the ui each time that a new scene is loaded
            SceneManager.sceneLoaded += instance.UpdateUI;
        }
    }
    #endregion

    #region Event Listeners
    private void UpdateUI(Scene arg0, LoadSceneMode arg1)
    {
        // Store the instance of the game manager
        GameManager gameManager = GameManager.Instance;

        // Win button only interactable if a game manager exists in this scene
        winButton.interactable = gameManager;

        // Can only override levels if you found a level navigator
        LevelNavigator navigator = FindObjectOfType<LevelNavigator>();
        toggleLevelOverrideButton.interactable = navigator;

        // Verbose inspector only interactable if there is a game manager
        toggleVerboseInspectorButton.interactable = gameManager;

        // If a game manager exists then connect the verbose inspector
        // to the in game inspector
        if (gameManager)
        {
            verboseInspector.ConnectInspector(gameManager.m_inspector);
        }
        // Otherwise disable the verbose inspector
        else SetVerboseInspectorActive(false);
    }
    private void Win()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.DebugWin();
        }
    }
    private void ToggleLevelSelectOverride()
    {
        LevelNavigator navigator = FindObjectOfType<LevelNavigator>();

        // If the navigator exists then toggle its override
        if (navigator)
        {
            LevelID maxID = new LevelID(LevelDataLoader.MaxLevel() + 1, 1);
            navigator.ToggleOverride(maxID);
        }
    }
    private void ToggleVerboseInspector()
    {
        SetVerboseInspectorActive(!verboseInspector.gameObject.activeInHierarchy);
    }
    #endregion

    #region Private Fields
    private void SetVerboseInspectorActive(bool active)
    {
        verboseInspector.gameObject.SetActive(active);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebuggingUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Button sequence that activates debugging UI")]
    // This is the tilde key (~ or `) followed by the konami code (minus "start" because it doesn't exist :P)
    private KeyCode[] activationSequence = new KeyCode [] {
        KeyCode.BackQuote, 
        KeyCode.UpArrow, KeyCode.UpArrow,
        KeyCode.DownArrow, KeyCode.DownArrow, 
        KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow, 
        KeyCode.B, KeyCode.A
    };
    [SerializeField]
    [Tooltip ("Time allowed between keystrokes before activation is aborted")]
    private float activationSequenceTimerForgiveness = 4.0f;
    [SerializeField]
    [Tooltip("Main panel that appears / disappears by pressing the back quote button")]
    private GameObject mainPanel = null;
    [SerializeField]
    [Tooltip("Reference to the button that lets the player instantly win the game")]
    private Button winButton = null;
    [SerializeField]
    [Tooltip("Button that unlocks all levels when clicked")]
    private Button toggleLevelOverrideButton = null;
    [SerializeField]
    [Tooltip("Button that toggles the verbose inspector")]
    private Button toggleVerboseInspectorButton = null;
    [SerializeField]
    [Tooltip("Reference to a verbose inspector")]
    private VerboseInspector verboseInspector = null;
    #endregion

    #region Private Fields
    private static DebuggingUI instance;
    private static readonly string prefabPath = nameof(DebuggingUI);
    int activatedSequenceIndex = 0;
    float activationSequenceTimer = 0;
    bool active = false;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        mainPanel.SetActive(false);
        winButton.onClick.AddListener(Win);
        toggleLevelOverrideButton.onClick.AddListener(ToggleLevelSelectOverride);
        toggleVerboseInspectorButton.onClick.AddListener(ToggleVerboseInspector);
#if UNITY_EDITOR
        mainPanel.SetActive (true);
        active = true;
#endif
    }

    private void Update()
    {
        if (active) {
            if (Input.GetKeyDown (activationSequence [0])) {
                mainPanel.SetActive (false);
                active = false;
            }
        } else {
            if (Input.GetKeyDown (activationSequence [activatedSequenceIndex])) {
                activatedSequenceIndex++;
                activationSequenceTimer = activationSequenceTimerForgiveness;

                if (activatedSequenceIndex >= activationSequence.Length) {
                    activatedSequenceIndex = 0;
                    mainPanel.SetActive (true);
                    active = true;
                }
            }

            if (activationSequenceTimer <= 0) {
                activatedSequenceIndex = 0;
            }

            activationSequenceTimer = Mathf.Clamp (activationSequenceTimer - Time.deltaTime, 0, activationSequenceTimerForgiveness);
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
            LevelID maxID = new LevelID(LevelDataLoader.MaxLevel() + 1, 4);
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

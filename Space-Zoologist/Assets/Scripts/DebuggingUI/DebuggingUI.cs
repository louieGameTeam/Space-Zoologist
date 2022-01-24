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
            SceneManager.sceneLoaded += UpdateUI;
        }
    }
    #endregion

    #region Event Listeners
    private static void UpdateUI(Scene arg0, LoadSceneMode arg1)
    {
        instance.winButton.interactable = GameManager.Instance;

        // Can only override levels if you found a level navigator
        LevelNavigator navigator = FindObjectOfType<LevelNavigator>();
        instance.toggleLevelOverrideButton.interactable = navigator;
    }
    private static void Win()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.DebugWin();
        }
    }
    private static void ToggleLevelSelectOverride()
    {
        LevelNavigator navigator = FindObjectOfType<LevelNavigator>();

        // If the navigator exists then toggle its override
        if (navigator)
        {
            LevelID maxID = new LevelID(LevelDataLoader.MaxLevel() + 1, 1);
            navigator.ToggleOverride(maxID);
        }
    }
    #endregion
}

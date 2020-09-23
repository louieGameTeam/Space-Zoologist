using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject GameOverHUD = default;
    [SerializeField] GameObject IngameUI = default;
    [SerializeField] GameObject OptionsButton = default;
    [SerializeField] Button RestartButton = default;
    [SerializeField] PauseManager PauseManager = default;
    [SerializeField] SceneNavigator SceneNavigator = default;

    private void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.GameOver, this.HandleGameOver);
        this.RestartButton.onClick.AddListener(() => {this.SceneNavigator.LoadLevel(this.SceneNavigator.RecentlyLoadedLevel);});
    }

    public void HandleGameOver()
    {
        this.PauseManager.Pause();
        this.GameOverHUD.SetActive(true);
        this.IngameUI.SetActive(false);
        this.OptionsButton.SetActive(false);
    }
}

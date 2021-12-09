using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Data for the window displayed when all objectives finish, before final NPC dialogue")]
    private GenericWindowData objectiveFinishedWindow;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        //EventManager.Instance.SubscribeToEvent(EventType.GameOver, )
    }
    #endregion

    #region Private Methods
    private void OnGameOver()
    {

    }
    #endregion
}

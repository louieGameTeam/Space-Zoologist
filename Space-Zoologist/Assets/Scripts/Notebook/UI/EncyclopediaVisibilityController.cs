using UnityEngine;

public class EncyclopediaVisibilityController : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private GenericFoldout EncyclopediaBackgroundFill;
    [SerializeField] private ResearchEncyclopediaUI EncyclopediaUI;
    #endregion

    #region Monobehaviour Callbacks
    // Event subscription should occur here
    private void Awake()
    {
        EventManager.Instance.SubscribeToEvent(EventType.ReportBackStart, OnReportBackStart);
        EventManager.Instance.SubscribeToEvent(EventType.ReportBackEnd, OnReportBackEnd);
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Function called when report back quiz starts, prevents player from accessing encyclopedia
    /// while still allowing them to open the notebook
    /// </summary>
    private void OnReportBackStart()
    {
        GameManager.Instance.m_menuManager.ToggleUISingleButton(true, "NotebookButton");
        EncyclopediaBackgroundFill.gameObject.SetActive(false);
        EncyclopediaUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// Function called when report back quiz has concluded, allows player to access encyclopedia again
    /// </summary>
    private void OnReportBackEnd()
    {
        EncyclopediaBackgroundFill.gameObject.SetActive(true);
        EncyclopediaUI.gameObject.SetActive(true);
    }
    #endregion
}
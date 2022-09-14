using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginHandler : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip ("Data for the window displayed when a login is requested")]
    private GenericWindow loginWindow;
    #endregion

    #region Private Fields
    // Input field for user email
    private TMP_InputField emailInput;
    #endregion

    #region Public Methods
    public void RequestLogin () {
        OpenWindow (loginWindow, () => SubmitLogin (), () => SceneManager.LoadScene ("LevelMenu"));
    }

    public void SubmitLogin () {
        print (emailInput.text);
        // TODO: Encrypt and store email from emailInput
        SummaryManager summaryManager = (SummaryManager)FindObjectOfType(typeof(SummaryManager));
        summaryManager.SetUpCurrentSummaryTrace(emailInput.text);
        SceneNavigator.LoadScene ("LevelMenu");
    }
    #endregion

    #region Private Methods
    private void OpenWindow (GenericWindow window, UnityAction primaryAction, UnityAction secondaryAction = null) {
        // Instantiate the window under the main canvas
        Transform canvas = GameObject.FindWithTag ("MainCanvas").transform;
        window = Instantiate (window, canvas);

        // Setup the primary and secondary action of the window
        window.AddPrimaryAction (primaryAction);
        if (secondaryAction != null) {
            window.AddSecondaryAction (secondaryAction);
        }

        // Assign email input field
        emailInput = window.gameObject.GetComponentInChildren<TMP_InputField> ();

        // Open the window
        window.Open ();
    }
    #endregion
}

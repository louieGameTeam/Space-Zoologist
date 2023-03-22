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
    private GenericWindow loginWindow = null;
    #endregion

    #region Private Fields
    // Input field for user email
    private TMP_InputField[] userInformationInput = new TMP_InputField[4];
    #endregion

    #region Public Methods
    public void RequestLogin () {
        OpenWindow (loginWindow, () => SubmitLogin (), () => SkipLogin ());
    }

    public void SubmitLogin () {
        for (int i = 0; i < 4; i++) {
            if (userInformationInput[i].text == "") {
                return;
            }
            print (i + " " + userInformationInput[i].text);
        }
        SummaryManager summaryManager = (SummaryManager)FindObjectOfType(typeof(SummaryManager));
        summaryManager.SetUpCurrentSummaryTrace(userInformationInput);
        SceneNavigator.LoadScene ("LevelMenu");
    }

    public void SkipLogin () {
        print("Skip Login");

        // Add in default user information
        userInformationInput[0].text = "default_user";
        userInformationInput[1].text = "default_first_name";
        userInformationInput[2].text = "default_last_name";
        userInformationInput[3].text = "default_class_ID";

        SummaryManager summaryManager = (SummaryManager)FindObjectOfType(typeof(SummaryManager));
        summaryManager.SetUpCurrentSummaryTrace(userInformationInput);
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
        userInformationInput = window.gameObject.GetComponentsInChildren<TMP_InputField> ();

        // Open the window
        window.Open ();
    }
    #endregion
}

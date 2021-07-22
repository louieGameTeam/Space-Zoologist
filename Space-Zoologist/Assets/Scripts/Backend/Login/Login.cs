using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private string loginEndpoint = "http://127.0.0.1:13756/account/login";
    [SerializeField] private string createEndpoint = "http://127.0.0.1:13756/account/create";
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Button signinButton;
    [SerializeField] private Button createButton;
    
    public void OnSignInClick()
    {
        alertText.text = "Signing in...";
        ActivateButtons(false);
        StartCoroutine(TrySignin());
    }

    public void OnCreateClick()
    {
        alertText.text = "Creating account...";
        ActivateButtons(false);
        StartCoroutine(TryCreate());
    }

    private IEnumerator TrySignin()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (username.Length < 3 || username.Length > 24)
        {
            alertText.text = "Invalid username.";
            ActivateButtons(true);
            yield break;
        }

        if (password.Length < 3 || password.Length > 24)
        {
            alertText.text = "Invalid password.";
            ActivateButtons(true);
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest request = UnityWebRequest.Post(loginEndpoint, form);
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;
            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.responseCode == 200)
        {
            if (request.downloadHandler.text != "Invalid credentials.") // login success?
            {
                ActivateButtons(false);
                Account returnedAccount = JsonUtility.FromJson<Account>(request.downloadHandler.text);
                alertText.text = $"{returnedAccount._id} Welcome " + returnedAccount.username + "!";
            } else
            {
                alertText.text = "Invalid credentials.";
                ActivateButtons(true);
            }
        } else
        {
            alertText.text = "Error connecting to server.";
            ActivateButtons(true);
        }

        yield return null;
    }

    private IEnumerator TryCreate()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (username.Length < 3 || username.Length > 24)
        {
            alertText.text = "Invalid username.";
            ActivateButtons(true);
            yield break;
        }

        if (password.Length < 3 || password.Length > 24)
        {
            alertText.text = "Invalid password.";
            ActivateButtons(true);
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest request = UnityWebRequest.Post(createEndpoint, form);
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;
            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.responseCode == 200)
        {
            if (request.downloadHandler.text != "Invalid credentials." && request.downloadHandler.text != "Username is already in use.") // login success?
            {
                Account returnedAccount = JsonUtility.FromJson<Account>(request.downloadHandler.text);
                alertText.text = "Account has been created.";
            } else
            {
                alertText.text = "Username is already in use.";
            }
        } else
        {
            alertText.text = "Error connecting to server.";
        }

        ActivateButtons(true);

        yield return null;
    }

    private void ActivateButtons(bool toggle)
    {
        signinButton.interactable = toggle;
        createButton.interactable = toggle;
    }
}

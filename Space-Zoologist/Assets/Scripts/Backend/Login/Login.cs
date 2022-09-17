using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    const string devLoginEndpoint = "http://127.0.0.1:13756/account/login";
    const string devCreateEndpoint = "http://127.0.0.1:13756/account/create";
    const string prodLoginEndpoint = "http://spacezoologist.herokuapp.com/account/login";
    const string prodCreateEndpoint = "http://spacezoologist.herokuapp.com/account/create"; 
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

        using (UnityWebRequest request = UnityWebRequest.Post(devLoginEndpoint, form))
        {
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
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                if (response.code == 0) // login success?
                {
                    ActivateButtons(false);
                    alertText.text = "Welcome!";
                } else
                {
                    switch(response.code)
                    {
                        case 1:
                            alertText.text = "Invalid credentials.";
                            ActivateButtons(true);
                            break;
                        default:
                            alertText.text = "Corruption detected.";
                            ActivateButtons(false);
                            break;
                    }
                }
            } else
            {
                alertText.text = "Error connecting to server.";
                ActivateButtons(true);
            }

            yield return null;
        }
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

        using (UnityWebRequest request = UnityWebRequest.Post(devCreateEndpoint, form))
        {
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
                CreateResponse response = JsonUtility.FromJson<CreateResponse>(request.downloadHandler.text);
                if (response.code == 0) // login success?
                {
                    alertText.text = "Account has been created.";
                } else
                {
                    switch(response.code)
                    {
                        case 1:
                            alertText.text = "Invalid credentials.";
                            break;
                        case 2:
                            alertText.text = "Username is already in use.";
                            break;
                        default:
                            alertText.text = "Corruption detected.";
                            break;
                    }
                }
            } else
            {
                alertText.text = "Error connecting to server.";
            }

            ActivateButtons(true);

            yield return null;
        }
    }

    private void ActivateButtons(bool toggle)
    {
        signinButton.interactable = toggle;
        createButton.interactable = toggle;
    }
}

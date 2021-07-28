using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class SubmitPlayTrace : MonoBehaviour
{
    [SerializeField] private static string playtraceEndpoint = "http://127.0.0.1:13756/traces/playtrace";

    public static IEnumerator TrySubmitPlayTrace(string json)
    {
        var request = new UnityWebRequest(playtraceEndpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class SubmitSummaryTrace : MonoBehaviour
{
    [SerializeField] private static string prodSummarytraceEndpoint = "http://spacezoologist.herokuapp.com/traces/summarytrace/submit"; 
    [SerializeField] private static string devSummarytraceEndpoint = "http://127.0.0.1:13756/traces/summarytrace/submit";

    public static IEnumerator TrySubmitSummaryTrace(string json)
    {
        var request = new UnityWebRequest(prodSummarytraceEndpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
    }
}

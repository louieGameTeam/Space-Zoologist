using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class GetSummaryTrace : MonoBehaviour
{
    [SerializeField] private static string prodSummarytraceEndpoint = "http://spacezoologist.herokuapp.com/traces/summarytrace/get"; 
    [SerializeField] private static string devSummarytraceEndpoint = "http://127.0.0.1:13756/traces/summarytrace/get";

    public static IEnumerator TryGetSummaryTrace(string playerID, System.Action<SummaryTraceResponse> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerID", playerID);
        UnityWebRequest request = UnityWebRequest.Post(devSummarytraceEndpoint, form);
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
            SummaryTraceResponse response = JsonUtility.FromJson<SummaryTraceResponse>(request.downloadHandler.text);
            if (response.code == 0)
            {
                Debug.Log("Creating summary trace object from DB.");
            } else if (response.code == 1) {
                Debug.Log("Player ID not found in request body.");
            } else {
                Debug.Log("Player ID not found in DB.");
            }
            yield return null;
            callback(response);
        }

        yield return null;
        callback(null);

        Debug.Log("Status Code: " + request.responseCode);
    }
}

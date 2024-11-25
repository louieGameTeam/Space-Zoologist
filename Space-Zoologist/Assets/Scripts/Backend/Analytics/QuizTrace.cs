using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// An ad-hoc class meant to extract results from operations performed by code in the Quiz infrastructure. Much of this is centralizing and simplifying what is found in QuizInstance.cs (e.g. transforming into primitive data types so that it can be easily used by the backend/viewer).
[System.Serializable]
public class QuizTrace
{
    [SerializeField] private string playerID;
    [SerializeField] private int levelNumber;
    [SerializeField] private int enclosureNumber;
    [SerializeField] private int questionsAnswered;
    [SerializeField] private bool completed;
    [SerializeField] private string grade;
    [SerializeField] private int totalScore;
    [SerializeField] private int importantCategoriesScore;
    [SerializeField] private int unimportantCategoriesScore;

    public QuizTrace()
    {
        this.playerID = "";
        this.levelNumber = 0;
        this.enclosureNumber = 0;
        this.questionsAnswered = 0;
        this.completed = false;
        this.grade = "";
        this.totalScore = 0;
        this.importantCategoriesScore = 0;
        this.unimportantCategoriesScore = 0;
    }

    public QuizTrace(LevelID levelID, QuizInstance quizInstance)
    {
        this.playerID = SummaryManager.Instance.CurrentSummaryTrace.PlayerID;
        this.levelNumber = levelID.LevelNumber;
        this.enclosureNumber = levelID.EnclosureNumber;
        this.questionsAnswered = quizInstance.QuestionsAnswered;
        this.completed = quizInstance.Completed;
        this.grade = quizInstance.Grade.ToString();
        this.totalScore = quizInstance.ItemizedScore.TotalScore;
        this.importantCategoriesScore = quizInstance.ScoreInImportantCategories;
        this.unimportantCategoriesScore = quizInstance.ScoreInUnimportantCategories;
    }
}

public class QuizTraceManager : MonoBehaviour
{
    [SerializeField] private static string prodQuizTraceEndpoint = "https://spacezoologist.herokuapp.com/traces/quiztrace/submit"; 
    [SerializeField] private static string devQuizTraceEndpoint = "https://127.0.0.1:13756/traces/quiztrace/submit";

    public static IEnumerator TrySubmitQuizTrace(QuizTrace quizTrace)
    {
        string json = JsonUtility.ToJson(quizTrace);

        var request = new UnityWebRequest(devQuizTraceEndpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.chunkedTransfer = false;
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Request successfully sent");
        }
        request.Dispose();
    }
}

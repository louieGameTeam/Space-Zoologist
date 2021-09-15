using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor;
using UnityEngine.UI;


public class DialogueResponseManager : MonoBehaviour
{
	#region QuizResponse
	[SerializeField] List<QuizResponse> quizResponses = default;
	private int fScore = 0;
	private int tScore = 0;
	private int wScore = 0;

	public void EndOfQuiz()
	{
		ConversationManager conversationManager = FindObjectOfType<ConversationManager>();
		foreach (QuizResponse quizResponse in quizResponses)
        {
			if (tScore >= quizResponse.tScore && fScore >= quizResponse.fScore && wScore >= quizResponse.wScore)
            {
				conversationManager.StartConversation(quizResponse.NPCConversation);
				break;
			}
        }
	}

	public void LoadNextLevel(string level)
    {
		LevelDataLoader levelDataLoader = FindObjectOfType<LevelDataLoader>();
		levelDataLoader.LoadLevel(level);
    }

	public void ReloadLevel()
    {
		LevelDataLoader levelDataLoader = FindObjectOfType<LevelDataLoader>();
		levelDataLoader.ReloadLevel();
	}

	public void increaseFScore(int score)
    {
		fScore += score;
		Debug.Log("fscore: " + fScore);
	}

	public void increaseTScore(int score)
	{
		tScore += score;
		Debug.Log("tscore: " + tScore);
	}

	public void increaseWScore(int score)
	{
		wScore += score;
		Debug.Log("wscore: " + tScore);
	}
	#endregion
}

[System.Serializable]
public class QuizResponse
{
	[SerializeField] public int tScore = 0;
	[SerializeField] public int fScore = 0;
	[SerializeField] public int wScore = 0;
	[SerializeField] public DialogueEditor.NPCConversation NPCConversation;
}
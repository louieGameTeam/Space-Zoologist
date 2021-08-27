using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor;
using UnityEngine.UI;


public class DialogueResponseManager : MonoBehaviour
{
	/*
	 * 
	 * TODO create a quiz class that maps strings to number of correct. DialogueResponse will update the quiz class's
	 * number of correct (F + 1, F + 2) and quiz class will then update the specific dialogue once it has been reached.
	 */
	[SerializeField] List<QuizResponse> quizResponses = default;
	private ConversationManager conversationManager;
	private int fScore = 0;
	private int tScore = 0;
	private int wScore = 0;

	public void EndOfQuiz()
	{
		Debug.Log("check9ing quiz results");
		conversationManager = FindObjectOfType<ConversationManager>();
		foreach (QuizResponse quizResponse in quizResponses)
        {
			if (tScore >= quizResponse.tScore && fScore >= quizResponse.fScore && wScore >= quizResponse.wScore)
            {
				conversationManager.StartConversation(quizResponse.NPCConversation);
				break;
			}
        }
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
}

[System.Serializable]
public class QuizResponse
{
	[SerializeField] public int tScore = 0;
	[SerializeField] public int fScore = 0;
	[SerializeField] public int wScore = 0;
	[SerializeField] public DialogueEditor.NPCConversation NPCConversation;
}
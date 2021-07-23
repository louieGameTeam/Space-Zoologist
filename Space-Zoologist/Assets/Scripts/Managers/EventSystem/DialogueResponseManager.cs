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
	bool listening = false;
	List<string> listenedKeys;
	List<string> listenedSpecies;
	[SerializeField] List<QuizResponse> quizResponses = default;
	[SerializeField] Inspector inspector;
	[SerializeField] ConversationManager conversationManager;
	[SerializeField] SceneNavigator sceneNavigator = default;
	[SerializeField] string endOfQuizText = "Alright, give me a minute to process your answers…";
	List<UnityEvent> toTrigger; // Generic unity events to be triggered after response
	private int fScore = 0;
	private int tScore = 0;

	private GameObject lastPopulationReturned = null;

	private void Start()
	{
		Reset();
	}

	private void Reset()
	{
		// initialize variables
		listening = false;
		listenedKeys = new List<string>();
		listenedSpecies = new List<string>();
		toTrigger = new List<UnityEvent>();

		lastPopulationReturned = null;
	}

	public void ListenForKey(string k)
	{
		listenedKeys.Add(k);
		StartListening();
	}

	// Listening for checking animals & food
	public void ListenForSpecies(string speciesName)
	{
		listenedSpecies.Add(speciesName);
		StartListening();
	}


	/// <summary>
	/// Add an unity event that will be triggered after response.
	/// </summary>
	/// <param name="uEvent"></param>
	public void AddOnResponseEvent(UnityEvent uEvent)
	{
		toTrigger.Add(uEvent);
	}


	public void Update()
	{
		if (listening)
		{
			HandleListenedKeys();
			HandleInspectorClicked();
		}
		WatchForEndOfQuiz();
	}

	private void StartListening()
	{
		listening = true;
		conversationManager.FreezeConversation();
	}

	private void Responded()
	{
		print("responded");
		foreach (UnityEvent Event in toTrigger)
		{
			Event?.Invoke();
		}
		conversationManager.UnfreezeConversation();

		Reset();
	}

	private void HandleListenedKeys()
	{
		foreach (string k in listenedKeys)
		{
			if (Input.GetKeyDown(k))
			{
				Responded();
				return;
			}
		}
	}

	public void loadNextLevel(string levelName)
    {
		sceneNavigator.LoadLevel(levelName);
    }

	private void WatchForEndOfQuiz()
	{
		if (conversationManager.DialogueText.text == endOfQuizText)
		{
			foreach (QuizResponse quizResponse in quizResponses)
            {
				if (tScore == quizResponse.tScore && fScore == quizResponse.fScore)
                {
					conversationManager.StartConversation(quizResponse.NPCConversation);

				}
            }
		}
	}

	public void increaseFScore(int score)
    {
		fScore += score;
	}

	public void increaseTScore(int score)
	{
		tScore += score;
	}

	private void HandleInspectorClicked()
	{
		if (!inspector.IsInInspectorMode)
		{
			return;
		}
		GameObject PopulationReturned = inspector.GetAnimalSelected();
		if (PopulationReturned != null && PopulationReturned != lastPopulationReturned)
		{
			AnimalSpecies species = PopulationReturned.GetComponent<Population>().Species;
			foreach (string listened in listenedSpecies)
			{
				if (listened == species.SpeciesName)
				{
					Responded();
					break;
				}
			}
			lastPopulationReturned = PopulationReturned;
		}
	}
}

[System.Serializable]
public class QuizResponse
{
	[SerializeField] public int tScore = 0;
	[SerializeField] public int fScore = 0;
	[SerializeField] public DialogueEditor.NPCConversation NPCConversation;
}
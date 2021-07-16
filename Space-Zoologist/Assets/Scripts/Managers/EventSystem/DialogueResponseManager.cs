using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor;


public class DialogueResponseManager : MonoBehaviour
{
	bool listening = false;
	List<string> listenedKeys;
	List<string> listenedButtons;
	List<string> listenedUnityButtons;
	List<string> listenedSpecies;
	[SerializeField] Inspector inspector;
	[SerializeField] ConversationManager conversationManager;
	List<UnityEvent> toTrigger; // Generic unity events to be triggered after response

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
		listenedButtons = new List<string>();
		listenedUnityButtons = new List<string>();
		listenedSpecies = new List<string>();
		toTrigger = new List<UnityEvent>();

		lastPopulationReturned = null;
	}

	public void ListenForKey(string k)
	{
		listenedKeys.Add(k);
		StartListening();
	}


	//TODO Temporarily disabled to avoid confusion with UnityButton
	//public void ListenForButton(string button){
	//	listenedButtons.Add(button);
	//  StartListening();
	//}

	public void ListenForUnityButton(string buttonName)
	{
		listenedUnityButtons.Add(buttonName);
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


	// Most simple solution(){ have the interested unity buttons call this function with same 
	// button Name as the one in ListenForUnityButton()
	public void UnityButtonTriggered(string buttonName)
	{
		if (!listening) { return; }

		foreach (string button in listenedUnityButtons)
		{
			if (button == buttonName)
			{
				Responded();
				return;
			}
		}
	}

	public void Update()
	{
		if (listening)
		{
			//utilize Unity Input KeyDown & ButtonDown
			foreach (string k in listenedKeys)
			{
				if (Input.GetKeyDown(k))
				{
					Responded();
					return;
				}
			}

			foreach (string b in listenedButtons)
			{
				if (Input.GetButtonDown(b))
				{
					Responded();
					return;
				}
			}

			if (inspector.IsInInspectorMode)
			{
				GameObject PopulationReturned = inspector.GetAnimalSelected();
				if (PopulationReturned != null && PopulationReturned != lastPopulationReturned)
				{
					AnimalSpecies species = PopulationReturned.GetComponent<Population>().Species;
					foreach (string listened in listenedSpecies)
					{
						if (listened == species.SpeciesName)
						{
							Responded();
							return;
						}
					}
					lastPopulationReturned = PopulationReturned;
				}

			}
		}
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
}
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningSystem : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the canvas to create the warning window on")]
    private Canvas mainCanvas;
    [SerializeField]
    [Tooltip("Window to instantiate for the warning")]
    private GenericWindow warningWindowPrefab;
    #endregion

    #region Private Fields
    private GenericWindow warningWindow;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.NextDay, OnNextDay);

        // Create a warning window
        warningWindow = Instantiate(warningWindowPrefab, mainCanvas.transform);
        warningWindow.gameObject.SetActive(false);
    }
    #endregion

    #region Event Listeners
    private void OnNextDay()
    {
        PopulationManager population = GameManager.Instance.m_populationManager;
        List<AnimalSpecies> endangeredSpecies = new List<AnimalSpecies>();

        foreach(Population pop in population.Populations)
        {
            GrowthCalculator growth = pop.GrowthCalculator;

            // If the population is declining at a rate lower than -0.5f,
            // and it has not been marked as endangered already, then 
            // add it to the list of endangered species
            if (growth.populationIncreaseRate <= -0.5f && 
                !endangeredSpecies.Contains(pop.Species))
            {
                endangeredSpecies.Add(pop.species);
            }
        }

        // Signal a warning about the endangered species
        if (endangeredSpecies.Count > 0)
        {
            SignalWarning(endangeredSpecies);
        }
    }
    #endregion

    #region Private Methods
    private void SignalWarning(List<AnimalSpecies> endangeredSpecies)
    {
        // Setup the message
        string message = "Some animal populations are rapidly declining. Please improve their needs as soon as possible.";
        message += "\n\nEndangered species: ";
        message += string.Join(", ", endangeredSpecies.Select(animal => animal.SpeciesName));

        // Set the message text and open the window
        warningWindow.MessageText.text = message;
        warningWindow.Open();
    }
    #endregion
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningSystem : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the canvas to create the warning window on")]
    private Canvas mainCanvas = null;
    [SerializeField]
    [Tooltip("Window to instantiate for the warning")]
    private GenericWindow warningWindowPrefab = null;
    [SerializeField]
    [Tooltip ("Popup showing continuing population decline")]
    private GameObject warningPopup = null;
    [SerializeField]
    private float popupAnimationSpeed = 2;
    [SerializeField]
    private float popupAnimationHoldTime = 3;
    #endregion

    #region Private Fields
    private GenericWindow warningWindow;
    private bool justSignalledWarning = false;
    private Coroutine popupAnimator = null;
    
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
            if (growth.ChangeRate <= -0.5f && 
                !endangeredSpecies.Contains(pop.Species))
            {
                endangeredSpecies.Add(pop.species);
            }
        }

        // Signal a warning about the endangered species
        if (endangeredSpecies.Count > 0)
        {
            SignalWarning(endangeredSpecies);
            justSignalledWarning = true;
        } else {
            justSignalledWarning = false;
        }
    }
    #endregion

    #region Private Methods
    private void SignalWarning(List<AnimalSpecies> endangeredSpecies)
    {
        if (justSignalledWarning) {
            if (popupAnimator != null) {
                StopCoroutine (popupAnimator);
            }
            popupAnimator = StartCoroutine (SignalWarningPopup ());
        } else {
            // Setup the message
            string message = "Some animal populations are rapidly declining. Please improve their needs as soon as possible.";
            message += "\n\nEndangered species: ";
            message += string.Join (", ", endangeredSpecies.Select (animal => animal.ID.Data.Name.Get (ItemName.Type.Colloquial)));

            // Set the message text and open the window
            warningWindow.MessageText.text = message;
            warningWindow.Open ();
        }
    }

    IEnumerator SignalWarningPopup() {
        warningPopup.SetActive (true);

        for (float i = 0; i < 1; i += Time.deltaTime * popupAnimationSpeed) {
            warningPopup.transform.localScale = Vector3.one * i;
            yield return null;
        }

        yield return new WaitForSeconds (popupAnimationHoldTime);

        for (float i = 0; i < 1; i += Time.deltaTime * popupAnimationSpeed) {
            warningPopup.transform.localScale = Vector3.one * (1 - i);
            yield return null;
        }

        warningPopup.SetActive (false);
    }
    #endregion
}

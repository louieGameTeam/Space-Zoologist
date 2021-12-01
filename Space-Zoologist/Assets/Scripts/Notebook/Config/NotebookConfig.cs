using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NotebookConfig : ScriptableObject
{
    #region Public Properties
    public string Acronym => acronym;
    public ResearchConfig Research => research;
    public ObservationsConfig Observations => observations;
    public TestAndMetricsConfig TestAndMetrics => testAndMetrics;
    public NotebookTabScaffold TabScaffold => tabScaffold;
    public List<ItemID> InitiallyUnlockedItems => initiallyUnlockedItems;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Acronym that the player gets to spell out on the home page")]
    private string acronym = "ROCTM";
    
    [Space]
    
    [SerializeField]
    [WrappedProperty("researchEntryRegistry")]
    [Tooltip("Reference to the model holding all the player's research and info" +
        "about the different species, foods, and tiles")]
    private ResearchConfig research;
    [SerializeField]
    [Tooltip("Player observation notes")]
    private ObservationsConfig observations;
    [SerializeField]
    [Tooltip("Test and metrics that the player has taken")]
    private TestAndMetricsConfig testAndMetrics;

    [Space]

    [SerializeField]
    [Tooltip("Controls which tabs are available in what levels")]
    private NotebookTabScaffold tabScaffold;
    [SerializeField]
    [Tooltip("List of items that should be unlocked at the beginning of the game")]
    private List<ItemID> initiallyUnlockedItems;
    #endregion
}

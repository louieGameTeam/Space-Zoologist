using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Notebook/Notebook")]
public class Notebook : ScriptableObject
{
    // General notes for this notebook
    public string GeneralNotes { get; set; }

    [SerializeField]
    [Tooltip("Reference to the model holding all the player's research and info" +
        "about the different species, foods, and tiles")]
    private ResearchModel research;
}

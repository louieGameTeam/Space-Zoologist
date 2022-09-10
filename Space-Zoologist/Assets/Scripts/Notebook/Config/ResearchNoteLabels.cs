using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Notebook/ResearchNoteLabels")]
[System.Serializable]
public class ResearchNoteLabels : ScriptableObject
{
    // Public accessors
    public List<string> Labels => labels;

    // Private data
    [SerializeField]
    private List<string> labels = null;
}

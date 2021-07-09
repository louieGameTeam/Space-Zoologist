using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEntry
{
    // Public accessors to private data
    public ResearchCategory Category => category;
    public ResearchNotes Notes => notes;
    public ResearchEncyclopedia Encyclopedia => encyclopedia;

    // Private data
    [SerializeField]
    private ResearchCategory category;
    [SerializeField]
    private ResearchNotes notes;
    [SerializeField]
    [Expandable] 
    private ResearchEncyclopedia encyclopedia;

    public void Setup(ResearchCategoryType type)
    {
        category.Setup(type);
        notes.Setup();
    }
}

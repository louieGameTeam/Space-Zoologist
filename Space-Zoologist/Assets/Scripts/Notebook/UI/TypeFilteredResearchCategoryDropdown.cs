using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class TypeFilteredResearchCategoryDropdown : ResearchCategoryDropdown
{
    public ResearchCategoryType Type
    {
        get => type;
        set
        {
            // Set the type a call awake again to reset the dropdown
            type = value;
            Awake();
        }
    }

    [SerializeField]
    [Tooltip("Research category type that this dropdown represents")]
    private ResearchCategoryType type;

    protected override ResearchCategory[] GetResearchCategories()
    {
        return UIParent.Notebook.Research.ResearchDictionary
            .Where(kvp => kvp.Key.Type == type)
            .Select(kvp => kvp.Key)
            .ToArray();
    }
}

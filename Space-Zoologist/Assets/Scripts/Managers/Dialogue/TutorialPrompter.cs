using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DialogueEditor;

using TMPro;

/// <summary>
/// Manager of miscellaneous events that occur during tutorials in the dialogue
/// It manages the freezing of the conversation until a directive is finished,
/// as well as assists in highlighting buttons that the player needs to press
/// to complete the tutorial
/// </summary>
public class TutorialPrompter : MonoBehaviour
{
    #region Private Properties
    private ConversationFreezingScheduler FreezingScheduler
    {
        get
        {
            if(!freezingScheduler)
            {
                GameObject schedulerObject = new GameObject("Conversation Freezing Scheduler");
                freezingScheduler = schedulerObject.AddComponent<ConversationFreezingScheduler>();
            }
            return freezingScheduler;
        }
    }
    private TutorialHighlightingScheduler HighlightingScheduler
    {
        get
        {
            if(!highlightingScheduler)
            {
                GameObject schedulerObject = new GameObject("Tutorial Highlighting Scheduler");
                highlightingScheduler = schedulerObject.AddComponent<TutorialHighlightingScheduler>();
                highlightingScheduler.Setup(highlightPrefab);
            }
            return highlightingScheduler;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Prefab used to highlight different parts of the tutorial")]
    private TutorialHighlight highlightPrefab;
    #endregion

    #region Private Fields
    private ConversationFreezingScheduler freezingScheduler;
    private TutorialHighlightingScheduler highlightingScheduler;
    #endregion

    #region Public Methods
    public void ToggleUI(bool active)
    {
        GameManager.Instance.m_menuManager.ToggleUI(active);
    }

    public void ToggleUISingleButton(string buttonName)
    {
        GameManager.Instance.m_menuManager.ToggleUISingleButton(buttonName);
    }
    #endregion

    #region Prompt Callbacks
    public void FreezeUntilNotebookOpen()
    {
        bool NotebookIsOpen() => GameManager.Instance.NotebookUI.IsOpen;
        HighlightingScheduler.SetHighlights(HighlightNotebookButton());
        FreezingScheduler.FreezeUntilConditionIsMet(NotebookIsOpen);
    }
    public void FreezeUntilResearchTabOpen()
    {
        FreezeUntilNotebookTabOpen(NotebookTab.Research);
    }
    public void FreezeUntilObserveTabOpen()
    {
        FreezeUntilNotebookTabOpen(NotebookTab.Observe);
    }
    public void FreezeUntilConceptTabOpen()
    {
        FreezeUntilNotebookTabOpen(NotebookTab.Concepts);
    }
    public void FreezeUntilTestAndMetricsTabOpen()
    {
        FreezeUntilNotebookTabOpen(NotebookTab.TestAndMetrics);
    }
    public void FreezeUntilGoatTerrainHighlightAdd()
    {
        FreezeUntilHighlightPresent(
            new ItemID(ItemRegistry.Category.Species, 0), 
            0,
            new TextHighlight(195, 336));
    }
    public void FreezeUntilGoatFirstHighlightRemove()
    {
        FreezeUntilHighlightAbsent(
            new ItemID(ItemRegistry.Category.Species, 0), 
            0,
            new TextHighlight(0, 127));
    }
    public void FreezeUntilCowPicked(string pickerNameFilter)
    {
        FreezeUntilNotebookItemPicked(NotebookTab.Research, new ItemID(ItemRegistry.Category.Species, 1), pickerNameFilter);
    }
    public void FreezeUntilSlugPicked(string pickerNameFilter)
    {
        FreezeUntilNotebookItemPicked(NotebookTab.Research, new ItemID(ItemRegistry.Category.Species, 4), pickerNameFilter);
    }
    public void FreezeUntilBuildUIOpen()
    {
        FreezingScheduler.FreezeUntilConditionIsMet(() =>
        {
            return GameManager.Instance.m_menuManager.IsInStore;
        });
        HighlightingScheduler.SetHighlights(HighlightBuildButton(true));
    }
    public void FreezeUntilBuildUIClosed()
    {
        FreezingScheduler.FreezeUntilConditionIsMet(() =>
        {
            return !GameManager.Instance.m_menuManager.IsInStore;
        });
        HighlightingScheduler.SetHighlights(HighlightBuildButton(false));
    }
    public void FreezeUntilZeigPickedForPlacement()
    {
        FreezeUntilBuildItemPicked<PodSection>(new ItemID(ItemRegistry.Category.Species, 0), 2);
    }
    public void FreezeUntilZeigsExist(int numZeigs)
    {
        FreezeUntilPopulationExists(SpeciesType.Goat, numZeigs);
    }
    public void FreezeUntilZeigPopulationIncrease()
    {
        PopulationManager populationManager = GameManager.Instance.m_populationManager;
        List<Population> goatPopulations = populationManager.GetPopulationsBySpeciesType(SpeciesType.Goat);
        int currentGoats = goatPopulations.Sum(pop => pop.Count);

        FreezeUntilPopulationExists(SpeciesType.Goat, currentGoats + 1);
    }
    public void FreezeUntilInspectorOpened()
    {
        FreezingScheduler.FreezeUntilConditionIsMet(() =>
        {
            return GameManager.Instance.m_inspector.IsInInspectorMode;
        });
        HighlightingScheduler.SetHighlights(HighlightInspectorButton());
    }
    public void FreezeUntilGoatIsInspected()
    {
        FreezeUntilPopulationIsInspected(SpeciesType.Goat);
    }
    public void ClearHighlights()
    {
        HighlightingScheduler.ClearHighlights();
    }
    #endregion

    #region FreezeUntil Helpers
    private void FreezeUntilNotebookTabOpen(NotebookTab tab)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;

        // Local method returns true if notebook is open
        Func<bool> NotebookTabIsOpen(NotebookTab theTab) => () => notebook.IsOpen && notebook.TabPicker.CurrentTab == theTab;

        // Freeze until notebook tab is open
        FreezingScheduler.FreezeUntilConditionIsMet(NotebookTabIsOpen(tab));

        // Highlight notebook button if not open, then highlight correct tab
        HighlightingScheduler.SetHighlights(HighlightNotebookButton(), HighlightNotebookTabButton(tab));
    }
    private void FreezeUntilHighlightPresent(ItemID item, int articleIndex, TextHighlight targetHighlight)
    {
        // Cache some useful values
        NotebookUI notebook = GameManager.Instance.NotebookUI;

        // Get the list of all highlights in this encyclopedia article
        List<TextHighlight> highlights = notebook
            .Data.Research.GetEntry(item)
            .GetArticleData(articleIndex).Highlights;

        FreezingScheduler.FreezeUntilConditionIsMet(() =>
        {
            // Get index of a highlight that this highlight contains
            int indexOfMatch = highlights.FindIndex(current => current.Contains(targetHighlight));
            // Freeze until highlight is found and notebook is open to research tab
            return indexOfMatch >= 0 && notebook.IsOpen && notebook.TabPicker.CurrentTab == NotebookTab.Research;
        });
        HighlightingScheduler.SetHighlights(HighlightNotebookButton(), 
            HighlightNotebookTabButton(NotebookTab.Research), 
            HighlightHighlightButton());
    }
    private void FreezeUntilHighlightAbsent(ItemID item, int articleIndex, TextHighlight targetHighlight)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        ResearchEncyclopediaArticleInputField inputField = notebook.GetComponentInChildren<ResearchEncyclopediaArticleInputField>(true);

        // Get the list of all highlights in this encyclopedia article
        List<TextHighlight> highlights = notebook
            .Data.Research.GetEntry(item)
            .GetArticleData(articleIndex).Highlights;

        FreezingScheduler.FreezeUntilConditionIsMet(() =>
        {
            // Get an index of any highlight that overlaps the target highlight
            int indexOfMatch = highlights.FindIndex(current => targetHighlight.Overlap(current));
            // Freeze until no overlapping highlights found and notebook is open to research tab
            return indexOfMatch < 0 && notebook.IsOpen && notebook.TabPicker.CurrentTab == NotebookTab.Research;
        });
        HighlightingScheduler.SetHighlights(HighlightNotebookButton(), 
            HighlightNotebookTabButton(NotebookTab.Research), 
            HighlightEraseButton());
    }
    private void FreezeUntilNotebookItemPicked(NotebookTab targetTab, ItemID targetItem, string nameFilter)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        // Get all pickers in the given tab
        ItemPicker[] pickers = notebook.TabPicker.GetTabRoot(targetTab).GetComponentsInChildren<ItemPicker>(true);
        // Find a picker whose name contains the filter
        ItemPicker picker = Array.Find(pickers, p => p.name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0);

        FreezingScheduler.FreezeUntilConditionIsMet(() =>
        {
            return picker.SelectedItem == targetItem && notebook.IsOpen && notebook.TabPicker.CurrentTab == targetTab;
        });
        // Highlight notebook button if not open, then highlight correct tab
        HighlightingScheduler.SetHighlights(HighlightNotebookButton(),
            // Highlight the correct tab
            HighlightNotebookTabButton(targetTab),
            // Highlight the dropdown in the picker
            HighlightItemPicker(targetTab, targetItem, nameFilter)[0],
            HighlightItemPicker(targetTab, targetItem, nameFilter)[1]);
    }
    private void FreezeUntilBuildItemPicked<StoreSectionType>(ItemID targetItem, int storeSectionIndex) 
        where StoreSectionType : StoreSection
    {
        MenuManager menuManager = GameManager.Instance.m_menuManager;
        BuildUI buildUI = GameManager.Instance.BuildUI;
        // Get the store section that manages the placement of the target item
        StoreSectionType storeSection = buildUI.GetComponentInChildren<StoreSectionType>(true);

        // Freeze until the menu is in store, the store section is picked and the item selected is the target item
        FreezingScheduler.FreezeUntilConditionIsMet(() =>
        {
            if (storeSection.SelectedItem != null)
            {
                return menuManager.IsInStore && buildUI.StoreSectionIndexPicker.FirstValuePicked == storeSectionIndex && storeSection.SelectedItem.ItemID == targetItem;
            }
            else return false;
        });

        // Set the highlights for the build button, the section picker, and the item in that section
        HighlightingScheduler.SetHighlights(HighlightBuildButton(true),
            HighlightBuildSectionPicker(storeSectionIndex),
            HighlightBuildItem<StoreSectionType>(targetItem));
    }
    private void FreezeUntilPopulationExists(SpeciesType targetSpecies, int amount)
    {
        PopulationManager populationManager = GameManager.Instance.m_populationManager;
        RectTransform nextDayButton = FindRectTransform("NextDay");

        // Local function returns true when population is present
        bool PopulationPresent()
        {
            List<Population> goatPopulations = populationManager.GetPopulationsBySpeciesType(targetSpecies);
            int currentGoats = goatPopulations.Sum(pop => pop.Count);
            return currentGoats >= amount;
        }

        FreezingScheduler.FreezeUntilConditionIsMet(PopulationPresent);
        HighlightingScheduler.SetHighlights(new ConditionalHighlight()
        {
            predicate = PopulationPresent,
            invert = true,
            target = () => nextDayButton
        });
    }
    private void FreezeUntilPopulationIsInspected(SpeciesType targetSpecies)
    {
        Inspector inspector = GameManager.Instance.m_inspector;

        FreezingScheduler.FreezeUntilConditionIsMet(() =>
        {
            // Get the population highlighted
            if(inspector.PopulationHighlighted)
            {
                // Get the population script on the object highlighted
                Population population = inspector.PopulationHighlighted.GetComponent<Population>();

                // If you got the script then check the species type highlighted
                if(population)
                {
                    return population.Species.Species == targetSpecies;
                }
                return false;
            }
            return false;
        });
    }
    #endregion

    #region Highlighting Helpers
    private ConditionalHighlight HighlightInspectorButton()
    {
        InspectorObjectiveUI ui = GameManager.Instance.InspectorObjectUI;
        RectTransform target = ui.InspectorToggle.transform as RectTransform;

        // Return a conditional highlight that highlights the inspector toggle while not in inspector mode
        return new ConditionalHighlight()
        {
            predicate = () => !GameManager.Instance.m_inspector.IsInInspectorMode,
            target = () => target
        };
    }
    private ConditionalHighlight HighlightBuildButton(bool targetOpenState)
    {
        RectTransform buildButton = FindRectTransform("DraftButton");

        // Target the build button while not in the store
        return new ConditionalHighlight()
        {
            predicate = () => GameManager.Instance.m_menuManager.IsInStore,
            invert = targetOpenState,
            target = () => buildButton
        };
    }
    private ConditionalHighlight HighlightBuildSectionPicker(int sectionIndex)
    {
        GenericToggleGroupPicker<int> storeSectionGroup = GameManager.Instance.BuildUI.StoreSectionIndexPicker;
        GenericTogglePicker<int> storeSectionPicker = storeSectionGroup.GetTogglePicker(sectionIndex);
        RectTransform pickerTransform = storeSectionPicker.transform as RectTransform;

        // Setup a highlight to target the picker that picks the particular store section
        return new ConditionalHighlight()
        {
            predicate = () => storeSectionGroup.FirstValuePicked != sectionIndex,
            target = () => pickerTransform
        };
    }
    private ConditionalHighlight HighlightBuildItem<StoreSectionType>(ItemID item)
        where StoreSectionType : StoreSection
    {
        // Get the store section targetted
        StoreSectionType storeSection = GameManager.Instance.BuildUI.GetComponentInChildren<StoreSectionType>(true);
        // Get a list of all store item cells in the store section
        StoreItemCell[] cells = storeSection.GetComponentsInChildren<StoreItemCell>(true);
        // Find a cell in the list of all the cells that will be our target
        StoreItemCell target = Array.Find(cells, cell => cell.item.ItemID == item);
        // Convert the target transform to a rect transform
        RectTransform targetTransform = target.transform as RectTransform;

        // Return a highlight for the item cell with the given id
        return new ConditionalHighlight()
        {
            predicate = () => storeSection.SelectedItem == null || storeSection.SelectedItem.ItemID != item,
            target = () => targetTransform
        };
    }
    private ConditionalHighlight[] HighlightItemPicker(NotebookTab targetTab, ItemID targetItem, string nameFilter)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        // Get all pickers in the given tab
        ItemPicker[] pickers = notebook.TabPicker.GetTabRoot(targetTab).GetComponentsInChildren<ItemPicker>(true);
        // Find a picker whose name contains the filter
        ItemPicker picker = Array.Find(pickers, p => p.name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0);
        // Get the item dropdown for this category
        ItemDropdown categoryDropdown = picker.GetDropdown(targetItem.Category);
        // Get the dropdown targetted by this item
        TMP_Dropdown categoryDropdownTMP = picker.GetDropdown(targetItem.Category).Dropdown;
        // Get the dropdown of the target category
        RectTransform categoryDropdownTransform = categoryDropdownTMP.GetComponent<RectTransform>();

        // Local function used to get the rect transform of the particular item in the dropdown
        RectTransform DropdownItemGetter()
        {
            Transform child = categoryDropdownTransform.Find("Dropdown List");
            child = child.GetChild(categoryDropdown.DropdownIndex(targetItem) + 1);
            return child as RectTransform;
        }

        return new ConditionalHighlight[]
        {
            // Highlight the dropdown in the picker
            new ConditionalHighlight()
            {
                predicate = () => !categoryDropdownTMP.IsExpanded && picker.SelectedItem != targetItem,
                target = () => categoryDropdownTransform
            },
            // Highlight the single option button in the dropdown list
            new ConditionalHighlight()
            {
                predicate = () => picker.SelectedItem != targetItem,
                target = DropdownItemGetter
            }
        };
    }
    private ConditionalHighlight HighlightEraseButton()
    {
        // Cache some useful values
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        ResearchEncyclopediaArticleInputField inputField = notebook.GetComponentInChildren<ResearchEncyclopediaArticleInputField>(true);
        RectTransform erasePicker = FindRectTransformInChildren(notebook.TabPicker.GetTabRoot(NotebookTab.Research), "ErasePicker");

        return new ConditionalHighlight()
        {
            predicate = () => !inputField.IsHighlighting,
            target = () => erasePicker
        };
    }
    private ConditionalHighlight HighlightHighlightButton()
    {
        // Cache some useful values
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        ResearchEncyclopediaArticleInputField inputField = notebook.GetComponentInChildren<ResearchEncyclopediaArticleInputField>(true);
        RectTransform highlightPicker = FindRectTransformInChildren(notebook.TabPicker.GetTabRoot(NotebookTab.Research), "HighlightPicker");

        return new ConditionalHighlight()
        {
            predicate = () => !inputField.IsHighlighting,
            target = () => highlightPicker
        };
    }
    private ConditionalHighlight HighlightNotebookTabButton(NotebookTab tab)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        RectTransform tabButton = notebook.TabPicker.GetTabSelectButton(tab).GetComponent<RectTransform>();

        // Return a highlight that will be active while the tab is not picked
        return new ConditionalHighlight()
        {
            predicate = () => notebook.TabPicker.CurrentTab != tab,
            target = () => tabButton
        };
    }
    private ConditionalHighlight HighlightNotebookButton()
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        RectTransform notebookButton = FindRectTransform("NotebookButton");
        
        // Highlight the notebook button while notebook is not open
        return new ConditionalHighlight()
        {
            predicate = () => !notebook.IsOpen,
            target = () => notebookButton
        };
    }
    // Find rect transform in children of the parent with the given name
    private RectTransform FindRectTransformInChildren(Transform parent, string name)
    {
        Transform child = parent.Find(name);

        if(child)
        {
            RectTransform childRect = child as RectTransform;

            if (childRect) return childRect;
            else
            {
                Debug.Log($"{nameof(TutorialPrompter)}: child '{child}' is not a rect transform");
                return null;
            }
        }
        else
        {
            Debug.Log($"{nameof(TutorialPrompter)}: parent '{parent}' has no child named '{child}'");
            return null;
        }
    }
    private RectTransform FindRectTransform(string name)
    {
        GameObject targetGameObject = GameObject.Find(name);

        if(targetGameObject)
        {
            RectTransform target = targetGameObject.transform as RectTransform;

            // Return the rect transform you got
            if (target) return target;
            // No rect transform found on the object so log the result and return null
            else
            {
                Debug.Log($"{nameof(TutorialPrompter)}: Game object {targetGameObject} " +
                    $"has no component of type {nameof(RectTransform)} attached");
                return null;
            }
        }
        // No game object found so log the result and return null
        else
        {
            Debug.Log($"{nameof(TutorialPrompter)}: Unable to find an object named {name} in the scene");
            return null;
        }
    }
    #endregion
}

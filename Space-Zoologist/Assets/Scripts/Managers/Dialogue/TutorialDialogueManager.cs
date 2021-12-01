using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DialogueEditor;

public class TutorialDialogueManager : MonoBehaviour
{
    #region Private Properties
    private TutorialCoroutineScheduler CoroutineScheduler
    {
        get
        {
            if(!coroutineScheduler)
            {
                // Try to find an existing scheduler
                coroutineScheduler = FindObjectOfType<TutorialCoroutineScheduler>();

                // If no scheduler exists, then create one
                if(!coroutineScheduler)
                {
                    GameObject schedulerObject = new GameObject("Tutorial Coroutine Scheduler");
                    coroutineScheduler = schedulerObject.AddComponent<TutorialCoroutineScheduler>();
                }
            }
            return coroutineScheduler;
        }
    }
    #endregion

    #region Private Fields
    private TutorialCoroutineScheduler coroutineScheduler;
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

    #region Dialogue Callbacks
    public void FreezeUntilNotebookOpen()
    {
        CoroutineScheduler.FreezeUntilConditionIsMet(() => GameManager.Instance.NotebookUI.IsOpen);
    }
    public void FreezeUntilResearchTabOpen()
    {
        FreezeUntilNotebookTabOpen(NotebookTab.Research);
    }
    public void FreezeUntilGoatTerrainHighlightPresent()
    {
        FreezeUntilHighlightPresent(
            new ItemID(ItemRegistry.Category.Species, 0), 
            0,
            new TextHighlight(195, 336));
    }
    public void FreezeUntilGoatFirstHighlightAbsent()
    {
        FreezeUntilHighlightAbsent(
            new ItemID(ItemRegistry.Category.Species, 0), 
            0,
            new TextHighlight(0, 127));
    }
    public void FreezeUntilSlugPicked(string pickerNameFilter)
    {
        FreezeUntilItemPicked(NotebookTab.Research, new ItemID(ItemRegistry.Category.Species, 4), pickerNameFilter);
    }
    public void FreezeUntilBuildUIOpen()
    {
        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            return GameManager.Instance.m_menuManager.IsInStore;
        });
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
        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            return GameManager.Instance.m_inspector.IsInInspectorMode;
        });
    }
    public void FreezeUntilGoatIsInspected()
    {
        FreezeUntilPopulationIsInspected(SpeciesType.Goat);
    }
    #endregion

    #region Private Methods
    private void FreezeUntilNotebookTabOpen(NotebookTab tab)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            return notebook.IsOpen && notebook.TabPicker.CurrentTab == tab;
        });
    }
    private void FreezeUntilHighlightPresent(ItemID item, int articleIndex, TextHighlight targetHighlight)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;

        // Get the list of all highlights in this encyclopedia article
        List<TextHighlight> highlights = notebook
            .Data.Research.GetEntry(item)
            .GetArticleData(articleIndex).Highlights;

        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            // Get index of a highlight that this highlight contains
            int indexOfMatch = highlights.FindIndex(current => current.Contains(targetHighlight));
            // Freeze until highlight is found and notebook is open to research tab
            return indexOfMatch >= 0 && notebook.IsOpen && notebook.TabPicker.CurrentTab == NotebookTab.Research;
        });
    }
    private void FreezeUntilHighlightAbsent(ItemID item, int articleIndex, TextHighlight targetHighlight)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;

        // Get the list of all highlights in this encyclopedia article
        List<TextHighlight> highlights = notebook
            .Data.Research.GetEntry(item)
            .GetArticleData(articleIndex).Highlights;

        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            // Get an index of any highlight that overlaps the target highlight
            int indexOfMatch = highlights.FindIndex(current => targetHighlight.Overlap(current));
            // Freeze until no overlapping highlights found and notebook is open to research tab
            return indexOfMatch < 0 && notebook.IsOpen && notebook.TabPicker.CurrentTab == NotebookTab.Research;
        });
    }
    private void FreezeUntilItemPicked(NotebookTab targetTab, ItemID targetItem, string nameFilter)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;
        // Get all pickers in the given tab
        ItemPicker[] pickers = notebook.TabPicker.GetTabRoot(targetTab).GetComponentsInChildren<ItemPicker>(true);
        // Find a picker whose name contains the filter
        ItemPicker picker = Array.Find(pickers, p => p.name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0);

        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            return picker.SelectedItem == targetItem && notebook.IsOpen && notebook.TabPicker.CurrentTab == targetTab;
        });
    }
    private void FreezeUntilBuildItemPicked<StoreSectionType>(ItemID targetItem, int storeSectionIndex) 
        where StoreSectionType : StoreSection
    {
        MenuManager menuManager = GameManager.Instance.m_menuManager;
        BuildUI buildUI = GameManager.Instance.BuildUI;
        // Get the store section that manages the placement of the target item
        StoreSectionType storeSection = buildUI.GetComponentInChildren<StoreSectionType>(true);

        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            if (storeSection.SelectedItem != null)
            {
                return menuManager.IsInStore && buildUI.StoreSectionIndexPicker.FirstValuePicked == storeSectionIndex && storeSection.SelectedItem.ItemID == targetItem;
            }
            else return false;
        });
    }
    private void FreezeUntilPopulationExists(SpeciesType targetSpecies, int amount)
    {
        PopulationManager populationManager = GameManager.Instance.m_populationManager;

        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            List<Population> goatPopulations = populationManager.GetPopulationsBySpeciesType(targetSpecies);
            int currentGoats = goatPopulations.Sum(pop => pop.Count);
            return currentGoats >= amount;
        });
    }
    private void FreezeUntilPopulationIsInspected(SpeciesType targetSpecies)
    {
        Inspector inspector = GameManager.Instance.m_inspector;

        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
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
}

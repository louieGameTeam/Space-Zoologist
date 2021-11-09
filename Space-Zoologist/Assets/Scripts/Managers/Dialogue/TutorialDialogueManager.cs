using System;
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
            new ResearchEncyclopediaArticleHighlight(195, 336));
    }
    public void FreezeUntilGoatFirstHighlightAbsent()
    {
        FreezeUntilHighlightAbsent(
            new ItemID(ItemRegistry.Category.Species, 0), 
            0,
            new ResearchEncyclopediaArticleHighlight(0, 127));
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
        MenuManager menuManager = GameManager.Instance.m_menuManager;
        BuildUI buildUI = GameManager.Instance.BuildUI;
        PodSection podSection = buildUI.GetComponentInChildren<PodSection>();
        ItemID zeigID = new ItemID(ItemRegistry.Category.Species, 0);

        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            if (podSection.SelectedItem != null)
            {
                return menuManager.IsInStore && buildUI.StoreSectionIndexPicker.FirstValuePicked == 2 && podSection.SelectedItem.ItemID == zeigID;
            }
            else return false;
        });
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
    private void FreezeUntilHighlightPresent(ItemID item, int articleIndex, ResearchEncyclopediaArticleHighlight targetHighlight)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;

        // Get the list of all highlights in this encyclopedia article
        List<ResearchEncyclopediaArticleHighlight> highlights = notebook
            .Notebook.Research.GetEntry(item)
            .Encyclopedia.GetArticle(articleIndex).Highlights;

        CoroutineScheduler.FreezeUntilConditionIsMet(() =>
        {
            // Get index of a highlight that this highlight contains
            int indexOfMatch = highlights.FindIndex(current => current.Contains(targetHighlight));
            // Freeze until highlight is found and notebook is open to research tab
            return indexOfMatch >= 0 && notebook.IsOpen && notebook.TabPicker.CurrentTab == NotebookTab.Research;
        });
    }
    private void FreezeUntilHighlightAbsent(ItemID item, int articleIndex, ResearchEncyclopediaArticleHighlight targetHighlight)
    {
        NotebookUI notebook = GameManager.Instance.NotebookUI;

        // Get the list of all highlights in this encyclopedia article
        List<ResearchEncyclopediaArticleHighlight> highlights = notebook
            .Notebook.Research.GetEntry(item)
            .Encyclopedia.GetArticle(articleIndex).Highlights;

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
    #endregion
}

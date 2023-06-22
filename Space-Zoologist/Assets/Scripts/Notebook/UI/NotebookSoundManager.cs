using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Finds all general dropdowns and tmp dropdowns in the notebook game object
/// and plays a sound when they are enabled
/// </summary>
public class NotebookSoundManager : NotebookUIChild
{
    #region Public Typedefs
    public class PreviousExpandedState
    {
        public TMP_Dropdown dropdown;
        public bool previousExpandedState;
    }
    #endregion

    #region Private Fields
    private List<PreviousExpandedState> expandedStates = new List<PreviousExpandedState>();
    #endregion

    #region Monobehaviour Messages
    private void Update()
    {
        foreach(PreviousExpandedState state in expandedStates)
        {
            // If the current expanded state is unequal to the previous expanded state
            // then play the dropdown sound
            if(state.dropdown.IsExpanded && !state.previousExpandedState)
            {
                PlayDropdownSound();
            }
            else if (!state.dropdown.IsExpanded && state.previousExpandedState)
            {
                PlayCloseDropdownSound();
            }
            
            if(state.previousExpandedState)
                print(state.dropdown.IsExpanded + " " + state.previousExpandedState);

            state.previousExpandedState = state.dropdown.IsExpanded;
        }
    }
    
    #endregion

    #region Public Methods
    public void SetupSoundEvents()
    {
        // Make all general dropdowns play the dropdown sound
        GeneralDropdown[] dropdowns = UIParent.GetComponentsInChildren<GeneralDropdown>(true);
        foreach (GeneralDropdown dropdown in dropdowns)
        {
            dropdown.OnDropdownEnabled.AddListener(PlayDropdownSound);
        }

        // Add the expanded states of all tmp dropdowns to the list
        TMP_Dropdown[] tmpDropdowns = UIParent.GetComponentsInChildren<TMP_Dropdown>(true);
        foreach(TMP_Dropdown tmpDropdown in tmpDropdowns)
        {
            expandedStates.Add(new PreviousExpandedState() 
            { 
                dropdown = tmpDropdown, 
                previousExpandedState = tmpDropdown.IsExpanded 
            });
        }
    }
    #endregion

    #region Private Methods
    private void PlayDropdownSound() => AudioManager.instance.PlayOneShot(SFXType.NotebookDropdown);
    private void PlayCloseDropdownSound() => AudioManager.instance.PlayOneShot(SFXType.DropdownClose);
    #endregion
}

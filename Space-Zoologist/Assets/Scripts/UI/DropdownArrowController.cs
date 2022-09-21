using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class DropdownArrowController : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the dropdown arrow")]
    protected Image dropdownArrow;
    private Vector3 collapsedRotation;  // Rotation of dropdown arrow when dropdown is collapsed
    private Vector3 expandedRotation;   // Rotation of dropdown arrow when dropdown is expanded
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        collapsedRotation = new Vector3(dropdownArrow.transform.eulerAngles.x, dropdownArrow.transform.eulerAngles.y, dropdownArrow.transform.eulerAngles.z);
        expandedRotation = new Vector3(collapsedRotation.x, collapsedRotation.y, collapsedRotation.z + 180);
    }
    #endregion

    #region Public Methods
    public void OnDropdownExpand()
    {
        ExpandDropdownArrow();
    }
    public void OnDropdownCollapse()
    {
        if (dropdownArrow.transform.eulerAngles == expandedRotation)
        {
            CollapseDropdownArrow();
        }
    }
    #endregion

    #region Private Methods
    private void CollapseDropdownArrow()
    {
        dropdownArrow.transform.eulerAngles = collapsedRotation;
    }
    private void ExpandDropdownArrow()
    {
        dropdownArrow.transform.eulerAngles = expandedRotation;
    }
    #endregion
}
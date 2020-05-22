using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Sets up and displays whatever is given to it
/// /// </summary>
public class JournalPopupDisplayLogic : MonoBehaviour
{
    [SerializeField] ToggleGroup SpeciesToggle = default;
    public void RemoveSelfFromList(GameObject item)
    {
        Destroy(item);
    }

    public void UpdatePopupActiveSelf()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }

    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }

    public void CheckSpeciesToggle()
    {
        if (!this.SpeciesToggle.AnyTogglesOn())
        {
            this.gameObject.SetActive(false);
            Debug.Log("Must select a species first");
        }
    }
}

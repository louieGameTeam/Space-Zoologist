using UnityEngine.UI;
using UnityEngine;

public class DescriptionDisplayLogic : MonoBehaviour
{
    [SerializeField] Text Description = default;

    public void InitializeSpeciesDescription(GameObject species)
    {
        this.Description.text = species.GetComponent<SpeciesData>().JournalData.DiscoveredSpeciesEntryText;
    }
}

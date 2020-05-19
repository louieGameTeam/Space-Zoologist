using UnityEngine.UI;
using UnityEngine;

public class DescriptionDisplayLogic : MonoBehaviour
{
    [SerializeField] Text Description = default;

    public void InitializeDescription(GameObject species)
    {
        this.Description.text = species.GetComponent<SpeciesData>().Description;
    }
}

using UnityEngine.UI;
using UnityEngine;

public class SpeciesEntryDisplayLogic : MonoBehaviour
{
    [Header("Child component References")]
    [SerializeField] Image SpeciesPicture = default;
    [SerializeField] Text SpeciesName = default;

    public void Initialize(AnimalSpecies speciesInfo)
    {
        this.SpeciesPicture.sprite = speciesInfo.Sprite;
        this.SpeciesName.text = speciesInfo.SpeciesName;
    }
}

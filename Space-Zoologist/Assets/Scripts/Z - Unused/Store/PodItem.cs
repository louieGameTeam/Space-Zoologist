using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class PodItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image speciesImage = default;
    [SerializeField] GameObject Popup = default;
    [SerializeField] Text ItemInfo = default;
    AnimalSpecies species = default;
    PodSection podMenu = default;

    public void Initialize(AnimalSpecies species, PodSection podMenu)
    {
        this.species = species;
        speciesImage.sprite = species.Icon;
        this.podMenu = podMenu;
    }

    public void OnPodClick()
    {
        //podMenu.OnSelectSpecies(species);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.Popup.SetActive(true);
        this.ItemInfo.text = this.species.SpeciesName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.Popup.SetActive(false);
    }
}

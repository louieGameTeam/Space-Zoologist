using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PodItem : MonoBehaviour
{
    [SerializeField] Image speciesImage = default;
    AnimalSpecies species = default;
    PodMenu podMenu = default;

    public void Initialize(AnimalSpecies species, PodMenu podMenu)
    {
        this.species = species;
        speciesImage.sprite = species.Icon;
        this.podMenu = podMenu;
    }

    public void OnPodClick()
    {
        podMenu.OnSelectSpecies(species);
    }
}

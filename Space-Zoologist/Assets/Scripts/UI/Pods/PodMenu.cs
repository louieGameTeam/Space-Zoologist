using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PodMenu : MonoBehaviour
{
    [SerializeField] LevelData levelData = default;
    [SerializeField] PopulationManager populationManager = default;
    [SerializeField] GameObject podButtonPrefab = default;
    [SerializeField] Transform PodItemContainer = default;
    [SerializeField] CursorItem cursorItem = default;
    RectTransform rectTransform = default;
    AnimalSpecies selectedSpecies = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        foreach (AnimalSpecies species in levelData.AnimalSpecies)
        {
            GameObject newPodItem = Instantiate(podButtonPrefab, PodItemContainer);
            PodItem podItem = newPodItem.GetComponent<PodItem>();
            podItem.Initialize(species, this);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!PointOverMenu(Input.mousePosition) && !selectedSpecies)
            {
                // Debug.Log("Pod issue");
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            DeselectSpecies();
        }
    }

    public void OnSelectSpecies(AnimalSpecies animalSpecies)
    {
        selectedSpecies = animalSpecies;
        cursorItem.Begin(animalSpecies.Icon, OnCursorItemClick);
    }

    private void DeselectSpecies()
    {
        selectedSpecies = null;
        if (cursorItem.isActiveAndEnabled)
        {
            cursorItem.Stop(OnCursorItemClick);
        }
    }

    private void OnDisable()
    {
        DeselectSpecies();
    }

    public void OnCursorItemClick(PointerEventData pointerEventData)
    {
        // If in CursorItem mode and the cursor is clicked while over the menu
        if (PointOverMenu(pointerEventData.position))
        {
            Debug.Log("Clicked over UI");
            DeselectSpecies();
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(pointerEventData.position);
            populationManager.AddAnimals(selectedSpecies, 1, position);
        }
    }

    private bool PointOverMenu(Vector3 point)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, point);
    }
}

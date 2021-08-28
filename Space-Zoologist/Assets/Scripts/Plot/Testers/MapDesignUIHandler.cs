using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum SelectionType {Tile, Animal, FoodSource, None }
public class MapDesignUIHandler : MonoBehaviour
{
    [SerializeField] private InputField fileNameInputField;
    [SerializeField] private LevelIO levelIO;
    [SerializeField] private TilePlacementController tilePlacementController;
    private string fileName;

    public void OnButtonSetFileName() { fileName = fileNameInputField.text; }
    public void OnButtonSaveLevel() { levelIO.SaveAsPreset(fileName); }
    public void OnButtonLoadLevel() { 
        levelIO.LoadPreset(fileName); 
    }
    public void OnErasingToggle() { tilePlacementController.isErasing = !tilePlacementController.isErasing; }
}

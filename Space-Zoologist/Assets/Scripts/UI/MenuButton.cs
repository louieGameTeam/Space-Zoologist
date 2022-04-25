using UnityEngine;  
 using System.Collections;  
 using UnityEngine.EventSystems;  
 using UnityEngine.UI;
 using TMPro;
 
 // Changes the color of some text when the button is highlighted
 public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
 
    public TextMeshProUGUI text;
    public Color SelectedColor;

    private Color defaultColor;

    private void Start() {
        defaultColor = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = SelectedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = defaultColor;
    }
}
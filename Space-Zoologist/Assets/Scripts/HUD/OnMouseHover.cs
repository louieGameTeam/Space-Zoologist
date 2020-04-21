using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class OnMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private GameObject DisplayToAppear;

    public void OnPointerEnter(PointerEventData eventData)
    {

        Debug.Log(DisplayToAppear.transform.GetChild(0).GetComponent<Text>().text);
        this.DisplayToAppear.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.DisplayToAppear.SetActive(false);
    }

    public void InitializeDisplay(GameObject display)
    {
        this.DisplayToAppear = display;
    }
}

using UnityEngine.UI;
using UnityEngine;

public class ResearchPopupDisplayLogic : MonoBehaviour
{
    [Header("Add children UI components")]
    [SerializeField] Text ItemName = default;
    [SerializeField] Text ItemDescription = default;

    public void SetupPopup(GameObject hoveredNeed)
    {
        NeedData needData = hoveredNeed.GetComponent<NeedData>();
        this.ItemDescription.text = "Progress description here maybe";
        this.ItemName.text = needData.Need.Name.ToString();
        this.gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }
}

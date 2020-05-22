using UnityEngine.UI;
using UnityEngine;

public class NeedsEntryDisplayLogic : MonoBehaviour
{
    [SerializeField] Text NeedName = default;
    [SerializeField] Image Sprite = default;
    public void SetupDisplay(SpeciesNeed need)
    {
        this.gameObject.SetActive(true);
        this.Sprite.sprite = need.Sprite;
        this.gameObject.GetComponent<ItemData>().SpeciesNeedItemData = need;
        this.NeedName.text = need.Name.ToString();
    }
}

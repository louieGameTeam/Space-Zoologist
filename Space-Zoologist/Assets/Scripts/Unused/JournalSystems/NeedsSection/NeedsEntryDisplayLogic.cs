using UnityEngine.UI;
using UnityEngine;

public class NeedsEntryDisplayLogic : MonoBehaviour
{
    [SerializeField] Text NeedName = default;
    [SerializeField] Image Sprite = default;
    public void SetupDisplay(Need need)
    {
        this.gameObject.SetActive(true);
        this.NeedName.text = need.NeedName[0];
        this.Sprite.sprite = need.Sprite;
    }
}

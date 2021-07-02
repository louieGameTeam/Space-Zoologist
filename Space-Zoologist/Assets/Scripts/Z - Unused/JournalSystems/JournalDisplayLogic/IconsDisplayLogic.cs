using UnityEngine;

public class IconsDisplayLogic : MonoBehaviour
{
    [SerializeField] GameObject AddNeedIcon = default;
    [SerializeField] GameObject RemoveNeedIcon = default;
    //[SerializeField] GameObject ResearchNeedIcon = default;

    public void UpdateNeedIcons(GameObject needSelected)
    {
        this.AddNeedIcon.SetActive(true);
        this.RemoveNeedIcon.SetActive(true);
    }
}

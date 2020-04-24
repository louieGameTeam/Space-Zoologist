using UnityEngine;

public class FloatingObjectManager : MonoBehaviour
{
    [SerializeField] Camera CurrentCamera = default;
    [SerializeField] GameObject FloatingObjectPreview = default;
    public void CreateNewFloatingObject(Sprite sprite)
    {
        // TODO communicate to the correct system that an item which will affect it has been placed down
        GameObject placedItem = Instantiate(this.FloatingObjectPreview, this.transform);
        placedItem.transform.SetParent(this.gameObject.transform);
        placedItem.transform.position = new Vector3(this.CurrentCamera.ScreenToWorldPoint(Input.mousePosition).x, this.CurrentCamera.ScreenToWorldPoint(Input.mousePosition).y, 0);
        placedItem.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}

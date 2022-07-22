using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for moving item preview around
/// </summary>
[System.Serializable]
public class ItemPlaceCursorPreviewMover
{
    private TileDataController tileController;
    private GameObject target;
    private Vector2Int size;
    private bool snap;
    public void UpdatePosition()
    {
        Vector3 curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curPos.z = target.transform.position.z;
        if (snap)
            target.transform.position = tileController.SnapAreaToCell(curPos, size);
        else
            target.transform.position = curPos;
    }

    public ItemPlaceCursorPreviewMover(TileDataController tileController, Sprite sprite, Vector2Int size, bool snap)
    {
        target = new GameObject("Preview Cursor");
        var renderer = target.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 100;
        renderer.sprite = sprite;
        this.tileController = tileController;
        this.size = size;
        this.snap = snap;
        var ren = target.GetComponent<SpriteRenderer>();
        if(ren)
        {
            ren.color = ren.color.SetAlpha(0.6f);
        }
    }

    public void Clear()
    {
        GameObject.Destroy(target);
    }
}

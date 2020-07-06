using UnityEngine;
using System.Collections.Generic;

public class FloatingObjectManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] Camera CurrentCamera = default;
    [SerializeField] GameObject FloatingObjectPreview = default;
    [Header("Increase for larger levels")]
    [SerializeField] private int NumberOfObjectsToPool = 10;
    private List<GameObject> PooledObjects = new List<GameObject>();
    public ItemSelectedEvent ItemSelected = new ItemSelectedEvent(); 
    private PoolingSystem PoolSystem = default;

    public void Start()
    {
        this.PoolSystem = this.gameObject.AddComponent(typeof(PoolingSystem)) as PoolingSystem;
        this.PooledObjects = this.PoolSystem.AddPooledObjects(this.NumberOfObjectsToPool, this.FloatingObjectPreview);
    }

    // Generic way of creating pooled object and initializing it with data
    public void CreateNewFloatingObject(ItemData item)
    {
        GameObject placedItem = this.PoolSystem.GetPooledObject(this.PooledObjects);
        if (placedItem == null)
        {
            // May take up a lot of memory?
            this.PooledObjects.AddRange(this.PoolSystem.AddPooledObjects(this.NumberOfObjectsToPool, this.FloatingObjectPreview));
            placedItem = this.PoolSystem.GetPooledObject(this.PooledObjects);
        }
        placedItem.transform.SetParent(this.gameObject.transform);
        placedItem.transform.position = new Vector3(this.CurrentCamera.ScreenToWorldPoint(Input.mousePosition).x, this.CurrentCamera.ScreenToWorldPoint(Input.mousePosition).y, 0);
        placedItem.GetComponent<SpriteRenderer>().sprite = item.StoreItemData.Sprite;
        placedItem.GetComponent<ItemData>().StoreItemData = item.StoreItemData;
        this.SetupItemSelectedHandler(placedItem, this.ItemSelected);
        placedItem.SetActive(true);
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent itemSelected)
    {
        item.GetComponent<SelectableSprite>().SetupItemSelectedHandler(this.ItemSelected);
    }
    
}

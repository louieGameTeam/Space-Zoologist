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

    public void Start()
    {
        this.AddPooledObjects();
    }

    private void AddPooledObjects()
    {
        for (int i = 0; i < this.NumberOfObjectsToPool; i++)
        {
            PooledObjects.Add(Instantiate(this.FloatingObjectPreview, this.transform));
        }
    }

    // Generic way of creating pooled object and initializing it with data
    public void CreateNewFloatingObject(ItemData item)
    {
        GameObject placedItem = GetPooledObject();
        if (placedItem == null)
        {
            this.AddPooledObjects();
            placedItem = GetPooledObject();
        }
        placedItem.transform.SetParent(this.gameObject.transform);
        placedItem.transform.position = new Vector3(this.CurrentCamera.ScreenToWorldPoint(Input.mousePosition).x, this.CurrentCamera.ScreenToWorldPoint(Input.mousePosition).y, 0);
        placedItem.GetComponent<SpriteRenderer>().sprite = item.StoreItemData.Sprite;
        placedItem.GetComponent<ItemData>().StoreItemData = item.StoreItemData;
        this.SetupItemSelectedHandler(placedItem, this.ItemSelected);
        placedItem.SetActive(true);
    }

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < this.PooledObjects.Count; i++) {
            if (!this.PooledObjects[i].activeInHierarchy) {
            return this.PooledObjects[i];
            }
        }
        return null;
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent itemSelected)
    {
        item.GetComponent<SelectableSprite>().SetupItemSelectedHandler(this.ItemSelected);
    }
    
}

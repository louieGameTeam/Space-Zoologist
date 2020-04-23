using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AddFood : MonoBehaviour
{
    public GameObject managers;
    public FoodSourceManager foodSourceManager;
    public FoodSourceType type;

    private void Awake()
    {
        managers = GameObject.Find("Managers");
        foodSourceManager = managers.GetComponent<FoodSourceManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(addFood);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject foodPrefab;

    private void addFood()
    {
        Instantiate(foodPrefab, new Vector3(Random.Range(-3.0f, 7.0f), Random.Range(-5.0f, 3.0f), 0), Quaternion.identity);
        //EventManager.TriggerEvent("Add pop");

        foodSourceManager.CreateFoodSource(type, Vector2Int.zero);

        Debug.Log($"Food source count: {foodSourceManager.foodListSize}");
    }
}

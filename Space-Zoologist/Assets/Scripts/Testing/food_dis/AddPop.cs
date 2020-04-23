using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPop : MonoBehaviour
{
    public GameObject managers;
    public PopulationManager populationManager;
    public Species specie;

    private void Awake()
    {
        managers = GameObject.Find("Managers");
        populationManager = managers.GetComponent<PopulationManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(addPop);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject popPrefab;

    private void addPop()
    {
        Instantiate(popPrefab, new Vector3(Random.Range(-3.0f, 7.0f), Random.Range(-5.0f, 3.0f), 0), Quaternion.identity);
        //EventManager.TriggerEvent("Add pop");

        populationManager.CreatePopulation(specie, Vector2Int.zero);

        Debug.Log($"Population count: {populationManager.popListSize}");
    }
}
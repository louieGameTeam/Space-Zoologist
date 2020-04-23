using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tester : MonoBehaviour
{
    //public GameObject textObj;
    public Text text;

    PopulationManager popMan;
    FoodSourceManager foodMan;

    private void Awake()
    {
        //textObj = GameObject.Find("Text");
        //text = textObj.GetComponent<Text>();
        Debug.Log(text.text);

        popMan = FindObjectOfType<PopulationManager>();
        foodMan = FindObjectOfType<FoodSourceManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
   
    // Update is called once per frame
    void Update()
    {

        Population pop = FindObjectOfType<Population>();

        text.text = $"Total food: {foodMan.getTotalFood()}\n" + $"Total pop: {popMan.popListSize}\n"
                    + $"Food distributed: {pop.GetNeedValue(NeedType.SpaceMaple)}";
    }
}

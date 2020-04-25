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
    NeedSystemManager needMan;

    private void Awake()
    {
        //textObj = GameObject.Find("Text");
        //text = textObj.GetComponent<Text>();
        //Debug.Log(text.text);

        this.gameObject.AddComponent<PopulationManager>();
        this.gameObject.AddComponent<FoodSourceManager>();
        this.gameObject.AddComponent<NeedSystemManager>();

        popMan = this.GetComponent<PopulationManager>();
        foodMan = this.GetComponent<FoodSourceManager>();
        needMan = this.GetComponent<NeedSystemManager>();

        popMan.needSystemManager = this.needMan;
        foodMan.needMan = this.needMan;
        foodMan.popMan = this.popMan;
        needMan.foodMan = this.foodMan;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
   
    // Update is called once per frame
    void Update()
    {

        Population pop = FindObjectOfType<Population>();

        if(pop)
        {
            text.text = $"Total food: {foodMan.getTotalFood()}\n" + $"Total pop: {popMan.popListSize}\n"
                    + $"Food distributed: {pop.GetNeedValue(NeedType.SpaceMaple)}";
        }
        else
        {
            text.text = $"Total food: {foodMan.getTotalFood()}\n" + $"Total pop: {popMan.popListSize}\n";
                    //+ $"Food distributed: {pop.GetNeedValue(NeedType.SpaceMaple)}";
        }
        
    }
}

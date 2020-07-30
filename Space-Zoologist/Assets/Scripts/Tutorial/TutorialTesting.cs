using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TutorialTesting : MonoBehaviour
{
    [SerializeField] NeedSystemManager NeedSystemManager = default;
    [SerializeField] PopulationManager PopulationManager = default;
    [SerializeField] FoodSourceManager FoodSourceManager = default;
    [SerializeField] Tilemap baseLayer = default;
    [SerializeField] PodMenu podMenu = default;
    [SerializeField] List<Item> ItemToAdd = default;
    [SerializeField] FoodSourceSpecies foodSourceToAdd = default;
    [SerializeField] List<StoreSection> SectionToAddTo = default;
    [SerializeField] AnimalSpecies speciesToIntroduce = default;
    [SerializeField] List<string> TutorialsDescriptions = default;
    [SerializeField] Text TutorialDialogue = default;
    private List<bool> Triggers = new List<bool>();

    private void Awake()
    {
        for (int i=0; i<this.TutorialsDescriptions.Count; i++)
        {
            this.Triggers.Add(false);
        }
    }

    private void Start()
    {
        this.ShadeOutsidePerimeter();
        //TODO: what this is doing?
        //NeedSystemManager.AddSystem(new FoodSourceNeedSystem(ReservePartitionManager.ins, ItemToAdd[0].ItemName));
        NeedSystemManager.AddSystem(new FoodSourceNeedSystem(ReservePartitionManager.ins, NeedType.FoodSource));
        this.TriggerDialogue(0);
    }

    public void Update()
    {
        if (!this.Triggers[0] && PopulationManager.Populations.Count == 1)
        {
            this.AddItem(0);
            this.TriggerDialogue(2);
            this.podMenu.Pods[0].SetActive(false);
            this.podMenu.DeselectSpecies();
            this.Triggers[0] = true;
        }
        else if (!this.Triggers[1] && FoodSourceManager.FoodSources.Count > 0)
        {
            this.TriggerDialogue(3);
            PopulationManager.Populations[0].UpdateNeed("Fruit Tree", 10);
            PopulationManager.Populations[0].UpdateGrowthConditions();
            this.Triggers[1] = true;
        }
        else if (!this.Triggers[2] && PopulationManager.Populations.Count == 1 && PopulationManager.Populations[0].AnimalPopulation.Count > 1)
        {
            PopulationManager.Populations[0].UpdateNeed("Fruit Tree", 0);
            PopulationManager.Populations[0].UpdateGrowthConditions();
            this.AddSpecies();
            this.AddItem(1);
            this.TriggerDialogue(4);
            this.Triggers[2] = true;
        }
        else if (!this.Triggers[3] && PopulationManager.Populations.Count == 2)
        {
            PopulationManager.Populations[1].AutomotonTesting = true;
            this.podMenu.DeselectSpecies();
            this.Triggers[3] = true;
            this.podMenu.Pods[1].SetActive(false);
            this.TriggerDialogue(5);
        }
    }

    public void TriggerDialogue(int index)
    {
        this.TutorialDialogue.text = this.TutorialsDescriptions[index];
    }

    public void TriggerDialogueOnce()
    {
        if (!this.Triggers[4])
        {
            this.Triggers[4] = true;
            this.TriggerDialogue(1);
        }
    }

    public void AddSpecies()
    {
        // TODO: only need to add a system once
        // Creating new need calculator is hanlde when a new consumer or source is added
        //NeedSystemManager.AddSystem(new SpeciesNeedSystem(ReservePartitionManager.ins, speciesToIntroduce.SpeciesName));
        NeedSystemManager.AddSystem(new SpeciesNeedSystem(ReservePartitionManager.ins, NeedType.Species));
        this.podMenu.AddPod(speciesToIntroduce);
    }

    public void AddItem(int index)
    {
        if (index == 0)
        {
            FoodSourceManager.UpdateFoodSourceSpecies(foodSourceToAdd);
        }
        this.SectionToAddTo[index].AddItem(ItemToAdd[index]);
    }

    private void ShadeOutsidePerimeter()
    {
        for (int x=-2; x<TilemapUtil.ins.MaxWidth + 2; x++)
        {
            this.ShadeSquare(x, -2, Color.red);
            this.ShadeSquare(x, TilemapUtil.ins.MaxHeight + 1, Color.red);
        }
        for (int y=-2; y<TilemapUtil.ins.MaxHeight + 2; y++)
        {
            this.ShadeSquare(-2, y, Color.red);
            this.ShadeSquare(TilemapUtil.ins.MaxWidth + 1, y, Color.red);
        }
    }

    private void ShadeSquare(int x, int y, Color color)
    {
        Vector3Int cellToShade = new Vector3Int(x, y, 0);
        baseLayer.SetColor(cellToShade, color);
    }
}

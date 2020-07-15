using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

[CreateAssetMenu]
public class AnimalSpecies : ScriptableObject
{
    // Getters
    public string SpeciesName => speciesName;
    public int Dominance => dominance;
    public float GrowthFactor => growthFactor;
    public Dictionary<string, Need> Needs => needs;
    public float Size => size;
    public List<BehaviorScriptTranslation> Behaviors => needBehaviorSet;
    public List<TileType> AccessibleTerrain => accessibleTerrain;
    public Sprite Icon => icon;
    public Sprite Sprite => icon;
    public Sprite Representation => representation;

    public AnimatorController AnimatorController => animatorController;

    // Values
    [SerializeField] private AnimatorController animatorController = default;
    [SerializeField] private string speciesName = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private int dominance = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private float growthFactor = default;
    private Dictionary<string, Need> needs = new Dictionary<string, Need>();
    [SerializeField] private List<NeedTypeConstructData> needsList = default;
    [Header("Behavior displayed when need isn't being met")]
    [SerializeField] private List<BehaviorScriptTranslation> needBehaviorSet = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float size = default;
    [SerializeField] private List<TileType> accessibleTerrain = default;
    [SerializeField] private Sprite icon = default;

    // Replace later with actual representation/animations/behaviors
    [SerializeField] private Sprite representation = default;

    private void OnEnable()
    {
        foreach (NeedTypeConstructData needData in needsList)
        {
            foreach (NeedConstructData need in needData.Needs)
            {
                // Use the NeedData to create Need
                Needs.Add(need.NeedName, new Need(needData.NeedType, need));
            }
        }
        while (this.needBehaviorSet.Count < 1)
        {
            this.needBehaviorSet.Add(new BehaviorScriptTranslation());
        }
        while (this.needBehaviorSet.Count > 1)
        {
            this.needBehaviorSet.RemoveAt(this.needBehaviorSet.Count - 1);
        }
    }

    public void OnValidate()
    {
        // // Calculate number of behaviors based off of unique need types
        // HashSet <NeedType> uniqueTypes = new HashSet<NeedType>();
        // int numBehaviors = 0;
        // foreach (Need need in this.needs.Values)
        // {
        //     if (!uniqueTypes.Contains(need.NType))
        //     {
        //         uniqueTypes.Add(need.NType);
        //         numBehaviors++;
        //     }
        // }

        // // Ensure there is a behavior for each needType
        // while (this.needBehaviorSet.Count < 0)
        // {
        //     this.needBehaviorSet.Add(new BehaviorScriptTranslation());
        // }
        // while (this.needBehaviorSet.Count > 1)
        // {
        //     this.needBehaviorSet.RemoveAt(this.needBehaviorSet.Count - 1);
        // }
        // // int i = 0;
        // // Give each behavior set the unique need type name. Prevents changing names
        // foreach (NeedType needType in uniqueTypes)
        // {
        //     this.needBehaviorSet[i].NeedType = needType;
        //     i++;
        // }

        // // Ensure there are no duplicate behaviors when behaviors are being chosen
        // HashSet<string> usedBehaviors = new HashSet<string>();
        // foreach (BehaviorScriptTranslation behavior in this.needBehaviorSet)
        // {
        //     if (!usedBehaviors.Contains(behavior.behaviorScriptName.ToString()))
        //     {
        //         usedBehaviors.Add(behavior.behaviorScriptName.ToString());
        //     }
        //     else
        //     {
        //         behavior.behaviorScriptName = BehaviorScriptName.None;
        //     }
        // }
    }
}

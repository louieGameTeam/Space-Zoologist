using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The scriptable object that stores all info shared by the plant species.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "Food/Species", order = 1)]
public class FoodScriptableObject : ScriptableObject
{
    [Tooltip("Amount of output if all needs are moderately met.")]
    [SerializeField] private float base_output = 0;
    public float BaseOutput { get => base_output; }

    //radius to be read
    [Tooltip("Radius of terrain and liquid tiles to read from. Synonymous to how far the plant's root reaches in real life. Warning: Setting this beyond 100 may cause significant lag!")]
    [SerializeField] private int radius = 0;
    public int Radius { get => radius; }


    [Tooltip("Scriptable Objects that store details on each specific needs. Drag and drop SO into the field and values will be updated spontaneously. To remove an" +
        " SO, set it to null.")]
    //Range scriptable objects to read in from
    [SerializeField]private NeedScriptableObject[] needs;
    public NeedScriptableObject[] Needs { get => needs; }


    //custom dictionary visible in inspector
    [Serializable]
    public struct Dict
    {
        public TileType type;
        public int value; //value of tile
    }
    [SerializeField] private Dict[] tileVal;

    //Workaround: dictionary to be initialized
    public Dictionary<TileType, int> TileDic { get; private set; }


    [Header("Read-Only: Values read from NeedSOs")]
    [SerializeField] private PlantNeedType[] types;
    public PlantNeedType[] Types { get => types; }
    [SerializeField] private float[] severities;
    public float[] Severities { get => severities; }
    [SerializeField] private float totalSeverity;
    public float TotalSeverity { get => totalSeverity; }


    //Gets called when value of scriptable object changes in the inspector, makes editing easier
    private void OnValidate(){
        if(tileVal.Length != 5)
            tileVal = new Dict[5];
        for (int i = 0; i < tileVal.Length; i++) {
            tileVal[i].type = (TileType)i;
        }
        //initialize the dictionary
        TileDic = new Dictionary<TileType, int>();
        for (int i = 0; i < tileVal.Length; i++)
        {
            TileDic.Add(tileVal[i].type, tileVal[i].value);
        }

        //sorting to make the interface cleaner
        List<NeedScriptableObject> temp = new List<NeedScriptableObject>(needs);
        temp.RemoveAll(item => item == null); //remove items that are null
        temp.Sort();
        needs = temp.ToArray();

        totalSeverity = 0;
        types = new PlantNeedType[needs.Length];
        severities = new float[needs.Length];

        for(int i = 0; i < needs.Length; i++){
            types[i] = needs[i].Type;
            severities[i] = needs[i].Severity;

            //negative weight will serve be for determining harmful environment to the plant
            if (severities[i] > 0)
            {
                totalSeverity += severities[i];
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurvivalObjectiveData
{
    // The species this objective cares about
    [SerializeField] public AnimalSpecies targetSpecies;
    // How many populations this objective needs
    public byte targetPopulationCount;
    // The populstion size this objective requires
    public byte targetPopulationSize;
    // The time (min) to hold this population size
    public float timeRequirement;
}

[System.Serializable]
public class ResourceObjectiveData
{
    // Have at least this amount when game is over
    public int amountToKeep;
}

[CreateAssetMenu]
public class LevelObjectiveData : ScriptableObject
{
    public List<SurvivalObjectiveData> survivalObjectiveDatas = default;
    public List<ResourceObjectiveData> resourceObjectiveDatas = default;
}

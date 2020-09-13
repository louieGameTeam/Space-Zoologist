using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType { Survival, Resource }

public class Objective
{
    public ObjectiveType ObjectiveType;
    private bool Status;
}

[System.Serializable]
public class SurvivalObjective : Objective
{
    // The species this objective cares about
    public AnimalSpecies targetSpecies;
    // How many populations this objective needs
    public byte targetPopulationCount;
    // The populstion size this objective requires
    public byte targetPopulationSize;
    // The time (min) to hold this population size
    public int timeRequirement;
}

[System.Serializable]
public class ResourceObjective : Objective
{
    // Have at least this amount when game is over
    public int amountToKeep;
}

[CreateAssetMenu]
public class LevelObjectives : ScriptableObject
{
    public List<SurvivalObjective> survivalObjective = default;
    public List<ResourceObjective> resourceObjective = default;
}

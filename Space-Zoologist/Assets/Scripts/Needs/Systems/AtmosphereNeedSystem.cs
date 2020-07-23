using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AtmosphereNeedSystem : NeedSystem
{
    //private readonly ReservePartitionManager rpm = null;
    private readonly EnclosureSystem enclosureSystem = null;
    public AtmosphereNeedSystem(EnclosureSystem enclosureSystem, string needName = "Atmosphere") : base(needName)
    {
        this.enclosureSystem = enclosureSystem;
    }

    /// <summary>
    /// Updates the density score of all the registered population and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
        if (this.Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        enclosureSystem.FindEnclosedAreas();

        foreach (Life life in Consumers)
        {
            // Get the atmospheric composition of a population 
            AtmosphericComposition atmosphericComposition = enclosureSystem.GetAtmosphericComposition(Vector3Int.FloorToInt(life.GetPosition()));

            // THe composition is a list of float value in the order of the AtmoshpereComponent Enum
            float[] composition = atmosphericComposition.GeComposition();

            foreach (var (value, index) in composition.WithIndex())
            {
                string needName = ((AtmoshpereComponent)index).ToString();

                if (life.GetNeedValues().ContainsKey(needName))
                {
                    life.UpdateNeed(needName, value);
                }
            }
        }

        this.isDirty = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalModifierManager : MonoBehaviour
{
    public HashSet<AnimalModifier> animalModifiers = new HashSet<AnimalModifier>();
    public bool AddModifier(AnimalModifier animalModifier)
    {
        if (!this.animalModifiers.Contains(animalModifier))
        {
            animalModifier.AddModifier(this.gameObject);
            this.animalModifiers.Add(animalModifier);
            return true;
        }
        return false;
    }

    public bool RemoveModifier(AnimalModifier animalModifier)
    {
        if (this.animalModifiers.Contains(animalModifier))
        {
            animalModifier.RemoveModifier(this.gameObject);
            this.animalModifiers.Remove(animalModifier);
            return true;
        }
        return false;
    }
}

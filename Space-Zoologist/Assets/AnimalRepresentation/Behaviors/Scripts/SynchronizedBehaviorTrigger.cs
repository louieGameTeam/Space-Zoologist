using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "SynchronizedBehavior", menuName = "SpeciesBehavior/SynchronizedBehavior")]
public class SynchronizedBehaviorTrigger : PopulationBehavior
{
    [SerializeField] private float[] synchronizedSteps = default;
    protected override void ProceedToNext(GameObject animal, List<GameObject> collaboratingAnimals, bool isDriven = false)
    {
        if (animalsToSteps[animal] < behaviorPatterns.Count) // exit behavior when all steps are completed
        {
            if (!isDriven && (synchronizedSteps == null || synchronizedSteps.Length == 0 || Array.Exists(synchronizedSteps, x => x == animalsToSteps[animal])))// Avoids infinite loop and select steps that 
            {
                foreach (GameObject otherAnimal in collaboratingAnimals)
                {
                    if (!animalsToSteps.ContainsKey(otherAnimal) || animalsToSteps[otherAnimal] != animalsToSteps[animal])
                    {
                        return;
                    }
                }
                // When the above loop completes without returning, it means that all animals are ready, then all animals should proceed to next step
                foreach (GameObject otherAnimal in collaboratingAnimals) //Calls other animals to proceed without checking completion, which collaborating animals defaults to null
                {
                    List<GameObject> otherAnimalCollabs = new List<GameObject>(collaboratingAnimals);
                    otherAnimalCollabs.Add(animal);
                    otherAnimalCollabs.Remove(otherAnimal);
                    this.ProceedToNext(otherAnimal, otherAnimalCollabs, true);
                }
            }
            animal.GetComponent<AnimalBehaviorManager>().AddBehaviorPattern(behaviorPatterns[animalsToSteps[animal]], stepCompletedCallback, alternativeCallback, collaboratingAnimals);
        }
        else
        {
            base.RemoveBehavior(animal);
        }
    }
}

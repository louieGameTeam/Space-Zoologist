using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAnimalsComeTogether : BehaviorPattern
{
    [SerializeField] private float totalDistanceCutoff = 3;
    [SerializeField] private float xDistanceCutoff = 1;
    Dictionary<GameObject, float> animalsToLastDistance = new Dictionary<GameObject, float>();
    Dictionary<GameObject, bool> animalsToIsClose = new Dictionary<GameObject, bool>();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        animalsToLastDistance.Add(gameObject, -1);
        animalsToIsClose.Add(gameObject, false);
        AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(gameObject.transform.position), base.GridSystem.Grid.WorldToCell(animalData.collaboratingAnimals[0].transform.position), AnimalsToAnimalData[gameObject].animal.MovementController.AssignPath, AnimalsToAnimalData[gameObject].animal.PopulationInfo.grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (animalData.animal.MovementController.HasPath)
        {
            animalData.animal.MovementController.MoveTowardsDestination();
        }
        PathFindToOtherAnimal(animal, animalData.collaboratingAnimals[0]);
        //Debug.Log(animalsToIsClose[animal] && animalData.animal.MovementController.DestinationReached);
        if (animalsToIsClose[animal] && animalData.animal.MovementController.DestinationReached)
        {
            return true;
        }
        return false;
    }
    private void PathFindToOtherAnimal(GameObject animal1, GameObject animal2)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(animal1.transform.position.x - animal2.transform.position.x, 2) - Mathf.Pow(animal1.transform.position.y - animal2.transform.position.y, 2));
        if (distance < totalDistanceCutoff && !animalsToIsClose[animal1])
        {
            float x;
            float y;
            if (animal1.transform.position.x < animal2.transform.position.x)
            {
                x = animal1.transform.position.x + Mathf.Abs(animal1.transform.position.x - animal2.transform.position.x) / 2 - (xDistanceCutoff / 2);
            }
            else
            {
                x = animal1.transform.position.x - Mathf.Abs(animal1.transform.position.x - animal2.transform.position.x) / 2 + (xDistanceCutoff / 2);
            }
            if (animal1.transform.position.y < animal2.transform.position.y)
            {
                y = animal1.transform.position.y + Mathf.Abs(animal1.transform.position.y - animal2.transform.position.y) / 2;
            }
            else
            {
                y = animal1.transform.position.y - Mathf.Abs(animal1.transform.position.y - animal2.transform.position.y) / 2;
            }
            Vector3Int originalPos = base.GridSystem.Grid.WorldToCell(animal1.transform.position);
            Vector3Int newPos = base.GridSystem.Grid.WorldToCell(new Vector3(x, y, animal1.transform.position.z));
            for (int i = 0; i < 2; i++)
            {
                if (newPos[i] != originalPos[i])
                {
                    //Debug.Log("recalculate");
                    AnimalPathfinding.PathRequestManager.RequestPath(originalPos, newPos, AnimalsToAnimalData[animal1].animal.MovementController.AssignPath, AnimalsToAnimalData[animal1].animal.PopulationInfo.grid);
                    break;
                }
            }
            animalsToIsClose[animal1] = true;
            return;
        }
        if (animalsToLastDistance[animal1] != -1 && distance > animalsToLastDistance[animal1]) // correct path if moving further away
        {
            animalsToLastDistance[animal1] = distance;
            AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(animal1.transform.position), base.GridSystem.Grid.WorldToCell(animal2.transform.position), AnimalsToAnimalData[animal1].animal.MovementController.AssignPath, AnimalsToAnimalData[animal1].animal.PopulationInfo.grid);
        }
        animalsToLastDistance[animal1] = distance;
    }
    protected override void ExitPattern(GameObject gameObject)
    {
        animalsToLastDistance.Remove(gameObject);
        animalsToIsClose.Remove(gameObject);
        base.ExitPattern(gameObject);
    }
}

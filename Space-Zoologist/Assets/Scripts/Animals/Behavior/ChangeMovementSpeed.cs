using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMovementSpeed : MonoBehaviour
{
    private AnimalBehavior _animalBehavior = AnimalBehavior.ChangeMovementSpeed;
    public AnimalBehavior animalBehavior => this._animalBehavior;
    [Range(0.0f, 2f)]
    [SerializeField] float MovementSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * this.MovementSpeed * Time.deltaTime);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class MovementTesting : MonoBehaviour
{
    [SerializeField] Population PopulationInfo = default;
    [SerializeField] float MovementSpeed = 1f;
    [SerializeField] bool RandomizeSpeed = false;
    [SerializeField] float PauseLength = 1f;
    private Transform Direction = default;
    private float PreviousDirectionAngle = default;
    private float NewDirectionAngle = default;
    private List<Vector3Int> AccessibleLocations = default;
    private Vector3Int Destination = default;
    private Vector3 CurrentPosition = default;
    private float LerpMoveTime = 0f;
    private float LerpDirectionTime = 0f;
    private float PauseTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        ReservePartitionManager.ins.AddPopulation(this.PopulationInfo);
        this.AccessibleLocations = ReservePartitionManager.ins.GetLocationWithAccess(this.PopulationInfo);
        this.CurrentPosition = this.transform.position;
        this.PreviousDirectionAngle = this.transform.rotation.z;
        this.PickRandomLocation();
        this.HandleDirectionChange();
    }

    // Update is called once per frame
    void Update()
    {
        this.MoveSprite();
    }

    private void MoveSprite() 
    {
        if (this.Paused())
        {
            this.HandleDirectionChange();
            this.PauseTime += Time.deltaTime;
            return;
        }
        else if (DestinationReached(this.Destination, this.transform.position))
        {
            this.CurrentPosition = this.transform.position;
            this.PreviousDirectionAngle = this.NewDirectionAngle;
            this.PickRandomLocation();
            this.CalculateNewDirectionAngle();
            this.LerpMoveTime = 0f;
            this.LerpDirectionTime = 0f;
            this.PauseTime = 0f;
            if (this.RandomizeSpeed)
            {
                this.PickRandomSpeed();
            }
        }
        else
        {
            this.MoveTowardsDestination();
        }
    }

    // TODO try different movement curves, slerp!
    private void MoveTowardsDestination()
    {
        this.LerpMoveTime += Time.deltaTime;
        if (this.LerpMoveTime > this.MovementSpeed) {
            this.LerpMoveTime = this.MovementSpeed;
        }
        float percent = this.LerpMoveTime / this.MovementSpeed;
        this.transform.position = Vector3.Lerp(this.CurrentPosition, this.Destination, percent);
    }

    private bool Paused()
    {
        return this.PauseLength > 0f && this.PauseTime < this.PauseLength;
    }

    private bool DestinationReached(Vector3Int destination, Vector3 currentLocation) 
    {
        return Mathf.Round(currentLocation.x) == destination.x && Mathf.Round(currentLocation.y) == destination.y;
    }

    private void PickRandomLocation()
    {
        var r = new System.Random();
        this.Destination = this.AccessibleLocations[r.Next(0, this.AccessibleLocations.Count)];
    }

    // TODO figure out how to calculate new direction angle and then properly lerp between previous angle
    private void HandleDirectionChange()
    {
        this.LerpDirectionTime += Time.deltaTime;
        if (this.LerpDirectionTime > this.PauseTime) {
            this.LerpDirectionTime = this.PauseTime;
        }
        float percent = this.LerpDirectionTime / this.PauseTime;
        float zValue = Mathf.Lerp(this.PreviousDirectionAngle, this.NewDirectionAngle, percent);
        this.transform.rotation = new Quaternion(0, 0, zValue, 0);
    }

    private void CalculateNewDirectionAngle()
    {
        this.NewDirectionAngle = Vector3.Angle(this.CurrentPosition, this.Destination);
        Debug.Log(this.NewDirectionAngle);
    }

    private void PickRandomSpeed()
    {
        var r = new System.Random();
        this.MovementSpeed = (float)r.NextDouble() + 0.75f;
    }
}

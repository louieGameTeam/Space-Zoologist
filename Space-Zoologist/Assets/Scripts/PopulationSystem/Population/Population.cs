using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// A runtime instance of a population.
/// </summary>
public class Population : MonoBehaviour
{
    public AnimalSpecies Species { get => species; }
    public int Count { get => AnimalPopulation.Count; }
    public float FoodDominance => Species.FoodDominance;
    public int PrePopulationCount => this.prePopulationCount;
    [HideInInspector]
    public bool HasAccessibilityChanged = false;
    public System.Random random = new System.Random();

    public AnimalPathfinding.Grid Grid { get; private set; }
    public List<Vector3Int> AccessibleLocations { get; private set; }
    public bool AccessibleLocationsExist { get => AccessibleLocations.Count > 0; }

    private float animatorSpeed = 1f;
    private float overlaySpeed = 1f;

    [Expandable] public AnimalSpecies species = default;
    [SerializeField] private GameObject AnimalPrefab = default;
    [Header("Add existing animals")]
    [SerializeField] public List<GameObject> AnimalPopulation = default;

    public GrowthCalculator GrowthCalculator;
    private PoolingSystem PoolingSystem = default;
    private int prePopulationCount = default;
    private PopulationBehaviorManager PopulationBehaviorManager = default;

    /// <summary>
    /// The number of liquid tiles that this population can drink from
    /// </summary>
    /// <remarks>
    /// This value is direclty set by the liquid need system because
    /// the liquid tiles need to be evenly distributed between all populations
    /// in the enclosure
    /// </remarks>
    public float drinkableLiquidTiles = 0;

    /// <summary>
    /// Initialize the population as the given species at the given origin after runtime.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin of the population</param>
    /// <param name="needSystemManager"></param>
    public void InitializeNewPopulation(AnimalSpecies species, Vector3 origin)
    {
        this.PopulationBehaviorManager = this.GetComponent<PopulationBehaviorManager>();
        this.species = species;
        this.transform.position = origin;
        this.GrowthCalculator = new GrowthCalculator(this);
        this.PoolingSystem = this.GetComponent<PoolingSystem>();
        this.PoolingSystem.AddPooledObjects(5, this.AnimalPrefab);
        this.GrowthCalculator = new GrowthCalculator(this);
    }

    /// <summary>
    /// Grabs the updated accessible area, then resets the behavior for all of the animals
    /// </summary>
    /// Could improve by checking what shape the accessible area is in
    public void UpdateAccessibleArea(List<Vector3Int> accessibleLocations, AnimalPathfinding.Grid grid)
    {
        this.AccessibleLocations = accessibleLocations;
        this.Grid = grid;
    }

    // Only pauses movements
    public void PauseAnimalsMovementController()
    {
        foreach(GameObject animal in this.AnimalPopulation)
        {
            // Tentative change to not freeze animations
            //PauseAnimatorComponent();
            animal.GetComponent<MovementController>().IsPaused = true;
            //animal.GetComponent<MovementController>().TryToCancelDestination();
        }
    }

    private void PauseAnimatorComponent(GameObject animal)
    {
        Animator animator = animal.GetComponent<Animator>();
        Animator overlay = animal.transform.GetChild(0).GetComponent<Animator>();
        if (animator.speed != 0)
        {
            this.animatorSpeed = animator.speed;
        }
        if (overlay.speed != 0)
        {
            this.overlaySpeed = overlay.speed;
        }
        animator.speed = 0;
        overlay.speed = 0;
    }

    public void UnpauseAnimalsMovementController()
    {
        foreach(GameObject animal in this.AnimalPopulation)
        {
            Animator animator = animal.GetComponent<Animator>();
            Animator overlay = animal.transform.GetChild(0).GetComponent<Animator>();
            animator.speed = this.animatorSpeed;
            overlay.speed = this.overlaySpeed;
            animal.GetComponent<MovementController>().IsPaused = false;
        }
    }

    public void InitializeExistingAnimals()
    {
        foreach (GameObject animal in this.AnimalPopulation)
        {
            if (animal.activeSelf)
            {
                MovementData data = new MovementData();
                animal.GetComponent<Animal>().Initialize(this, data);
            }
        }
        this.prePopulationCount = this.Count;
        this.PopulationBehaviorManager.Initialize();
    }

    // Add one because UpdateGrowthConditions updates this value independently of HandleGrowth
    public int DaysTillDeath()
    {
        if (GrowthCalculator.Rating.PredatorCount > 0) return 1;
        else return this.GrowthCalculator.DecayCountdown; 
    }

    // Don't add one because this value is updated when HandleGrowth is called
    public int DaysTillGrowth()
    {
        return this.GrowthCalculator.GrowthCountdown;
    }

    public bool HandleGrowth()
    {
        bool readyForGrowth;

        if (GrowthCalculator.ChangeRate > 0f)
        {
            readyForGrowth = this.GrowthCalculator.ReadyForGrowth();
            if (readyForGrowth)
            {
                //GrowthCalculator.populationIncreaseRate represents what percent of the population should be added on top of the existing population
                float populationIncreaseAmount = this.Count * GrowthCalculator.ChangeRate;
                for (int i = 0; i < populationIncreaseAmount; ++i)
                {
                    AddAnimal(FindValidPositionAroundCurrentAnimals());
                }
            }
        }
        else
        {
            readyForGrowth = this.GrowthCalculator.ReadyForDecay();
            if (readyForGrowth)
            {
                //GrowthCalculator.populationIncreaseRate represents what percent of the population should be removed from the existing population (as a negative number)
                float populationDecreaseAmount = this.Count * this.GrowthCalculator.ChangeRate * -1;
                for (int i = 0; i < populationDecreaseAmount; ++i)
                {
                    if (this.AnimalPopulation.Count == 0)
                        break;
                    this.RemoveAnimal(this.AnimalPopulation[0]);
                }
            }
        }

        return readyForGrowth;
    }

    /// <summary>
    /// Finds a valid spawning position around a random existing animal in the population
    /// </summary>
    /// <param name="position"></param>
    /// <param name="taxicabDistance"></param>
    public Vector3 FindValidPositionAroundCurrentAnimals()
    {
        // choose a random current animal as the source position
        Vector3Int sourcePos = AnimalPopulation[UnityEngine.Random.Range(0, Count - 1)].transform.position.ToVector3Int();
        // select a random tile from 3x3 around the animal
        var possibleSpawnPos = new List<Vector3Int>();
        for(int y = -1;y <= 1;y++)
        {
            for(int x = -1;x <= 1;x++)
            {
                Vector3Int position = sourcePos + new Vector3Int(x, y, 0);
                if(AccessibleLocations.Contains(position))
                    possibleSpawnPos.Add(position);
            }
        }
        sourcePos = possibleSpawnPos[UnityEngine.Random.Range(0, possibleSpawnPos.Count - 1)];
        // random position inside the chosen tile
        Vector3 finalPos = sourcePos + new Vector3(0.5f, 0.5f, 0);
        finalPos += (0.4f * UnityEngine.Random.insideUnitCircle).ToVector3();
        return finalPos;
    }

    public void AddAnimal(Vector3 position)
    {
        MovementData data = new MovementData();
        data.CurrentDirection = Direction.left;
        GameObject newAnimal = this.PoolingSystem.GetGuaranteedPooledObject(this.AnimalPrefab);
        newAnimal.transform.position = position;
        newAnimal.GetComponent<Animal>().Initialize(this, data);
        this.PopulationBehaviorManager.AddAnimal(newAnimal);
        this.AnimalPopulation.Add(newAnimal);
        // Invoke a population growth event
        EventManager.Instance.InvokeEvent(EventType.PopulationCountChange, (this, true));
    }

    // removes last animal in list and last behavior
    public void RemoveAnimal(GameObject animal)
    {
        if (this.Count == 0)
        {
            Debug.Log(this.gameObject.name + " population already exitinct");
            return;
        }
        if (this.Count > 0)
        {
            Debug.Log("Animal removed");
            this.AnimalPopulation.Remove(animal);
            if (this.Count == 0)
            {
                EventManager.Instance.InvokeEvent(EventType.PopulationExtinct, this);
            }
            else
            {
                // Invoke a population decline event
                EventManager.Instance.InvokeEvent(EventType.PopulationCountChange, (this, false));
            }
            // Despawn instead of remove, since the gameobject may persist to play despawn behaviors
            this.PopulationBehaviorManager.SetDespawnCallback(DisableAnimalGameObject);
            this.PopulationBehaviorManager.StartDespawnAnimal(animal);
            AudioManager.instance.PlayOneShot(SFXType.AnimalDespawn);
            //Debug.Log ("Animal removed; new population count: " + Count);
        }
    }

    public void DisableAnimalGameObject(GameObject animal)
    {
        animal.SetActive(false);
        this.PoolingSystem.ReturnObjectToPool(animal);
    }

    /// <summary>
    /// Debug function to remove all animals
    /// </summary>
    public void RemoveAll()
    {
        for (int i=this.Count - 1; i>=0; i--)
        {
            this.RemoveAnimal(this.AnimalPopulation[i]);
        }
    }

    public float GetTerrainDominance(TileType tile)
    {
        return Species.GetTerrainDominance(tile);
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    public bool GetAccessibilityStatus()
    {
        return this.HasAccessibilityChanged;
    }

    public void UpdatePopulationStateForChecking()
    {
        this.HasAccessibilityChanged = false;
        this.prePopulationCount = this.Count;
    }
}

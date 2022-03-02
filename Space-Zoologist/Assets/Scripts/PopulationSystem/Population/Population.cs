using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// A runtime instance of a population.
/// </summary>
public class Population : MonoBehaviour, Life
{
    public AnimalSpecies Species { get => species; }
    public int Count { get => this.enclosedPopulationCount; }
    public float FoodDominance => FoodSourceNeedSystem.foodDominanceRatios[species.ID]; //* Count;
    public int PrePopulationCount => this.prePopulationCount;
    public Vector3 Origin => this.origin;
    public bool IsPaused => this.isPaused;
    public int TotalWaterTilesRequired => Count * Species.WaterTilesRequired;
    public float WaterTilesUsed => Mathf.Min(TotalWaterTilesRequired, drinkableLiquidTiles);
    public bool NeededWaterTilesPresent => drinkableLiquidTiles >= TotalWaterTilesRequired;
    [HideInInspector]
    public bool HasAccessibilityChanged = false;
    public System.Random random = new System.Random();

    public Dictionary<ItemID, Need> Needs => needs;
    public AnimalPathfinding.Grid Grid { get; private set; }
    public List<Vector3Int>  AccessibleLocations { get; private set; }

    public GrowthStatus GrowthStatus => this.GrowthCalculator.GrowthStatus;
    private float animatorSpeed = 1f;
    private float overlaySpeed = 1f;

    [Expandable] public AnimalSpecies species = default;
    [SerializeField] private GameObject AnimalPrefab = default;
    [Header("Add existing animals")]
    [SerializeField] public List<GameObject> AnimalPopulation = default;
    [Header("Lowest Priority Behaviors")]
    [Header("Modify values and thresholds for testing")]
    [SerializeField] private List<Need> NeedEditorTesting = default;
    [SerializeField] private Dictionary<Animal, MovementData> AnimalsMovementData = new Dictionary<Animal, MovementData>();

    private Dictionary<ItemID, Need> needs = new Dictionary<ItemID, Need>();

    private Vector3 origin = Vector3.zero;
    public GrowthCalculator GrowthCalculator;
    private PoolingSystem PoolingSystem = default;
    private int prePopulationCount = default;
    private PopulationBehaviorManager PopulationBehaviorManager = default;
    private bool isPaused = false;
    private int enclosedPopulationCount = default;

    /// <summary>
    /// The number of liquid tiles that this population can drink from
    /// </summary>
    /// <remarks>
    /// This value is direclty set by the liquid need system because
    /// the liquid tiles need to be evenly distributed between all populations
    /// in the enclosure
    /// </remarks>
    public float drinkableLiquidTiles = 0;

    private void Start()
    {
        this.PoolingSystem = this.GetComponent<PoolingSystem>();
    }

    /// <summary>
    /// Initialize the population as the given species at the given origin after runtime.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin of the population</param>
    /// <param name="needSystemManager"></param>
    public void InitializeNewPopulation(AnimalSpecies species, Vector3 origin)
    {
        this.PopulationBehaviorManager = this.GetComponent<PopulationBehaviorManager>();
        this.PoolingSystem = this.GetComponent<PoolingSystem>();
        this.species = species;
        this.origin = origin;
        this.transform.position = origin;
        this.GrowthCalculator = new GrowthCalculator(this);
        this.PoolingSystem = this.GetComponent<PoolingSystem>();
        this.PoolingSystem.AddPooledObjects(5, this.AnimalPrefab);
        this.SetupNeeds();
    }
    public void LoadGrowthRate(float growthRate)
    {
        this.GrowthCalculator.populationIncreaseRate = growthRate;
    }

    private void SetupNeeds()
    {
        this.GrowthCalculator = new GrowthCalculator(this);
        this.needs = this.Species.SetupNeeds();
        this.NeedEditorTesting = new List<Need>();
        foreach (KeyValuePair<ItemID, Need> need in this.needs)
        {
            this.NeedEditorTesting.Add(need.Value);
            this.GrowthCalculator.setupNeedTracker(need.Value.NeedType);
        }
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

    public void RecountAnimals () 
    {
        enclosedPopulationCount = AnimalPopulation.Count;
        // Only counts animals that are in an enclosure that is completely enclosed
        foreach (GameObject animal in AnimalPopulation) {

            // Get my grid position
            Vector3Int myGridPosition = new Vector3Int(
                (int)animal.transform.position.x,
                (int)animal.transform.position.y,
                (int)animal.transform.position.z);

            // Get the enclosed area I'm enclosed in
            EnclosedArea myArea = GameManager
                .Instance
                .m_enclosureSystem
                .GetEnclosedAreaByCellPosition(myGridPosition);

            // If the area is not enclosed, then reduce the number of enclosed animals
            // WHY?!
            // Simply commenting this out fixes a bug. Is there any reason for this at all?
            if (!myArea.isEnclosed) 
            {
                // enclosedPopulationCount--;
            }
        }
        //Debug.Log ("Population Count: " + enclosedPopulationCount);
    }

    // Only pauses movements
    public void PauseAnimalsMovementController()
    {
        foreach(GameObject animal in this.AnimalPopulation)
        {
            this.isPaused = true;
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
            animal.GetComponent<MovementController>().IsPaused = true;
            //animal.GetComponent<MovementController>().TryToCancelDestination();
        }
    }

    public void UnpauseAnimalsMovementController()
    {
        foreach(GameObject animal in this.AnimalPopulation)
        {
            this.isPaused = false;
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
                this.AnimalsMovementData.Add(animal.GetComponent<Animal>(), data);
                animal.GetComponent<Animal>().Initialize(this, data);
            }
        }
        this.prePopulationCount = this.Count;
        this.PopulationBehaviorManager.Initialize();
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(ItemID need, float value)
    {
        Debug.Assert(this.needs.ContainsKey(need), $"{ species.ID } population has no need { need }");
        this.needs[need].UpdateNeedValue(value);
        // Debug.Log($"The { species.SpeciesName } population { need } need has new value: {this.needs[need].NeedValue}");
    }

    public void UpdateFoodNeed(float preferredValue, float compatibleValue)
    {
        this.GrowthCalculator.CalculateFoodNeed(preferredValue, compatibleValue);
    }

    /// <summary>
    /// Get the value of the given need.
    /// </summary>
    /// <param name="need">The need to get the value of</param>
    /// <returns></returns>
    public float GetNeedValue(ItemID need)
    {
        Debug.Assert(this.needs.ContainsKey(need), $"{ species.ID } population has no need { need }");
        return this.needs[need].NeedValue;
    }

    public List<NeedType> GetUnmentNeeds()
    {
        List<NeedType> needStatus = new List<NeedType>();
        foreach (KeyValuePair<NeedType, bool> need in this.GrowthCalculator.IsNeedMet)
        {
            if (!need.Value)
            {
                needStatus.Add(need.Key);
            }
        }
        return needStatus;
    }

    public bool IsStagnate()
    {
        return this.GrowthCalculator.populationIncreaseRate == 0;
    }

    // Add one because UpdateGrowthConditions updates this value independently of HandleGrowth
    public int DaysTillDeath()
    {
        return this.GrowthCalculator.DecayCountdown;
    }

    // Don't add one because this value is updated when HandleGrowth is called
    public int DaysTillGrowth()
    {
        return this.GrowthCalculator.GrowthCountdown;
    }

    /// <summary>
    /// Calculate growth, then remove or add animals as needed.
    /// </summary>
    public void UpdateGrowthConditions()
    {
        if (this.Species == null) return;
        this.GrowthCalculator.CalculateGrowth();
    }

    public bool HandleGrowth()
    {
        bool readyForGrowth = false;
        switch (this.GrowthCalculator.GrowthStatus)
        {
            case GrowthStatus.growing:
                readyForGrowth = this.GrowthCalculator.ReadyForGrowth();
                if (readyForGrowth)
                {
                    //GrowthCalculator.populationIncreaseRate represents what percent of the population should be added on top of the existing population
                    float populationIncreaseAmount = this.Count * this.GrowthCalculator.populationIncreaseRate;
                    for (int i = 0; i < populationIncreaseAmount; ++i)
                    {
                        this.AddAnimal(this.gameObject.transform.position);
                    }
                }
                break;
            case GrowthStatus.declining:
                readyForGrowth = this.GrowthCalculator.ReadyForDecay();
                if (readyForGrowth)
                {
                    //GrowthCalculator.populationIncreaseRate represents what percent of the population should be removed from the existing population (as a negative number)
                    float populationDecreaseAmount = this.Count * this.GrowthCalculator.populationIncreaseRate * -1;
                    for (int i = 0; i < populationDecreaseAmount; ++i)
                    {
                        this.RemoveAnimal(this.AnimalPopulation[i]);
                    }
                }
                break;
            default:
                break;
        }
        RecountAnimals ();
        return readyForGrowth;
    }

    public void AddAnimal(Vector3 position)
    {
        MovementData data = new MovementData();
        GameObject newAnimal = this.PoolingSystem.GetPooledObject(this.AnimalPopulation);

        if (newAnimal == null)
        {
            this.PoolingSystem.AddPooledObjects(5, this.AnimalPrefab);
            newAnimal = this.PoolingSystem.GetPooledObject(this.AnimalPopulation);
        }
        this.AnimalsMovementData.Add(newAnimal.GetComponent<Animal>(), data);

        newAnimal.transform.position = position;
        newAnimal.GetComponent<Animal>().Initialize(this, data);
        this.PopulationBehaviorManager.animalsToExecutionData.Add(newAnimal, new BehaviorExecutionData(0));
        this.PopulationBehaviorManager.OnBehaviorComplete(newAnimal);
        RecountAnimals ();
        // Invoke a population growth event
        EventManager.Instance.InvokeEvent(EventType.PopulationCountIncreased, this);
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
            this.AnimalsMovementData.Remove(animal.GetComponent<Animal>());
            this.PopulationBehaviorManager.RemoveAnimal(animal);
            animal.SetActive(false);

            this.PoolingSystem.ReturnObjectToPool(animal);
            this.AnimalPopulation.Remove(animal);
            RecountAnimals ();
            if (this.Count == 0)
            {
                EventManager.Instance.InvokeEvent(EventType.PopulationExtinct, this);
            }
            else
            {
                // Invoke a population decline event
                EventManager.Instance.InvokeEvent(EventType.PopulationCountDecreased, this);
            }
            //Debug.Log ("Animal removed; new population count: " + Count);
        }
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
        RecountAnimals ();
    }

    // Ensure there are enough behavior data scripts mapped to the population size
    void OnValidate()
    {
        // while (this.AnimalsMovementData.Count < this.AnimalPopulation.Count)
        // {
        //     this.AnimalsMovementData.Add(new MovementData());
        // }
        // while (this.AnimalsMovementData.Count > this.AnimalPopulation.Count)
        // {
        //     this.AnimalsMovementData.RemoveAt(this.AnimalsMovementData.Count - 1);
        // }
        //if (this.GrowthCalculator != null)
        //{
        //    this.UpdateEditorNeeds();
        //    this.UpdateGrowthConditions();
        //}
    }

    private void UpdateEditorNeeds()
    {
        // Debug.Log("Needs updated with editor");
        int i=0;
        foreach (KeyValuePair<ItemID, Need> need in this.needs)
        {
            if (this.NeedEditorTesting[i].ID.Equals(need.Value.ID))
            {
                this.NeedEditorTesting[i] = need.Value;
            }
            i++;
        }
        if (this.NeedEditorTesting != null)
        {
            foreach (Need need in this.NeedEditorTesting)
            {
                this.needs[need.ID] = need;
            }
        }
    }

    public Dictionary<ItemID, Need> GetNeedValues()
    {
        return this.Needs;
    }

    public bool HasWaterNeeds()
    {
        return Needs.Keys.Any(id => id.IsWater);
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

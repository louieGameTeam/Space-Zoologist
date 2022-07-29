using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance = null;

    #region Private Fields
    [SerializeField] private VFXLibrary VFXLibrary;
    private Dictionary<VFXType, VFXLibrary.VFXObject> VFXDict;
    private bool CanDisplayFoodQualityFX = true;
    #endregion

    #region Constants
    private int FOOD_QUALITY_VFX_INTERVAL = 15; // Interval of time between checks for displaying food quality VFX
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        LoadVFX();
    }

    private void Update()
    {
        if (CanDisplayFoodQualityFX)
        {
            StartCoroutine(DisplayFoodQualityVFX(FOOD_QUALITY_VFX_INTERVAL));
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Displays requested VFX at given pos and plays related SFX, if not SFXType.NumTypes
    /// </summary>
    /// <param name="pos"> Position to display VFX at </param>
    /// <param name="vfxtype"> Type of VFX to display </param>
    public void DisplayVFX(Vector3 pos, VFXType type)
    {
        VFXLibrary.VFXObject vfx = VFXDict[type];
        VisualEffect visualEffect = Instantiate(vfx.effect);
        visualEffect.transform.position = pos;

        if (vfx.sfxType != SFXType.NumTypes)
        {
            AudioManager.instance.PlayOneShot(vfx.sfxType);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Every checkInterval seconds, selects an individual from an animal population to display VFX for.
    /// Displays food quality VFX along with any included SFX every checkInterval seconds
    /// </summary>
    /// <param name="checkInterval"> Number of seconds to wait between checks for displaying food quality fx </param>
    /// <returns></returns>
    private IEnumerator DisplayFoodQualityVFX(int checkInterval)
    {
        CanDisplayFoodQualityFX = false;

        if (GameManager.Instance != null)
        {
            foreach (AnimalSpecies species in GameManager.Instance.AnimalSpecies.Values)
            {
                List<Population> populationList = GameManager.Instance.m_populationManager.GetPopulationsBySpecies(species);
                foreach (Population population in populationList)
                {
                    GameObject selectedAnimal = population.AnimalPopulation[Random.Range(0, population.AnimalPopulation.Count)];
                    DisplayVFX(selectedAnimal.transform.position, VFXType.GoodFood);
                }
            }
        }

        yield return new WaitForSeconds(checkInterval);
        CanDisplayFoodQualityFX = true;
    }

    private void LoadVFX()
    {
        VFXDict = new Dictionary<VFXType, VFXLibrary.VFXObject>();
        foreach (VFXLibrary.VFXObject vfx in VFXLibrary.VisualEffects)
        {
            VFXDict.Add(vfx.type, vfx);
        }
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the display of VFXObjects
/// </summary>
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance = null;

    #region Private Fields
    [SerializeField] private VFXLibrary VFXLibrary = null;
    private Dictionary<VFXType, VFXLibrary.VFXObject> VFXDict;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        LoadVFX();
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
        VisualEffectPlayer visualEffect = Instantiate(vfx.effect);
        visualEffect.transform.position = pos;

        // TODO: Decouple VFX from SFX
        if (vfx.sfxType != SFXType.NumTypes)
        {
            AudioManager.instance.PlayOneShot(vfx.sfxType, 0.6f);
        }
    }
    #endregion

    #region Private Methods
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
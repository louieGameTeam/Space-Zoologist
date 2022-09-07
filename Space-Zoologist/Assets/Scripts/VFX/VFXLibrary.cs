using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum VFXType
{
    GoodFood, NeutralFood, BadFood,
    NumTypes
}

[CreateAssetMenu]
public class VFXLibrary : ScriptableObject
{
    /// <summary>
    /// Container for VFX objects displaying identifying info and effects to be played.
    /// sfxType attribute is SFXType.NumTypes by default, but should be set to the correct
    /// SFXType to be played when the VFX is displayed.
    /// effect is a prefab for the VisualEffect to be instantiated when visual effect is displayed
    /// </summary>
    [System.Serializable]
    public class VFXObject
    {
        public VFXType type;
        public SFXType sfxType = SFXType.NumTypes;
        public VisualEffect effect;
    }

    public VFXObject[] VisualEffects => visualEffects;
    [SerializeField] private VFXObject[] visualEffects;

    private void OnValidate()
    {
        VFXObject[] temp = new VFXObject[(int)VFXType.NumTypes];
        foreach (VFXObject vfx in visualEffects)
        {
            if (vfx.type == VFXType.NumTypes) continue;
            temp[(int)vfx.type] = vfx;
        }

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] == null)
            {
                temp[i] = new VFXObject();
                temp[i].type = (VFXType)i;
            }
        }

        visualEffects = temp;
    }
}
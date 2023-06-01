using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    General,
    Unpause, Pause, NextDay,
    MenuOpen, MenuClose, Notification, TabSwitch,
    Cancel, Observation,
    Valid, Locked, Unavailable,
    BuildModeOpen, BuildModeClose, BuildModeTabSwitch,
    NotebookOpen, NotebookClose, NotebookDropdown, NotebookTabSwitch,
    GoodFood, NeutralFood, BadFood,
    AnimalDespawn,
    LevelFinishWin, LevelFinishFail, Fireworks,
    DialogueHover, DialogueSelect,
    Sell,
    NumTypes
}

[CreateAssetMenu]
public class SFXLibrary : ScriptableObject
{
    /// <summary>
    /// A contianer for Audioclips that keeps track of the index of the next clip that should be played.
    /// Used for SFX
    /// </summary>
    [System.Serializable]
    public class AudioObject
    {
        [System.NonSerialized] public int index = 0;
        public SFXType type;
        public AudioClip[] clips;
    }

    public AudioObject[] SoundEffects => soundEffects;
    [SerializeField] AudioObject[] soundEffects;

    private void OnValidate()
    {
        AudioObject[] temp = new AudioObject[(int)SFXType.NumTypes];
        foreach (AudioObject audio in soundEffects)
        {
            if (audio.type == SFXType.NumTypes) continue;
            temp[(int)audio.type] = audio;
        }

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] == null)
            {
                temp[i] = new AudioObject();
                temp[i].type = (SFXType)i;
            }
        }
        soundEffects = temp;
    }

    public void PlayGeneral() {
        AudioManager.instance.PlayOneShot(SFXType.General);
    }
}
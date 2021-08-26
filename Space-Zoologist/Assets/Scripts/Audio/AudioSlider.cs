using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : Slider
{
    public void UpdateMasterVolume(float vol)
    {
        AudioManager.instance.SetMasterVolume(vol);
    }

    public void UpdateMusicVolume(float vol)
    {
        AudioManager.instance.SetMusicVolume(vol);
    }

    public void UpdateSFXVolume(float vol)
    {
        AudioManager.instance.SetSFXVolume(vol);
    }
}

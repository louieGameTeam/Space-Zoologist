using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSFXPlayer : MonoBehaviour
{
    [SerializeField] private SFXType audioType;
    [SerializeField] private bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayAudioOneshot();
        }
    }

    public void PlayAudioOneshot()
    {
        AudioManager.instance.PlayOneShotRandom(audioType);
    }

    public void PlayAudioOneshotQual(bool qualifier)
    {
        if(qualifier)
            AudioManager.instance.PlayOneShotRandom(audioType);
    }
    
    public void PlayAudioOneshotQualInvert(bool qualifier)
    {
        if(!qualifier)
            AudioManager.instance.PlayOneShotRandom(audioType);
    }
}

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
        AudioManager.instance.PlayOneShot(audioType);
    }
}

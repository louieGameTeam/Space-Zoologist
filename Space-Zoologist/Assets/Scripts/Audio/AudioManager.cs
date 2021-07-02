using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource SFX => sfx;
    public MusicManager MusicManager => musicManager;
    [SerializeField] MusicManager musicManager;
    [SerializeField] AudioSource sfx;

    float masterVolume = 1;

    private void Awake()
    {
        if (instance) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        musicManager = GetComponentInChildren<MusicManager>();
        if (musicManager == null) {
            musicManager = transform.GetChild(0).gameObject.AddComponent<MusicManager>();
        }
    }
    public void SetMasterVolume(float vol) {
        masterVolume = vol;
    }

    public void SetMusicVolume(float vol)
    {
        musicManager.SetVolume(masterVolume * vol);
    }

    public void SetSFXVolume(float vol)
    {
        sfx.volume = masterVolume * vol;
    }

    public void PlayOneShot(AudioClip clip) {
        // introduce a little bit of variety
        sfx.PlayOneShot(clip, Random.value * 0.2f + 0.8f);
    }
}

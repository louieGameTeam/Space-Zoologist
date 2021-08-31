using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;
    public MusicManager MusicManager => musicManager;
    public AudioSource SFX => sfx;
    [SerializeField] MusicManager musicManager;
    [SerializeField] AudioSource sfx;
    float masterVolume = 1;
    float musicVolume = 1;
    float sfxVolume = 1;

    [SerializeField] SFXLibrary SFXLibrary;
    Dictionary<SFXType, SFXLibrary.AudioObject> SFXDict;


    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        musicManager = GetComponentInChildren<MusicManager>();

        //initialized from prefab
        if (musicManager == null)
        {
            if (transform.childCount > 0)
            {
                musicManager = transform.GetChild(0).gameObject.AddComponent<MusicManager>();
            }
            else
            {
                musicManager = new GameObject("Music Manager", typeof(MusicManager)).GetComponent<MusicManager>();
                musicManager.transform.SetParent(transform);
            }
        }
    }

    private void Start()
    {
        //TODO Load Volume Settings from player preferences

        LoadSFX();
        UpdateVolume();
    }

    public void LoadSFX()
    {
        SFXDict = new Dictionary<SFXType, SFXLibrary.AudioObject>();
        foreach (var audio in SFXLibrary.SoundEffects)
        {
            SFXDict.Add(audio.type, audio);
        }
    }

    public void SetMasterVolume(float vol)
    {
        masterVolume = vol;
        UpdateVolume();
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = vol;
        UpdateVolume();
    }

    public void SetSFXVolume(float vol)
    {
        sfxVolume = vol;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        musicManager.SetVolume(masterVolume * musicVolume);
        sfx.volume = masterVolume * sfxVolume;
    }

    /// <summary>
    /// Play the audioclip.
    /// </summary>
    /// <param name="clip"></param>
    public void PlayOneShot(AudioClip clip)
    {
        // introduce a little bit of variety
        sfx.PlayOneShot(clip, Random.value * 0.2f + 0.8f);
    }

    /// <summary>
    /// Play the sfx from SFXType in order. If there is only one, repeat that sound each time this is called.
    /// </summary>
    /// <param name="type"></param>
    public void PlayOneShot(SFXType type)
    {
        if (type == SFXType.None) return;
        // temporary
        //var audio = SFXDict[type];
        //PlayOneShot(audio.clips[audio.index]);

        //audio.index++;
        //if (audio.index == audio.clips.Length) audio.index = 0;
    }

    /// <summary>
    /// Pick a random sound to play from in the SFXType
    /// </summary>
    /// <param name="type"></param>
    public void PlayOneShotRandom(SFXType type)
    {
        if (type == SFXType.None) return;
        var audio = SFXDict[type];

        int index = Random.Range(0, audio.clips.Length);
        PlayOneShot(audio.clips[index]);
    }
}

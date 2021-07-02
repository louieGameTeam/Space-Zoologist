using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource Music;
    [SerializeField] AudioSource SFX;

    Queue<AudioClip> musicQueue;

    private void Awake()
    {
        if (instance) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        musicQueue = new Queue<AudioClip>();
    }

    public void QueueMusic(AudioClip toQueue) {
        musicQueue.Enqueue(toQueue);
    }

    private void Update()
    {
        if (!Music.isPlaying) {
            Music.clip = musicQueue.Dequeue();
            Music.Play();
        }
    }
}

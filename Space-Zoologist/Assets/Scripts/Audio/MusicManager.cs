using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public LoopableAudioTrack preTransitionMusic;
    public LoopableAudioTrack postTransitionMusic;

    public AudioSource preTransitionMusicSource;
    public AudioSource postTransitionMusicSource;

    bool isExitingPrologue;

    const int TEMPO = 112;                                      // tempo of track, in beats per minute
    const int BEATS_PER_BAR = 2;                                // the tracks have 4 beats per bar, but I'm allowing half-bar transitions
    const float SECONDS_PER_BAR = BEATS_PER_BAR * 60f / TEMPO;  // (beats / bars) * (60 seconds / 1 minute) * (minutes / beats)

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        isExitingPrologue = false;
        if(preTransitionMusic != null)
            preTransitionMusic.StartTrack(preTransitionMusicSource);
    }

    public void SetNextTrack(LoopableAudioTrack nextTrack) {
        if (preTransitionMusic == null)
        {
            preTransitionMusic = nextTrack;
            preTransitionMusic.StartTrack(preTransitionMusicSource);
        }
        else
            postTransitionMusic = nextTrack;
    }

    // !! CALL THIS TO SWITCH FROM PROLOGUE MUSIC TO MAIN MENU MUSIC !!
    public void StartTransition() {
        if (isExitingPrologue) return;
        if (preTransitionMusic == null)
        {
            postTransitionMusic.StartTrack(postTransitionMusicSource);
            return;
        }
        else if (postTransitionMusic == null) {
            return;
        }

        isExitingPrologue = true;

        float curPlayheadTime = preTransitionMusic.GetCurrentTime();
        int nextBar = Mathf.CeilToInt(curPlayheadTime / SECONDS_PER_BAR);
        float nextBarTime = nextBar * SECONDS_PER_BAR;
        StartCoroutine(MusicTransition(nextBarTime - curPlayheadTime));
    }

    // timed sequence of events to make the transition smooth
    IEnumerator MusicTransition(float delay) {
        float buffer = (float)LoopableAudioTrack.BUFFER + Time.deltaTime; //HACK to deal with start buffer

        delay -= buffer;
        if (delay < 0) delay += SECONDS_PER_BAR;

        yield return new WaitForSeconds(delay); // wait for the beat

        postTransitionMusic.StartTrack(postTransitionMusicSource); // start the main menu music

        float waitTime = SECONDS_PER_BAR * 1f; // wait for a full bar
        yield return new WaitForSeconds(waitTime);
        float fadeTime = SECONDS_PER_BAR * 1.25f; // fade for a bit longer

        // fade out the prologue
        float p = 0f;
        float startVolume = preTransitionMusicSource.volume;
        while (p < 1f) { // we only need to fade most of the way out
            preTransitionMusic.SetVolume(startVolume * (1 - p));
            p += Time.deltaTime / fadeTime;
            yield return null;
        }

        preTransitionMusic.StopTrack(); //wait for main menu to start before stopping prologue
        preTransitionMusicSource.volume = startVolume;

        preTransitionMusic = postTransitionMusic;
        postTransitionMusic = null;

        AudioSource temp = preTransitionMusicSource;
        preTransitionMusicSource = postTransitionMusicSource;
        postTransitionMusicSource = temp;

        isExitingPrologue = false;
    }
}

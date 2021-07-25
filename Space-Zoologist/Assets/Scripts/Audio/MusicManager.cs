using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public LoopableAudioTrack curMusic;
    public LoopableAudioTrack nextMusic;

    public AudioSource curMusicSource;
    public AudioSource nextMusicSource;

    [SerializeField] Image backgroundImage;

    bool isInTransition;

    const int TEMPO = 112;                                      // tempo of track, in beats per minute
    const int BEATS_PER_BAR = 2;                                // the tracks have 4 beats per bar, but I'm allowing half-bar transitions
    const float SECONDS_PER_BAR = BEATS_PER_BAR * 60f / TEMPO;  // (beats / bars) * (60 seconds / 1 minute) * (minutes / beats)

    float volume = 1f;

    Coroutine transition = null;

    private void Awake()
    {
        if (curMusicSource == null) {
            curMusicSource = gameObject.AddComponent<AudioSource>();
        }
        if (nextMusicSource == null) {
            nextMusicSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        isInTransition = false;
        if (!LoopableAudioTrack.IsEmpty(curMusic) && !curMusic.haveStarted) {
            curMusic.StartTrack(curMusicSource);
        }
    }

    public bool SetNextTrack(LoopableAudioTrack nextTrack, bool overwriteTransitioningMusic = false)
    {
        if (nextTrack == curMusic) return false;

        if (LoopableAudioTrack.IsEmpty(nextMusic) || !isInTransition)
        {
            nextMusic = nextTrack;
            return true;
        }
        else if (overwriteTransitioningMusic)
        {
            StopTransition();
            nextMusic = nextTrack;
            return true;
        }

        return false;
    }

    public void StopTransition() {
        nextMusic.StopTrack();
        StopCoroutine(transition);
        transition = null;
        isInTransition = false;
    }

    /// <summary>
    /// Starts a transition (instantaneous if withFading = true) to the next track with aligned tempo and bars.
    /// </summary>
    /// <param name="withFading">Whether the transition involves fading.</param>
    /// <param name="track">The track to play. Setting track will force a transition to start.</param>
    public void StartTransition(bool withFading, LoopableAudioTrack track = null)
    {
        if (track != null) {
            //Attempt to forcibly set the next track
            if (!SetNextTrack(track, true) && isInTransition)
            {
                // only true if nextTrack == track and is being transitioned
                return;
            }
        }

        if (LoopableAudioTrack.IsEmpty(nextMusic))
        {
            return;
        }
        else if (LoopableAudioTrack.IsEmpty(curMusic))
        {
            curMusic = nextMusic;
            nextMusic = null;
            curMusic.StartTrack(curMusicSource);
            return;
        }
        else if (nextMusic == curMusic)
        {
            nextMusic = null;
            return;
        }

        if (!withFading) {
            if (!LoopableAudioTrack.IsEmpty(nextMusic))
            {
                if (isInTransition)
                {
                    StopTransition();
                }

                curMusic.StopTrack(); //wait for main menu to start before stopping prologue
                curMusicSource.volume = volume;

                curMusic = nextMusic;
                nextMusic = null;

                curMusic.StartTrack(curMusicSource);
            }
            return;
        }

        if (isInTransition)
        {
            return;
        }

        isInTransition = true;

        float curPlayheadTime = curMusic.GetCurrentTime();
        int nextBar = Mathf.CeilToInt(curPlayheadTime / SECONDS_PER_BAR);
        float nextBarTime = nextBar * SECONDS_PER_BAR;
        transition = StartCoroutine(MusicTransition(nextBarTime - curPlayheadTime));
    }

    // timed sequence of events to make the transition smooth
    IEnumerator MusicTransition(float delay)
    {
        float buffer = (float)LoopableAudioTrack.BUFFER + Time.deltaTime; //HACK to deal with start buffer

        delay -= buffer;
        if (delay < 0) delay += SECONDS_PER_BAR;

        yield return new WaitForSeconds(delay); // wait for the beat

        nextMusic.StartTrack(nextMusicSource); // start the main menu music

        float waitTime = SECONDS_PER_BAR * 1f; // wait for a full bar
        yield return new WaitForSeconds(waitTime);
        float fadeTime = SECONDS_PER_BAR * 1.25f; // fade for a bit longer

        // fade out the prologue
        float p = 0f;
        while (p < 1f)
        { // we only need to fade most of the way out
            curMusic.SetVolume(volume * (1 - p));
            p += Time.deltaTime / fadeTime;
            yield return null;
        }

        curMusic.StopTrack(); //wait for main menu to start before stopping prologue
        curMusicSource.volume = volume;

        curMusic = nextMusic;
        nextMusic = null;

        // Switch cur and next music source to reuse code
        AudioSource temp = curMusicSource;
        curMusicSource = nextMusicSource;
        nextMusicSource = temp;

        isInTransition = false;
        transition = null;
    }


    public void SetVolume(float vol)
    {
        volume = vol;
        curMusicSource.volume = volume;
        nextMusicSource.volume = volume;
    }
}

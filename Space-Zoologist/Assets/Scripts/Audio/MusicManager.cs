using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public CustomMusicLoopController curMusic;
    public CustomMusicLoopController nextMusic;

    bool isInTransition;

    const int TEMPO = 112;                                      // tempo of track, in beats per minute
    public const int BEATS_PER_BAR = 4;                                // the tracks have 4 beats per bar, used to be 2 for half-bar transitions
    public const float SECONDS_PER_BAR = BEATS_PER_BAR * 60.0f / TEMPO;  // (beats / bars) * (60 seconds / 1 minute) * (minutes / beats)

    float volume = 1f;

    Coroutine transition = null;

    void Start()
    {
        isInTransition = false;
        if (curMusic != null && !curMusic.isPlaying)
        {
            curMusic.StartTrack();
        }
    }

    public bool SetNextTrack(CustomMusicLoopController nextTrack, bool overwriteTransitioningMusic = false)
    {
        if (nextMusic == null && nextTrack == curMusic || nextTrack == nextMusic) {
            return false;
        }

        if (nextMusic == null || !isInTransition)
        {
            if (nextMusic) Destroy(nextMusic.gameObject); // not needed anymore

            PrepareTrack(nextTrack);
            return true;
        }
        else if (overwriteTransitioningMusic)
        {
            StopTransition();
            Destroy(nextMusic.gameObject); // not needed anymore

            if (nextTrack == curMusic)
            {
                return false;
            }
            else
            {
                PrepareTrack(nextTrack);
                return true;
            }
        }

        return false;
    }

    private void PrepareTrack(CustomMusicLoopController nextTrack) {
        nextMusic = nextTrack;
        nextMusic.gameObject.name = nextMusic.Source.clip.name;
        nextMusic.transform.SetParent(transform);
        SetVolume(volume);
    }

    public void StopTransition()
    {
        nextMusic.StopTrack();
        StopCoroutine(transition);
        SetVolume(volume);
        transition = null;
        isInTransition = false;
    }

    /// <summary>
    /// Starts a transition (instantaneous if withFading = true) to the next track with aligned tempo and bars.
    /// </summary>
    /// <param name="withFading">Whether the transition involves fading.</param>
    /// <param name="track">The track to play. Setting track will force a transition to start.</param>
    /// <returns>returns -1 if no transition happened, 0 if it was instant, or the delay before the transition will start.</returns>
    public float StartTransition(bool withFading, CustomMusicLoopController track = null)
    {
        if (track != null)
        {
            //Attempt to forcibly set the next track
            if (!SetNextTrack(track, true) || isInTransition || track != nextMusic)
            {
                // failed to set a new track, abort
                return -1;
            }
        }

        if (nextMusic == null)
        {
            return -1;
        }
        else if (curMusic == null)
        {
            curMusic = nextMusic;
            nextMusic = null;
            curMusic.StartTrack();
            return 0;
        }
        else if (nextMusic == curMusic)
        {
            nextMusic = null;
            return -1;
        }

        if (!withFading)
        {
            if (nextMusic != null)
            {
                if (isInTransition)
                {
                    StopTransition();
                }

                curMusic.StopTrack(); //wait for main menu to start before stopping prologue
                curMusic.SetVolume(volume);
                Destroy(curMusic.gameObject); // not needed anymore

                curMusic = nextMusic;
                nextMusic = null;

                curMusic.StartTrack();
                return 0;
            }
            return -1;
        }

        if (isInTransition)
        {
            return -1;
        }

        isInTransition = true;

        float curPlayheadTime = curMusic.GetCurrentTime();
        int nextBar = Mathf.CeilToInt(curPlayheadTime / SECONDS_PER_BAR);
        float nextBarTime = nextBar * SECONDS_PER_BAR;
        transition = StartCoroutine(MusicTransition(nextBarTime - curPlayheadTime));
        return nextBarTime - curPlayheadTime;
    }

    // timed sequence of events to make the transition smooth
    IEnumerator MusicTransition(float delay)
    {
        yield return new WaitForSeconds(delay); // wait for the beat

		nextMusic.StartTrack();

        //float waitTime = SECONDS_PER_BAR * 0.5f; // wait for a full bar
		//yield return new WaitForSeconds(waitTime);

        float fadeTime = SECONDS_PER_BAR; // fade for a bit longer

        // fade out the prologue
		float p = 0f;
        float startVolume = curMusic.GetVolume();

        while (p < 1f)
        { // we only need to fade most of the way out
            curMusic.SetVolume(startVolume * (1 - p));
            p += Time.deltaTime / fadeTime;
            yield return null;
        }

        curMusic.StopTrack(); // wait for main menu to start before stopping prologue
        curMusic.SetVolume(startVolume); // revert the track to its original volume, just in case
        Destroy(curMusic.gameObject); // not needed anymore

        curMusic = nextMusic;
        nextMusic = null;

        isInTransition = false;
        transition = null;
    }


    public void SetVolume(float vol)
    {
        volume = vol;
        curMusic?.SetVolume(volume);
        nextMusic?.SetVolume(volume);
    }
}

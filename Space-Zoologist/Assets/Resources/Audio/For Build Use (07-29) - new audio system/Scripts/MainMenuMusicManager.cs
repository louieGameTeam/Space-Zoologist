using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controls the prologue -> main menu music transition.
///
/// I am assuming that both the prologueMusic AudioSource and the mainMenuMusic AudioSource
/// do NOT have PlayOnAwake set to true. Ultimately, all that needs to happen is for the
/// prologueMusic to start first, and for the mainMenuMusic to NOT be playing until after the
/// transition is indicated
/// </summary>

public class MainMenuMusicManager : MonoBehaviour {
    public CustomMusicLoopController prologueMusic;
    public CustomMusicLoopController mainMenuMusic;

    bool isExitingPrologue;

    const int TEMPO = 112;                                      // tempo of track, in beats per minute
    const int BEATS_PER_BAR = 2;                                // the tracks have 4 beats per bar, but I'm allowing half-bar transitions
    const float SECONDS_PER_BAR = BEATS_PER_BAR * 60f / TEMPO;  // (beats / bars) * (60 seconds / 1 minute) * (minutes / beats)

    //TODO: replace with the proper call
    void Start() {
        Initialize(true);
    }

    public void Initialize(bool startWithPrologue) {
        if (startWithPrologue) {
            isExitingPrologue = false;
            prologueMusic.StartTrack();
        } else {
            isExitingPrologue = true;
            mainMenuMusic.StartTrack();
        }
    }

    // !! CALL THIS TO SWITCH FROM PROLOGUE MUSIC TO MAIN MENU MUSIC !!
    public void StartMainMenuMusic() {
        if (isExitingPrologue) return;

        isExitingPrologue = true;

        float curPlayheadTime = prologueMusic.GetCurrentTime();
        int nextBar = Mathf.CeilToInt(curPlayheadTime / SECONDS_PER_BAR);
        float nextBarTime = nextBar * SECONDS_PER_BAR;
        StartCoroutine(MenuMusicTransition(nextBarTime - curPlayheadTime));
    }

    // timed sequence of events to make the transition smooth
    IEnumerator MenuMusicTransition(float delay) {
        yield return new WaitForSeconds(delay); // wait for the beat

        mainMenuMusic.StartTrack();

        float waitTime = SECONDS_PER_BAR * 0.5f; // wait for a full bar
        yield return new WaitForSeconds(waitTime);

        float fadeTime = SECONDS_PER_BAR * 0.5f; // fade for a bit longer

        // fade out the prologue
        float p = 0f;
        float startVolume = prologueMusic.GetVolume();

        while (p < 1f) { // we only need to fade most of the way out
            prologueMusic.SetVolume(startVolume * (1 - p));
            p += Time.deltaTime / fadeTime;
            yield return null;
        }

        prologueMusic.StopTrack(); // wait for main menu to start before stopping prologue
        prologueMusic.SetVolume(startVolume); // revert the track to its original volume, just in case
    }
}

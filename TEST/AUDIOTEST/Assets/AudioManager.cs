using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    double loopStartTime;   // the start of the portion of the track which is looped
    double loopEndTime;     // the end of the portion of the track which is looped

    AudioSource source;     // the audio source which contains the track to be looped
    AudioSource source2;    // the audio source used to allow custom looped tracks to loop seamlessly

    void Start () {
        source = GetComponent<AudioSource> ();
        source2 = transform.GetChild (0).GetComponent<AudioSource> ();

        loopEndTime = source.clip.length * 18f / 19f;
        loopStartTime = source.clip.length * 2f / 19f;

        source.PlayScheduled (AudioSettings.dspTime + 8D);
        source.SetScheduledEndTime (AudioSettings.dspTime + loopEndTime + 8D);

        source2.time = (float) loopStartTime;
        source2.PlayScheduled (AudioSettings.dspTime + loopEndTime + 8D);
        source2.SetScheduledEndTime (AudioSettings.dspTime + loopEndTime * 2D - loopStartTime + 8D);
    }

    void OnGUI () {
        GUI.TextArea (new Rect (10, 10, Screen.width - 10, 40), source.time.ToString () + ", " + source2.time.ToString () + ", " + loopEndTime);
    }
}

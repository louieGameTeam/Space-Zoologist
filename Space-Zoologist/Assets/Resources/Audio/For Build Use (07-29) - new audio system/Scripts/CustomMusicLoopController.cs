using UnityEngine;

/// <summary>
/// This is a track that contains a loop, but itself is not a loop. The track has a "start" section which
/// only plays once, and then the rest of the track repeats indefinitely.
///
/// This script should be placed on the GameObjec which has the music AudioSource on it. Currently,
/// I am assuming one AudioSource per GameObject, but that can easily be changed by making Initialize() public.
/// </summary>

[RequireComponent(typeof(AudioSource))]
public class CustomMusicLoopController : MonoBehaviour, System.IEquatable<CustomMusicLoopController>
{
    bool hasCustomLoopData;         // whether this audio clip has special looping behavior
    int loopLength;                 // the length, in samples, of the portion of the track which is looped
    int loopEndSample;              // the sample which indicates that we should loop back to the start of the loop

    AudioSource source;             // the audio source which contains the track to be looped

    public AudioSource Source => source;
    public bool isPlaying => source.isPlaying;

    void Awake()
    {
        AudioSource attachedSource = GetComponent<AudioSource>();
        Initialize(attachedSource);
    }

    void Initialize(AudioSource refSource)
    {
        source = refSource;
        string clipName = source.clip.name;

        // the clips are named as follows: "TRACK_NAME[LOOP_START-LOOP_END-NUM_BARS].wav"
        int openBracketIndex = clipName.IndexOf('['); // first, find the index of the '['
        hasCustomLoopData = openBracketIndex > -1; // if we don't find '[', assume we have no loop data

        if (hasCustomLoopData)
        {
            source.loop = false; // we handle looping on this script, so don't use the AudioSource's loop functionality

            int closeBracketIndex = clipName.IndexOf(']');
            if (closeBracketIndex == -1)
            { // if we don't find a close bracket, 
                Debug.LogError("Loop data is invalid: no ']' found!");
                return;
            }

            // start with the character after the open bracket, and before the close bracket
            string loopData = clipName.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            string[] tokens = loopData.Split('-');
            if (tokens.Length != 3)
            {
                Debug.LogError("Loop data is invalid: incorrect number of tokens found!");
                return;
            }

            int loopStartBar = int.Parse(tokens[0]);
            int loopEndBar = int.Parse(tokens[1]);
            int totalBarCount = int.Parse(tokens[2]);

            // the sample of the "loop end" is (loopEndBar / totalBarCount) of the way through the track
            loopEndSample = source.clip.samples * loopEndBar / totalBarCount;

            // the length of the loop is (loopEnd - "loop start")
            loopLength = loopEndSample - (source.clip.samples * loopStartBar / totalBarCount);
        }
        else
        {
            source.loop = true;
        }
    }

    void Update()
    {
        if (!source.isPlaying || !hasCustomLoopData) return;

        // once we have passed the end of the loop, go back to the start of the loop (automatically offset correctly)
        if (source.timeSamples > loopEndSample)
        {
            source.timeSamples -= loopLength;
        }
    }

    // start playing the track
    public void StartTrack()
    {
        if (source.isPlaying)
        {
            Debug.LogWarning("Trying to start a track that is already playing!");
            return;
        }

        source.Play();
        gameObject.name = "Now Playing: " + source.clip.name;
    }

    // stop playing the track
    public void StopTrack()
    {
        if (!source.isPlaying)
        {
            Debug.LogWarning("Trying to stop a track that isn't playing!");
            return;
        }

        source.Stop();
        gameObject.name = source.clip.name;
    }

    // pause the track
    public void PauseTrack()
    {
        if (!source.isPlaying)
        {
            Debug.LogWarning("Trying to pause a track that isn't playing!");
            return;
        }

        source.Pause();
        gameObject.name = "Paused: " + source.clip.name;
    }

    // unpause the track
    public void UnpauseTrack()
    {
        if (source.isPlaying)
        {
            Debug.LogWarning("Trying to unpause a track that is already playing!");
            return;
        }

        source.UnPause();
        gameObject.name = "Now Playing: " + source.clip.name;
    }

    // returns the realtime position of the track
    public float GetCurrentTime()
    {
        return source.time;
    }

    // changes the volume of this track
    public void SetVolume(float volume)
    {
        source.volume = volume;
    }

    // get the volume of this track
    public float GetVolume()
    {
        return source.volume;
    }

    public virtual bool Equals(CustomMusicLoopController other)
    {
        return source?.clip == other?.source?.clip;
    }
    public static bool operator ==(CustomMusicLoopController lhs, CustomMusicLoopController rhs)
    {
        return lhs?.source?.clip == rhs?.source?.clip;
    }

    public static bool operator !=(CustomMusicLoopController lhs, CustomMusicLoopController rhs)
    {
        return !(lhs == rhs);
    }
    public override bool Equals(object o)
    {
        return base.Equals(o);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

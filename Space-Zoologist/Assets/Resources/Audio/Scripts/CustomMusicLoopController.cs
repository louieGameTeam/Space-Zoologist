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
    [SerializeField] bool displayDebugInfo;
    bool hasCustomLoopData;         // whether this audio clip has special looping behavior
    bool usingPrimarySource = true; // whether source is currently playing instead of source2
    bool nextSourcePrepared;        // whether the next audio source has been reset
    float loopStartTime;            // the length, in samples, of the portion of the track which is looped
    float loopEndTime;              // the time which indicates that we should loop back to the start of the loop
    float loopPrepTime;             // the time which indicates that we should prep the next track to be looped

    double startedTime;
    float startedSourceTime;
    double scheduledEndTime;

    AudioSource source;             // the audio source which contains the track to be looped
    AudioSource source2;            // the audio source used to allow custom looped tracks to loop seamlessly

    public AudioSource Source => source;
    public bool isPlaying => source.isPlaying || source2.isPlaying;

    void Start()
    {
        AudioSource attachedSource = GetComponent<AudioSource>();
        Initialize(attachedSource);
    }

    public void Reinitialize() {
        Initialize(source);
    }

    void Initialize(AudioSource refSource)
    {
        source = refSource;
        if (source.clip == null)
        {
            if (GetComponent<MusicQueuer> ()) {
                Debug.Log ("Waiting for music to be queued");
            } else {
                Debug.LogWarning ("Abort: initializing an empty audio source!");
            }
            return;
        }
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
            loopEndTime = source.clip.length * loopEndBar / totalBarCount;
            //print (loopEndTime + ", " + source.clip.length);

            // the length of the loop is (loopEnd - "loop start")
            loopStartTime = source.clip.length * loopStartBar / totalBarCount;

            loopPrepTime = loopStartTime + (loopEndTime - loopStartTime) / 2;

            if (!transform.GetChild(0) || !transform.GetChild (0).GetComponent<AudioSource> ()) {
                Debug.LogError ("This object should have a child object with an audio source!");
                return;
            }
            source2 = transform.GetChild (0).GetComponent<AudioSource> ();
        }
        else
        {
            source.loop = true;
        }
    }

    void Update()
    {
        if (!isPlaying || !hasCustomLoopData) return;
        
        // One source has stopped playing and looped around, so it needs to be prepared to loop again
        if (!source.isPlaying || !source2.isPlaying) {
            PrepNextTrack ();
        }

        //print (source.time + ", " + source2.time);
    }

    // Tells the current track to end and next track to start at the loopEndTime
    void PrepNextTrack() 
    {
        if (usingPrimarySource) 
        {
            // Schedules the next source to be played once the current source hits the end of the loop
            source2.PlayScheduled (AudioSettings.dspTime + loopEndTime - source.time);
            // Schedules the current source to end at the end of the loop
            source2.SetScheduledEndTime (AudioSettings.dspTime + loopEndTime * 2 - source.time - loopStartTime);
            // Skips the next source ahead to the start of the loop
            source2.time = loopStartTime;

            startedTime = AudioSettings.dspTime;
            startedSourceTime = source.time;
            scheduledEndTime = AudioSettings.dspTime + loopEndTime - source.time;

            //print ("Prepped, will loop at " + (AudioSettings.dspTime + loopEndTime - source.time));
        } else 
        {
            // Schedules the next source to be played once the current source hits the end of the loop
            source.PlayScheduled (AudioSettings.dspTime + loopEndTime - source2.time);
            // Schedules the current source to end at the end of the loop
            source.SetScheduledEndTime (AudioSettings.dspTime + loopEndTime * 2 - source2.time - loopStartTime);
            // Skips the next source ahead to the start of the loop
            source.time = loopStartTime;

            startedTime = AudioSettings.dspTime;
            startedSourceTime = source2.time;
            scheduledEndTime = AudioSettings.dspTime + loopEndTime - source2.time;

            //print ("Prepped, will loop at " + (AudioSettings.dspTime + loopEndTime - source2.time));
        }

        usingPrimarySource = !usingPrimarySource;
    }

    // start playing the track
    public void StartTrack()
    {
        if (source.isPlaying || source2.isPlaying)
        {
            Debug.LogWarning("Trying to start a track that is already playing!");
            return;
        }

        // reset state, just in case
        source2.Stop ();
        source.time = 0;
        source2.time = 0;
        usingPrimarySource = true;
        nextSourcePrepared = false;

        source.PlayScheduled (AudioSettings.dspTime);
        source.SetScheduledEndTime (AudioSettings.dspTime + loopEndTime);
        //PrepNextTrack ();

        gameObject.name = "Now Playing: " + source.clip.name;

        //print ("Started at " + AudioSettings.dspTime + ", will loop at " + (AudioSettings.dspTime + loopEndTime));
    }

    // stop playing the track
    public void StopTrack()
    {
        if (!source.isPlaying && !source2.isPlaying)
        {
            Debug.LogWarning("Trying to stop a track that isn't playing!");
            return;
        }

        source.Stop();
        source2.Stop();
        gameObject.name = source.clip.name;
    }

    // pause the track
    public void PauseTrack()
    {
        if (!source.isPlaying && !source2.isPlaying)
        {
            Debug.LogWarning("Trying to pause a track that isn't playing!");
            return;
        }

        source.Pause();
        source2.Pause();
        gameObject.name = "Paused: " + source.clip.name;
    }

    // unpause the track
    public void UnpauseTrack()
    {
        if (source.isPlaying || source2.isPlaying)
        {
            Debug.LogWarning("Trying to unpause a track that is already playing!");
            return;
        }

        if (usingPrimarySource)
        {
            source2.UnPause ();
        } else
        {
            source.UnPause ();
        }
        gameObject.name = "Now Playing: " + source.clip.name;
    }

    // returns the realtime position of the track
    public float GetCurrentTime()
    {
        if (usingPrimarySource) {
            return source2.time;
        } else {
            return source.time;
        }
    }

    // changes the volume of this track
    public void SetVolume(float volume)
    {
        source.volume = volume;
        source2.volume = volume;
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

    void OnGUI () {
        if (displayDebugInfo) {
            GUI.TextArea (new Rect (10, 10, Screen.width - 10, 40), source.time.ToString () + ", " + source2.time.ToString () + ", " + usingPrimarySource);
            GUI.TextArea (new Rect (10, Screen.height - 40, Screen.width - 10, Screen.height - 10), startedTime + ", " + startedSourceTime + ", " + scheduledEndTime);
        }
    }
}

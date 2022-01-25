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
    float loopEndTime;              // the time which indicates that we should loop back to the start of the loop

    // TODO: Remove after done debugging
    double scheduledEndTime;

    AudioSource source;             // the audio source which contains the intro track
    AudioSource source2;            // the audio source which contains the main loop track

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

        // the clips are named as follows: "TRACK_NAME _ LOOP_INDICATOR.wav"
        int underscoreIndex = clipName.IndexOf('_'); // first, find the index of the '_'
        hasCustomLoopData = underscoreIndex > -1; // if we don't find '_', assume we have no loop data

        if (hasCustomLoopData)
        {
            source.loop = false;

            // get all characters before the underscore
            string trackTitle = clipName.Substring(0, underscoreIndex);

            loopEndTime = source.clip.length;

            if (!transform.GetChild(0) || !transform.GetChild (0).GetComponent<AudioSource> ()) {
                Debug.LogError ("This object should have a child object with an audio source!");
                return;
            }
            source2 = transform.GetChild (0).GetComponent<AudioSource> ();
            source2.clip = Resources.Load<AudioClip> ("Audio/Music/" + trackTitle + "_MainLoop");
            source2.loop = true;
        }
        else
        {
            source.loop = true;
        }
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
        source.time = 0;
        source2.time = 0;

        source.PlayScheduled (AudioSettings.dspTime);
        source2.PlayScheduled (AudioSettings.dspTime + loopEndTime);
        scheduledEndTime = loopEndTime;

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

        if (source2.time != 0)
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
        if (source2.time != 0) {
            // Because this loops after the intro, add the intro length to the clip time
            return source2.time + source.clip.length;
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
            GUI.TextArea (new Rect (10, 10, Screen.width - 10, 40), source.time.ToString () + ", " + source2.time.ToString () + ", " + scheduledEndTime);
        }
    }
}

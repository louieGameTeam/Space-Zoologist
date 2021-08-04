using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Current implementation is to play the music of the current scene and then self-destruct, probably changed later to coordinate with fade in/out
/// </summary>
///

[RequireComponent(typeof(CustomMusicLoopController))]
public class MusicQueuer : MonoBehaviour
{
    [SerializeField] CustomMusicLoopController nextTrack = default;
    [SerializeField] bool fading;

    // Start is called before the first frame update
    private void Start()
    {
        gameObject.name = nextTrack.Source.clip.name;
        transform.SetParent(AudioManager.instance.MusicManager.transform);
        QueueAndTrigger();
        Destroy(this);
    }


    public void QueueMusic()
    {
        AudioManager.instance?.MusicManager.SetNextTrack(nextTrack);
    }

    public void QueueAndTrigger()
    {
        AudioManager.instance?.MusicManager.StartTransition(fading, nextTrack);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicQueuer : MonoBehaviour
{
    [SerializeField] LoopableAudioTrack nextTrack = default;
    private void Start()
    {
        QueueAndTrigger();
    }

    // Start is called before the first frame update
    public void QueueMusic()
    {
        AudioManager.instance.MusicManager.SetNextTrack(nextTrack);
    }

    public void QueueAndTrigger()
    {
        QueueMusic();
        AudioManager.instance.MusicManager.StartTransition();
    }
}

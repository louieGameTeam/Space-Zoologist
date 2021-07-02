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
        MusicManager.instance.SetNextTrack(nextTrack);
    }

    public void QueueAndTrigger() {
        QueueMusic();
        MusicManager.instance.StartTransition();
    }
}

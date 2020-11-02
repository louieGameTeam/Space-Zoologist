using UnityEngine;

namespace DialogueEditor
{
    /// <summary>
    /// This class holds all of the values for a node which 
    /// need to be serialized. 
    /// </summary>
    [System.Serializable]
    public class NodeEventHolder : MonoBehaviour
    {
        [SerializeField] public int NodeID;
        [SerializeField] public UnityEngine.Events.UnityEvent Event;
        [SerializeField] public TMPro.TMP_FontAsset TMPFont;
        [SerializeField] public Sprite Icon;
        [SerializeField] public AudioClip Audio;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------
// Conversation C# class - User Facing
//--------------------------------------

namespace DialogueEditor
{
    public class Conversation
    {
        public SpeechNode Root;
    }

    public abstract class ConversationNode
    {
        public string Text;
        public TMPro.TMP_FontAsset TMPFont;
    }

    public class SpeechNode : ConversationNode
    {
        public string Name;

        /// <summary>
        /// Should this speech node go onto the next one automatically?
        /// </summary>
        public bool AutomaticallyAdvance;

        /// <summary>
        /// Should this speech node display a "continue" or "end" option?
        /// </summary>
        public bool AutoAdvanceShouldDisplayOption;

        /// <summary>
        /// If AutomaticallyAdvance==True, how long should this speech node 
        /// display before going onto the next one?
        /// </summary>
        public float TimeUntilAdvance;

        public Sprite Icon;
        public AudioClip Audio;
        public float Volume;

        /// <summary>
        /// The Options available on this Speech node, if any.
        /// </summary>
        public List<OptionNode> Options;

        /// <summary>
        /// The Speech node following the current, if any.
        /// </summary>
        public SpeechNode Dialogue;

        public UnityEngine.Events.UnityEvent Event;
    }

    public class OptionNode : ConversationNode
    {
        public SpeechNode Dialogue;
    }
}


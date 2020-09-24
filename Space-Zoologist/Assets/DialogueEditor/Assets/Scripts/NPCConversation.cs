using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace DialogueEditor
{
    //--------------------------------------
    // Conversation Monobehaviour (Serialized)
    //--------------------------------------

    [System.Serializable]
    [DisallowMultipleComponent]
    public class NPCConversation : MonoBehaviour
    {
        /// <summary> Version 1.03 </summary>
        public const int CurrentVersion = 103;

        private readonly string CHILD_NAME = "ConversationEventInfo";

        // Serialized data
        [SerializeField] public int CurrentIDCounter = 1;
        [SerializeField] private string json;
        [SerializeField] private int saveVersion;
        [SerializeField] public string DefaultName;
        [SerializeField] public Sprite DefaultSprite;
        [SerializeField] public TMPro.TMP_FontAsset DefaultFont;
        [FormerlySerializedAs("Events")]
        [SerializeField] private List<NodeEventHolder> NodeSerializedDataList;

        // Runtime vars
        public UnityEngine.Events.UnityEvent Event;

        public int Version { get { return saveVersion; } }


        //--------------------------------------
        // Util
        //--------------------------------------

        public NodeEventHolder GetNodeData(int id)
        {
            // Create list if none
            if (NodeSerializedDataList == null)
                NodeSerializedDataList = new List<NodeEventHolder>();

            // Look through list to find by ID
            for (int i = 0; i < NodeSerializedDataList.Count; i++)
                if (NodeSerializedDataList[i].NodeID == id)
                    return NodeSerializedDataList[i];

            // If none exist, create a new GameObject
            Transform EventInfo = this.transform.Find(CHILD_NAME);
            if (EventInfo == null)
            {
                GameObject obj = new GameObject(CHILD_NAME);
                obj.transform.SetParent(this.transform);
            }
            EventInfo = this.transform.Find(CHILD_NAME);

            // Add a new Component for this node
            NodeEventHolder h = EventInfo.gameObject.AddComponent<NodeEventHolder>();
            h.NodeID = id;
            h.Event = new UnityEngine.Events.UnityEvent();
            NodeSerializedDataList.Add(h);
            return h;
        }

        public void DeleteDataForNode(int id)
        {
            if (NodeSerializedDataList == null)
                return;

            for (int i = 0; i < NodeSerializedDataList.Count; i++)
            {
                if (NodeSerializedDataList[i].NodeID == id)
                {
                    GameObject.DestroyImmediate(NodeSerializedDataList[i]);
                    NodeSerializedDataList.RemoveAt(i);
                }
            }
        }


        //--------------------------------------
        // Serialize and Deserialize
        //--------------------------------------

        public void Serialize(EditableConversation conversation)
        {
            json = Jsonify(conversation);
            saveVersion = CurrentVersion;
        }

        public Conversation Deserialize()
        {
            // Deserialize an editor-version (containing all info) that 
            // we will use to construct the user-facing Conversation data structure. 
            EditableConversation ec = this.DeserializeForEditor();

            // Create a conversation. 
            Conversation conversation = new Conversation();
            // Create a dictionary to store our created nodes by UID
            Dictionary<int, SpeechNode> dialogues = new Dictionary<int, SpeechNode>();
            Dictionary<int, OptionNode> options = new Dictionary<int, OptionNode>();

            // Create a Dialogue and Option node for each in the conversation
            // Put them in the dictionary
            for (int i = 0; i < ec.SpeechNodes.Count; i++)
            {
                SpeechNode node = new SpeechNode();
                node.Name = ec.SpeechNodes[i].Name;
                node.Text = ec.SpeechNodes[i].Text;
                node.AutomaticallyAdvance = ec.SpeechNodes[i].AdvanceDialogueAutomatically;
                node.AutoAdvanceShouldDisplayOption = ec.SpeechNodes[i].AutoAdvanceShouldDisplayOption;
                node.TimeUntilAdvance = ec.SpeechNodes[i].TimeUntilAdvance;
                node.TMPFont = ec.SpeechNodes[i].TMPFont;
                node.Icon = ec.SpeechNodes[i].Icon;
                node.Audio = ec.SpeechNodes[i].Audio;
                node.Volume = ec.SpeechNodes[i].Volume;
                node.Options = new List<OptionNode>();
                if (this.GetNodeData(ec.SpeechNodes[i].ID) != null)
                {
                    node.Event = this.GetNodeData(ec.SpeechNodes[i].ID).Event;
                }

                dialogues.Add(ec.SpeechNodes[i].ID, node);
            }
            for (int i = 0; i < ec.Options.Count; i++)
            {
                OptionNode node = new OptionNode();
                node.Text = ec.Options[i].Text;
                node.TMPFont = ec.Options[i].TMPFont;

                options.Add(ec.Options[i].ID, node);
            }

            // Now that we have every node in the dictionary, reconstruct the tree 
            // And also look for the root
            for (int i = 0; i < ec.SpeechNodes.Count; i++)
            {
                // Connect dialogue to options
                for (int j = 0; j < ec.SpeechNodes[i].OptionUIDs.Count; j++)
                {
                    dialogues[ec.SpeechNodes[i].ID].Options.Add(options[ec.SpeechNodes[i].OptionUIDs[j]]);
                }

                // Connect dialogue to following dialogue
                if (ec.SpeechNodes[i].SpeechUID != EditableConversation.INVALID_UID)
                {
                    dialogues[ec.SpeechNodes[i].ID].Dialogue = dialogues[ec.SpeechNodes[i].SpeechUID];
                }

                // Check if root
                if (ec.SpeechNodes[i].EditorInfo.isRoot)
                {
                    conversation.Root = dialogues[ec.SpeechNodes[i].ID];
                }
            }

            for (int i = 0; i < ec.Options.Count; i++)
            {
                // Connect option to following dialogue
                if (dialogues.ContainsKey(ec.Options[i].SpeechUID))
                {
                    options[ec.Options[i].ID].Dialogue = dialogues[ec.Options[i].SpeechUID];
                }
            }

            return conversation;
        }

        public EditableConversation DeserializeForEditor()
        {
            // Dejsonify 
            EditableConversation conversation = Dejsonify();

            if (conversation != null)
            {
                // Deserialize the indivudual nodes
                {
                    if (conversation.SpeechNodes != null)
                        for (int i = 0; i < conversation.SpeechNodes.Count; i++)
                            conversation.SpeechNodes[i].Deserialize(this);

                    if (conversation.Options != null)
                        for (int i = 0; i < conversation.Options.Count; i++)
                            conversation.Options[i].Deserialize(this);
                }
            }

            // Clear our dummy event
            Event = new UnityEngine.Events.UnityEvent();

            return conversation;
        }

        private string Jsonify(EditableConversation conversation)
        {
            if (conversation == null || conversation.Options == null) { return ""; }

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(EditableConversation));
            ser.WriteObject(ms, conversation);
            byte[] jsonData = ms.ToArray();
            ms.Close();
            string toJson = System.Text.Encoding.UTF8.GetString(jsonData, 0, jsonData.Length);

            return toJson;
        }

        private EditableConversation Dejsonify()
        {
            if (json == null || json == "")
                return null;

            EditableConversation conversation = new EditableConversation();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(conversation.GetType());
            conversation = ser.ReadObject(ms) as EditableConversation;
            ms.Close();

            return conversation;
        }
    }


    //--------------------------------------
    // Editable Conversation C# class - Non-User facing; for use in editor (Deserialized)
    //--------------------------------------

    [DataContract]
    public class EditableConversation
    {
        public const int INVALID_UID = -1;

        public EditableConversation()
        {
            SpeechNodes = new List<EditableSpeechNode>();
            Options = new List<EditableOptionNode>();
        }

        [DataMember]
        public List<EditableSpeechNode> SpeechNodes;

        [DataMember]
        public List<EditableOptionNode> Options;

        // ----

        public EditableSpeechNode GetRootNode()
        {
            for (int i = 0; i < SpeechNodes.Count; i++)
            {
                if (SpeechNodes[i].EditorInfo.isRoot)
                    return SpeechNodes[i];
            }
            return null;
        }

        public EditableConversationNode GetNodeByUID(int uid)
        {
            for (int i = 0; i < SpeechNodes.Count; i++)
                if (SpeechNodes[i].ID == uid)
                    return SpeechNodes[i];

            for (int i = 0; i < Options.Count; i++)
                if (Options[i].ID == uid)
                    return Options[i];

            return null;
        }

        public EditableSpeechNode GetSpeechByUID(int uid)
        {
            for (int i = 0; i < SpeechNodes.Count; i++)
                if (SpeechNodes[i].ID == uid)
                    return SpeechNodes[i];

            return null;
        }

        public EditableOptionNode GetOptionByUID(int uid)
        {
            for (int i = 0; i < Options.Count; i++)
                if (Options[i].ID == uid)
                    return Options[i];

            return null;
        }
    }


    //--------------------------------------
    // Abstract Node class (Editor)

    [DataContract]
    public abstract class EditableConversationNode
    {
        /// <summary> Info used internally by the editor window. </summary>
        [DataContract]
        public class EditorArgs
        {
            [DataMember]
            public float xPos;

            [DataMember]
            public float yPos;

            [DataMember]
            public bool isRoot;
        }

        public EditableConversationNode()
        {
            parents = new List<EditableConversationNode>();
            parentUIDs = new List<int>();
            EditorInfo = new EditorArgs { xPos = 0, yPos = 0, isRoot = false };
        }

        /// <summary> Info used internally by the editor window. </summary>
        [DataMember]
        public EditorArgs EditorInfo;

        [DataMember]
        public int ID;

        [DataMember]
        public string Text;

        /// <summary> TextMeshPro font </summary>
        public TMPro.TMP_FontAsset TMPFont;
        [DataMember]
        public string TMPFontGUID;

        [DataMember]
        public List<int> parentUIDs;
        public List<EditableConversationNode> parents;

        // ------------------------

        public abstract void RemoveSelfFromTree();
        public abstract void RegisterUIDs();

        public virtual void PrepareForSerialization(NPCConversation conversation)
        {
            conversation.GetNodeData(this.ID).TMPFont = this.TMPFont;
        }

        public virtual void Deserialize(NPCConversation conversation)
        {
            this.TMPFont = conversation.GetNodeData(this.ID).TMPFont;

#if UNITY_EDITOR
            // If under V1.03, Load from database via GUID, so data is not lost for people who are upgrading
            if (conversation.Version < 103)
            {
                if (this.TMPFont == null)
                {
                    if (!string.IsNullOrEmpty(TMPFontGUID))
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(TMPFontGUID);
                        this.TMPFont = (TMPro.TMP_FontAsset)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(TMPro.TMP_FontAsset));

                    }
                }
            }
#endif
        }
    }


    //--------------------------------------
    // Speech Node class (Editor)

    [DataContract]
    public class EditableSpeechNode : EditableConversationNode
    {
        public EditableSpeechNode() : base()
        {
            Options = new List<EditableOptionNode>();
            OptionUIDs = new List<int>();
            SpeechUID = EditableConversation.INVALID_UID;
        }

        [DataMember]
        public string Name;

        /// <summary>
        /// The selectable options of this Speech.
        /// </summary>
        public List<EditableOptionNode> Options;
        [DataMember] public List<int> OptionUIDs;

        /// <summary>
        /// The Speech this Speech leads onto (if no options).
        /// </summary>
        public EditableSpeechNode Speech;
        [DataMember] public int SpeechUID;

        /// <summary>
        /// The NPC Icon
        /// </summary>
        public Sprite Icon;
        [DataMember] public string IconGUID;

        /// <summary>
        /// The Audio Clip acompanying this Speech.
        /// </summary>
        public AudioClip Audio;
        [DataMember] public string AudioGUID;

        /// <summary>
        /// The Volume for the AudioClip;
        /// </summary>
        [DataMember] public float Volume;

        /// <summary>
        /// If this dialogue leads onto another dialogue... 
        /// Should the dialogue advance automatially?
        /// </summary>
        [DataMember] public bool AdvanceDialogueAutomatically;

        /// <summary>
        /// If this dialogue automatically advances, should it also display an 
        /// "end" / "continue" button?
        /// </summary>
        [DataMember] public bool AutoAdvanceShouldDisplayOption;

        /// <summary>
        /// The time it will take for the Dialogue to automaically advance
        /// </summary>
        [DataMember] public float TimeUntilAdvance;

        // ------------------------------

        public void AddOption(EditableOptionNode newOption)
        {
            if (Options == null)
                Options = new List<EditableOptionNode>();

            if (Options.Contains(newOption))
                return;

            // Delete the speech I point to, if any
            if (this.Speech != null)
            {
                this.Speech.parents.Remove(this);
            }
            this.Speech = null;

            // Setup option connection
            if (!newOption.parents.Contains(this))
                newOption.parents.Add(this);
            Options.Add(newOption);
        }

        public void SetSpeech(EditableSpeechNode newSpeech)
        {
            // Remove myself as a parent from the speech I was previously pointing to
            if (this.Speech != null)
            {
                this.Speech.parents.Remove(this);
            }

            // Remove any options I may have
            if (Options != null)
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    // I am no longer the parents of these options
                    Options[i].parents.Remove(this);
                }
                Options.Clear();
            }

            this.Speech = newSpeech;
            if (!newSpeech.parents.Contains(this))
                newSpeech.parents.Add(this);
        }

        public override void RemoveSelfFromTree()
        {
            // This speech is no longer the parents resulting speech
            for (int i = 0; i < parents.Count; i++)
            {
                if (parents[i] != null)
                {
                    if (parents[i] is EditableOptionNode)
                    {
                        (parents[i] as EditableOptionNode).Speech = null;
                    }
                    else if (parents[i] is EditableSpeechNode)
                    {
                        (parents[i] as EditableSpeechNode).Speech = null;
                    }
                }
            }

            // This speech is no longer the parent of any children options
            if (Options != null)
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    Options[i].parents.Clear();
                }
            }

            // This speech is no longer the parent of any speech nodes
            if (this.Speech != null)
            {
                this.Speech.parents.Remove(this);
            }
        }

        public override void RegisterUIDs()
        {
            if (parentUIDs != null)
                parentUIDs.Clear();
            parentUIDs = new List<int>();
            for (int i = 0; i < parents.Count; i++)
            {
                parentUIDs.Add(parents[i].ID);
            }

            if (OptionUIDs != null)
                OptionUIDs.Clear();
            OptionUIDs = new List<int>();
            if (Options != null)
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    OptionUIDs.Add(Options[i].ID);
                }
            }

            if (Speech != null)
                SpeechUID = Speech.ID;
            else
                SpeechUID = EditableConversation.INVALID_UID;
        }

        public override void PrepareForSerialization(NPCConversation conversation)
        {
            base.PrepareForSerialization(conversation);

            conversation.GetNodeData(this.ID).Audio = this.Audio;
            conversation.GetNodeData(this.ID).Icon = this.Icon;
        }

        public override void Deserialize(NPCConversation conversation)
        {
            base.Deserialize(conversation);

            this.Audio = conversation.GetNodeData(this.ID).Audio;
            this.Icon = conversation.GetNodeData(this.ID).Icon;

#if UNITY_EDITOR
            // If under V1.03, Load from database via GUID, so data is not lost for people who are upgrading
            if (conversation.Version < 103)
            {            
                if (this.Audio == null)
                {
                    if (!string.IsNullOrEmpty(AudioGUID))
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(AudioGUID);
                        this.Audio = (AudioClip)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip));

                    }
                }

                if (this.Icon == null)
                {
                    if (!string.IsNullOrEmpty(IconGUID))
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(IconGUID);
                        this.Icon = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));

                    }
                }
            }
#endif
        }
    }

    //--------------------------------------
    // Option Node class (Editor)

    [DataContract]
    public class EditableOptionNode : EditableConversationNode
    {
        public EditableOptionNode() : base()
        {
            SpeechUID = EditableConversation.INVALID_UID;
        }

        /// <summary>
        /// The Speech this option leads to.
        /// </summary>        
        public EditableSpeechNode Speech;

        [DataMember]
        public int SpeechUID;

        public void SetSpeech(EditableSpeechNode newSpeech)
        {
            // Remove myself as a parent from the speech I was previously pointing to
            if (this.Speech != null)
            {
                this.Speech.parents.Remove(this);
            }

            this.Speech = newSpeech;
            if (!newSpeech.parents.Contains(this))
                newSpeech.parents.Add(this);
        }

        public override void RemoveSelfFromTree()
        {
            // This option is no longer part of any parents speechs possible options
            for (int i = 0; i < parents.Count; i++)
            {
                (parents[i] as EditableSpeechNode).Options.Remove(this);
            }

            // This option is no longer the parent to its child speech
            if (Speech != null)
            {
                Speech.parents.Remove(this);
            }
        }

        public override void RegisterUIDs()
        {
            if (parentUIDs != null)
                parentUIDs.Clear();
            parentUIDs = new List<int>();
            for (int i = 0; i < parents.Count; i++)
            {
                parentUIDs.Add(parents[i].ID);
            }

            SpeechUID = EditableConversation.INVALID_UID;
            if (Speech != null)
                SpeechUID = Speech.ID;
        }
    }
}
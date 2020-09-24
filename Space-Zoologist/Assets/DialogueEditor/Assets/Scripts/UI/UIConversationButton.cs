using UnityEngine;
using UnityEngine.UI;

namespace DialogueEditor
{
    public class UIConversationButton : MonoBehaviour
    {
        public enum eHoverState
        {
            idleOff,
            animatingOn,
            idleOn,
            animatingOff,
        }

        public TMPro.TextMeshProUGUI TextMesh;
        public Image OptionBackgroundImage;

        public OptionNode Option { get { return m_option; } }
        public SpeechNode Speech { get { return m_action; } }

        private OptionNode m_option;
        private SpeechNode m_action;
        private RectTransform m_rect;

        // Hovering 
        private float m_hoverT = 0.0f;
        private eHoverState m_hoverState;
        private bool Hovering { get { return (m_hoverState == eHoverState.animatingOn || m_hoverState == eHoverState.animatingOff); } }
        private Vector3 BigSize { get { return Vector3.one * 1.2f; } }


        //--------------------------------------
        // MonoBehaviour
        //--------------------------------------

        private void Awake()
        {
            m_rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (Hovering)
            {
                m_hoverT += Time.deltaTime;
                float normalised = m_hoverT / 0.2f;
                bool done = false;
                if (normalised >= 1)
                {
                    normalised = 1;
                    done = true;
                }
                Vector3 size = Vector3.one;
                float ease = EaseOutQuart(normalised);
                

                switch (m_hoverState)
                {
                    case eHoverState.animatingOn:
                        size = Vector3.Lerp(Vector3.one, BigSize, ease);
                        break;
                    case eHoverState.animatingOff:
                        size = Vector3.Lerp(BigSize, Vector3.one, ease);
                        break;
                }

                m_rect.localScale = size;

                if (done)
                {
                    m_hoverState = (m_hoverState == eHoverState.animatingOn) ? eHoverState.idleOn : eHoverState.idleOff;
                }
            }
        }




        //--------------------------------------
        // Input Events
        //--------------------------------------

        public void OnHover(bool hovering)
        {
            if (!ConversationManager.Instance.AllowMouseInteraction) { return; }

            if (hovering)
            {
                ConversationManager.Instance.AlertHover(this);
            }
            else
            {
                ConversationManager.Instance.AlertHover(null);
            }
        }

        public void OnClick()
        {
            if (!ConversationManager.Instance.AllowMouseInteraction) { return; }

            if (m_action != null)
                ConversationManager.Instance.DoSpeech(m_action);
            else
                ConversationManager.Instance.OptionSelected(m_option);
        }




        //--------------------------------------
        // Public calls
        //--------------------------------------

        public void SetHovering(bool selected)
        {
            if (selected && (m_hoverState == eHoverState.animatingOn || m_hoverState == eHoverState.idleOn)) { return; }
            if (!selected && (m_hoverState == eHoverState.animatingOff || m_hoverState == eHoverState.idleOff)) { return; }

            if (selected)
                m_hoverState = eHoverState.animatingOn;
            else
                m_hoverState = eHoverState.animatingOff;
            m_hoverT = 0f;
        }

        public void SetImage(Sprite sprite, bool sliced)
        {
            if (sprite != null)
            {
                OptionBackgroundImage.sprite = sprite;

                if (sliced)
                    OptionBackgroundImage.type = Image.Type.Sliced;
                else
                    OptionBackgroundImage.type = Image.Type.Simple;
            }
        }

        public void InitButton(OptionNode option)
        {
            // Set font
            if (option.TMPFont != null)
            {
                TextMesh.font = option.TMPFont;
            }
            else
            {
                TextMesh.font = null;
            }
        }

        public void SetAlpha(float a)
        {
            Color c_image = OptionBackgroundImage.color;
            Color c_text = TextMesh.color;
            c_image.a = a;
            c_text.a = a;
            OptionBackgroundImage.color = c_image;
            TextMesh.color = c_text;
        }

        public void SetOption(OptionNode option)
        {
            m_option = option;
            TextMesh.text = option.Text;
        }

        public void SetFollowingAction(SpeechNode action)
        {
            m_action = action;
            TextMesh.text = "Continue.";
        }

        public void SetAsEndConversation()
        {
            m_option = null;
            TextMesh.text = "End.";
        }




        //--------------------------------------
        // Util
        //--------------------------------------

        private static float EaseOutQuart(float normalized)
        {
            return (1 - Mathf.Pow(1 - normalized, 4));
        }
    }
}
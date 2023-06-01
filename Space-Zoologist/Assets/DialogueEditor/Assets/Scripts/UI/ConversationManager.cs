using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace DialogueEditor
{
    public class ConversationManager : MonoBehaviour
    {
        private const float TRANSITION_TIME = 0.05f; // Transition time for fades

        public static ConversationManager Instance { get; private set; }

        public delegate void ConversationStartEvent();
        public delegate void ConversationEndEvent();

        public static ConversationStartEvent OnConversationStarted;
        public static ConversationEndEvent OnConversationEnded;

        private enum eState
        {
            TransitioningDialogueBoxOn,
            ScrollingText,
            TransitioningOptionsOn,
            Idle,
            TransitioningOptionsOff,
            TransitioningDialogueOff,
            Off,
            NONE,
            freeze
        }

        // User-Facing options
        // Drawn by custom inspector
        public bool ScrollText;
        public float ScrollSpeed = 1;
        public Sprite BackgroundImage;
        public bool BackgroundImageSliced;
        public Sprite OptionImage;
        public bool OptionImageSliced;
        public bool AllowMouseInteraction;
        public bool UseAdvanceInput;
        public string AdvanceInput = "Submit";
        public RectTransform Background;
        public GameObject SkipConversationButton;
        [Tooltip("UI For indicating when dialogue can continue")]
        public RectTransform dialogueContinueIndicator;
        public Button ProgressConversationButton;
        // Non-User facing 
        // Not exposed via custom inspector
        //
        // Base panels
        public RectTransform DialoguePanel;
        public RectTransform OptionsPanel;
        // Dialogue UI
        public Image DialogueBackground;
        public Image NpcIcon;
        public TMPro.TextMeshProUGUI NameText;
        public TMPro.TextMeshProUGUI DialogueText;
        // Components
        public AudioSource AudioPlayer;
        // Prefabs
        public UIConversationButton ButtonPrefab;
        // Default values
        public Sprite BlankSprite;
        //Used for freezing
        private eState preFreezeState = eState.NONE;

        // Getter properties
        public bool IsConversationActive
        {
            get
            {
                return m_state != eState.NONE && m_state != eState.Off;
            }
        }

        // Private
        private float m_elapsedScrollTime;
        private int m_scrollIndex;
        public int m_targetScrollTextCount;
        private eState m_state;
        private float m_stateTime;
        private Conversation m_conversation;
        private List<UIConversationButton> m_uiOptions;
        private bool skipping;
        private bool isFrozen = false;
        private bool progressUIButtonDown = false;

        private Tween dialogueIndicatorTween;

        private SpeechNode m_pendingDialogue;
        private OptionNode m_selectedOption;
        private SpeechNode m_currentSpeech;

        // Selection options
        private int m_currentSelectedIndex = -1;

        //public GameObject BacklogGameObject;
        //public Button BacklogButton;
        //public TMPro.TextMeshProUGUI Backlog;
        //public ScrollRect BacklogScrollRect;
        //--------------------------------------
        // Awake, Start, Destroy
        //--------------------------------------

        public void Initialize()
        {
            // Destroy myself if I am not the singleton
            if (Instance != null && Instance != this)
            {
                GameObject.Destroy(this.gameObject);
            }
            Instance = this;

            NpcIcon.sprite = BlankSprite;
            DialogueText.text = "";
            //Backlog = BacklogGameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            //BacklogButton = BacklogGameObject.GetComponentInChildren<Button>(true);
            //BacklogScrollRect = BacklogGameObject.GetComponentInChildren<ScrollRect>(true);
            TurnOffUI();

            m_uiOptions = new List<UIConversationButton>();

            // Set the state when progress button is clicked (the state is cleared at the end of update)
            // The state probably shouldn't be update driven
            ProgressConversationButton.onClick.AddListener(OnProgressButtonPressed);
        }

        private void OnDestroy()
        {
            Instance = null;
            DOTween.Kill(dialogueIndicatorTween);
        }

        //--------------------------------------
        // Update
        //--------------------------------------

        private void Update()
        {
            // Use method to compute if we should progress or not
            bool progressInput = Progress();

            if (m_state != eState.Off) { 
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    skipping = !skipping;
                }
            }

            //if (Input.mouseScrollDelta.y > 0.2f && !BacklogGameObject.activeSelf) {
            //    ToggleBacklog();
            //} else if (Input.mouseScrollDelta.y < -0.2f && BacklogGameObject.activeSelf) {
            //    ToggleBacklog();
            //}

            switch (m_state)
            {
                case eState.TransitioningDialogueBoxOn:
                    {
                        m_stateTime += Time.deltaTime;
                        float t = m_stateTime / TRANSITION_TIME;

                        if (t > 1)
                        {
                            DoSpeech(m_pendingDialogue);
                            return;
                        }

                        SetColorAlpha(DialogueBackground, t);
                        SetColorAlpha(NpcIcon, t);
                        SetColorAlpha(NameText, t);
                    }
                    break;

                case eState.ScrollingText:
                    ProgressConversationButton.interactable = true;
                    UpdateScrollingText();
                    break;

                case eState.TransitioningOptionsOn:
                    {
                        m_stateTime += Time.deltaTime;
                        float t = m_stateTime / TRANSITION_TIME;

                        if (t > 1)
                        {
                            SetState(eState.Idle);

                            return;
                        }

                        for (int i = 0; i < m_uiOptions.Count; i++)
                            m_uiOptions[i].SetAlpha(t);
                    }
                    break;

                case eState.Idle:
                    {
                        m_stateTime += Time.deltaTime;
                        if (m_currentSpeech.Dialogue != null || m_currentSpeech.Options == null || m_currentSpeech.Options.Count == 0)
                        {
                            if (m_stateTime > m_currentSpeech.TimeUntilAdvance)
                            {
                                ProgressConversationButton.interactable = true;
                                if ((progressInput || skipping || m_currentSpeech.AutomaticallyAdvance) && !isFrozen)
                                {
                                    SetState(eState.TransitioningOptionsOff);
                                    ProgressConversationButton.interactable = false;
                                }
                            }
                        }
                        else
                        {
                            ProgressConversationButton.interactable = false;
                        }
                    }
                    break;

                case eState.TransitioningOptionsOff:
                    {
                        m_stateTime += Time.deltaTime;
                        float t = m_stateTime / TRANSITION_TIME;

                        if (t > 1)
                        {
                            ClearOptions();
                            m_currentSpeech.Event?.Invoke();

                            //if (m_currentSpeech.AutomaticallyAdvance)
                            //{
                                if (m_currentSpeech.Dialogue != null)
                                {
                                    DoSpeech(m_currentSpeech.Dialogue);
                                    return;
                                }
                                else if (m_currentSpeech.Options == null || m_currentSpeech.Options.Count == 0)
                                {
                                    EndConversation();
                                    return;
                                }  
                            //}

                            if (m_selectedOption == null)
                            {
                                EndConversation();
                                return;
                            }

                            SpeechNode nextAction = m_selectedOption.Dialogue;
                            if (nextAction == null)
                            {
                                EndConversation();
                            }
                            else
                            {
                                DoSpeech(nextAction);
                            }
                            return;
                        }


                        for (int i = 0; i < m_uiOptions.Count; i++)
                            m_uiOptions[i].SetAlpha(1 - t);

                        SetColorAlpha(DialogueText, 1 - t);
                    }
                    break;

                case eState.TransitioningDialogueOff:
                    {
                        m_stateTime += Time.deltaTime;
                        float t = m_stateTime / TRANSITION_TIME;

                        if (t > 1)
                        {
                            TurnOffUI();
                            return;
                        }

                        SetColorAlpha(DialogueBackground, 1 -t);
                        SetColorAlpha(NpcIcon, 1 - t);
                        SetColorAlpha(NameText, 1 - t);
                    }
                    break;
                case eState.freeze:
                    break;
            }

            // Because progression is polled in update instead of an event trigger, reset the state of the button down
            progressUIButtonDown = false;
        }

        private void UpdateScrollingText()
        {
            bool progressInput = Progress();
            const float charactersPerSecond = 1500;
            float timePerChar = (60.0f / charactersPerSecond);
            timePerChar *= ScrollSpeed;

            m_elapsedScrollTime += Time.deltaTime;

            if (progressInput || skipping)
            {
                m_elapsedScrollTime = 0f;
                DialogueText.maxVisibleCharacters = m_targetScrollTextCount;
                SetState(eState.TransitioningOptionsOn);
            }
            

            if (m_elapsedScrollTime > timePerChar)
            {
                m_elapsedScrollTime = 0f;

                DialogueText.maxVisibleCharacters = m_scrollIndex;
                m_scrollIndex++;

                // Finished?
                if (m_scrollIndex >= m_targetScrollTextCount)
                {
                    SetState(eState.TransitioningOptionsOn);
                }
            }
        }


        //IEnumerator<int> ExpandDialogue() {
        //    while (Vector2.Distance(Background.sizeDelta, new Vector2(1400, Background.sizeDelta.y)) > 10){
        //        Background.sizeDelta = Vector2.MoveTowards(Background.sizeDelta, new Vector2(1400, Background.sizeDelta.y), 20);
        //        yield return 0;
        //    }
        //}

        //IEnumerator<int> ShrinkDialogue()
        //{
        //    while (Vector2.Distance(Background.sizeDelta, new Vector2(1100, Background.sizeDelta.y)) > 10)
        //    {
        //        Background.sizeDelta = Vector2.MoveTowards(Background.sizeDelta, new Vector2(1100, Background.sizeDelta.y), 20);
        //        yield return 0;
        //    }
        //}

        //--------------------------------------
        // Set state
        //--------------------------------------

        private void SetState(eState newState)
        {
            // Exit
            switch (m_state)
            {
                case eState.TransitioningOptionsOn:
                    // Make sure all options are completely visible
                    foreach (UIConversationButton option in m_uiOptions)
                        option.SetAlpha(1);
                    break;
                case eState.TransitioningOptionsOff:
                    m_selectedOption = null;
                    break;
                case eState.TransitioningDialogueBoxOn:
                    SetColorAlpha(DialogueBackground, 1);
                    SetColorAlpha(NpcIcon, 1);
                    SetColorAlpha(NameText, 1);
                    break;
                case eState.Idle:
                    SetConversationContinueIndicator(false);
                    break;
            }

            m_state = newState;
            m_stateTime = 0f;

            // Enter 
            switch (m_state)
            {
                case eState.TransitioningDialogueBoxOn:
                    {
                        SetColorAlpha(DialogueBackground, 0);
                        SetColorAlpha(NpcIcon, 0);
                        SetColorAlpha(NameText, 0);

                        DialogueText.text = "";
                        NameText.text = m_pendingDialogue.Name;
                        NpcIcon.sprite = m_pendingDialogue.Icon != null ? m_pendingDialogue.Icon : BlankSprite;
                    }
                    break;

                case eState.ScrollingText:
                    {
                        SetColorAlpha(DialogueText, 1);

                        if (m_targetScrollTextCount == 0)
                        {
                            SetState(eState.TransitioningOptionsOn);
                            return;
                        }
                    }
                    break;

                case eState.TransitioningOptionsOn:
                    {
                        for (int i = 0; i < m_uiOptions.Count; i++)
                        {
                            m_uiOptions[i].gameObject.SetActive(true);
                        }
                    }
                    break;
                case eState.Idle:
                    if(m_currentSpeech.Options.Count == 0 && !isFrozen)
                        SetConversationContinueIndicator(true);
                    break;
            }     
        }

        //--------------------------------------
        // Start / End Conversation
        //--------------------------------------

        public void StartConversation(NPCConversation conversation)
        {
            m_conversation = conversation.Deserialize();
            if (OnConversationStarted != null)
                OnConversationStarted.Invoke();

            TurnOnUI();
            ClearOptions();
            m_pendingDialogue = m_conversation.Root;
            SetState(eState.TransitioningDialogueBoxOn);
        }

        public void StartConversationWithoutDeserialization(NPCConversation conversation) {

            m_conversation = conversation.RuntimeLoad();
            if (OnConversationStarted != null)
                OnConversationStarted.Invoke();

            TurnOnUI();
            ClearOptions();
            m_pendingDialogue = m_conversation.Root;
            SetState(eState.TransitioningDialogueBoxOn);
        }

        public void EndConversation()
        {
            SetState(eState.TransitioningDialogueOff);
            SetSkipConversationButton(false);
            if (OnConversationEnded != null)
                OnConversationEnded.Invoke();
            //Clear out any highlights should conversation be ended
            FindObjectOfType<TutorialHighlightingScheduler>()?.ClearHighlights();
        }

        //--------------------------------------
        // (CUSTOM) Pause/Continue Conversation
        //--------------------------------------

        public void PauseConversation()
        {
            m_pendingDialogue = m_currentSpeech.Dialogue;
            SetState(eState.TransitioningDialogueOff);
        }

        public void ContinueConversation()
        {
            TurnOnUI();
            SetState(eState.TransitioningDialogueBoxOn);
        }

        public void TurnOffPortraitAndPauseConversation()
        {
            m_pendingDialogue = m_currentSpeech.Dialogue;
            NpcIcon.gameObject.SetActive(false);
            SetState(eState.freeze);
        }

        public void FreezeConversation()
        {
            isFrozen = true;
            preFreezeState = m_state;
            SetState(eState.Idle);
        }

        public void UnfreezeConversation()
        {
            isFrozen = false;
            SetState(preFreezeState);
        }
        //--------------------------------------
        // Public functions
        //--------------------------------------

        public void SelectNextOption()
        {
            int next = m_currentSelectedIndex + 1;
            if (next > m_uiOptions.Count - 1)
            {
                next = 0;
            }
            SetSelectedOption(next);
        }

        public void SelectPreviousOption()
        {
            int previous = m_currentSelectedIndex - 1;
            if (previous < 0)
            {
                previous = m_uiOptions.Count - 1;
            }
            SetSelectedOption(previous);
        }

        public void PressSelectedOption()
        {
            if (m_state != eState.Idle) { return; }
            if (m_currentSelectedIndex < 0) { return; }
            if (m_currentSelectedIndex >= m_uiOptions.Count) { return; }
            if (m_uiOptions.Count == 0) { return; }

            UIConversationButton button = m_uiOptions[m_currentSelectedIndex];

            if (button.Speech != null)
                ConversationManager.Instance.DoSpeech(button.Speech);
            else
                ConversationManager.Instance.OptionSelected(button.Option);
        }

        public void AlertHover(UIConversationButton button)
        {
            for (int i = 0; i < m_uiOptions.Count; i++)
            {
                if (m_uiOptions[i] == button && m_currentSelectedIndex != i)
                {
                    SetSelectedOption(i);
                    return;
                }
            }

            if (button == null)
                UnselectOption();
        }




        //--------------------------------------
        // Do Speech
        //--------------------------------------

        public void DoSpeech(SpeechNode speech)
        {
            if (speech == null)
            {
                EndConversation();
                return;
            }

            m_currentSpeech = speech;

            // Clear current options
            ClearOptions();
            m_currentSelectedIndex = -1;

            // Set sprite
            if (speech.Icon == null)
            {
                NpcIcon.sprite = BlankSprite;
            }
            else
            {
                NpcIcon.sprite = speech.Icon;
            }

            // Set font
            if (speech.TMPFont != null)
            {
                DialogueText.font = speech.TMPFont;
            }
            else
            {
                DialogueText.font = null;
            }

            // Set name
            NameText.text = speech.Name;

            // Set text
            if (string.IsNullOrEmpty(speech.Text))
            {
                if (ScrollText)
                {
                    DialogueText.text = "";
                    m_targetScrollTextCount = 0;
                    DialogueText.maxVisibleCharacters = 0;
                    m_elapsedScrollTime = 0f;
                    m_scrollIndex = 0;
                }
                else
                {
                    DialogueText.text = "";
                    DialogueText.maxVisibleCharacters = 1;
                }
            }
            else
            {
                if (ScrollText)
                {
                    DialogueText.text = speech.Text;
                    m_targetScrollTextCount = speech.Text.Length + 1;
                    DialogueText.maxVisibleCharacters = 0;
                    m_elapsedScrollTime = 0f;
                    m_scrollIndex = 0;
                }
                else
                {
                    DialogueText.text = speech.Text;
                    DialogueText.maxVisibleCharacters = speech.Text.Length;
                }
                //if (Backlog != null)
                //{
                //    Backlog.text += speech.Name + ":\n" + speech.Text + "\n\n";
                //}
            }


            // Call the event
            //speech.Event?.Invoke();

            // Play the audio
            if (speech.Audio != null)
            {
                AudioPlayer.clip = speech.Audio;
                AudioPlayer.volume = speech.Volume;
                AudioPlayer.Play();
            }

            // Display new options
            if (speech.Options.Count > 0)
            {
                //StartCoroutine(ShrinkDialogue());
                
                // Decrease spacing when many options are present to prevent options being too small
                var layout = OptionsPanel.GetComponent<VerticalLayoutGroup>();
                if (speech.Options.Count > 3)
                {
                    layout.spacing = 10;
                }
                else {
                    layout.spacing = 20;
                }

                for (int i = 0; i < speech.Options.Count; i++)
                {
                    UIConversationButton option = GameObject.Instantiate(ButtonPrefab, OptionsPanel);
                    option.InitButton(speech.Options[i]);
                    option.SetOption(speech.Options[i]);
                    m_uiOptions.Add(option);
                }
            }
            else
            {
                //StartCoroutine(ExpandDialogue());
                // Display "Continue" / "End" if we should.
                //bool notAutoAdvance = !speech.AutomaticallyAdvance;
                bool autoWithOption = (speech.AutomaticallyAdvance && speech.AutoAdvanceShouldDisplayOption);
                if (autoWithOption)
                {
                    // Else display "continue" button to go to following dialogue
                    if (speech.Dialogue != null)
                    {
                        UIConversationButton option = GameObject.Instantiate(ButtonPrefab, OptionsPanel);
                        option.SetFollowingAction(speech.Dialogue);
                        m_uiOptions.Add(option);
                    }
                    // Else display "end" button
                    else
                    {
                        UIConversationButton option = GameObject.Instantiate(ButtonPrefab, OptionsPanel);
                        option.SetAsEndConversation();
                        m_uiOptions.Add(option);
                    }
                }
            }
            // Auto select first option
            // SetSelectedOption(0);

            // Set the button sprite and alpha
            for (int i = 0; i < m_uiOptions.Count; i++)
            {
                m_uiOptions[i].SetImage(OptionImage, OptionImageSliced);
                m_uiOptions[i].SetAlpha(0);
                m_uiOptions[i].gameObject.SetActive(false);
            }

            // update UI
            if (!speech.enableNotebookUI && GameManager.Instance.NotebookUI.IsOpen)
                GameManager.Instance.NotebookUI.Toggle();
            // magic string, should refactor if this becomes an issue later
            GameManager.Instance.m_menuManager.ToggleUISingleButton(speech.enableNotebookUI, "notebook");
            AudioManager.instance.PlayOneShotRandom(SFXType.NeutralExplain);
            SetState(eState.ScrollingText);
        }
        
        //--------------------------------------
        // Button callbacks
        //--------------------------------------

        private void OnProgressButtonPressed()
        {
            progressUIButtonDown = true;
        }
        
        //--------------------------------------
        // Option Selected
        //--------------------------------------

        public void OptionSelected(OptionNode option)
        {
            m_selectedOption = option;
            SetState(eState.TransitioningOptionsOff);
        }

        //--------------------------------------
        // Util
        //--------------------------------------

        private void TurnOnUI()
        {
            DialoguePanel.gameObject.SetActive(true);
            OptionsPanel.gameObject.SetActive(true);
            NpcIcon.gameObject.SetActive(true);

            if (BackgroundImage != null)
            {
                DialogueBackground.sprite = BackgroundImage;

                if (BackgroundImageSliced)
                    DialogueBackground.type = Image.Type.Sliced;
                else
                    DialogueBackground.type = Image.Type.Simple;
            }

            NpcIcon.sprite = BlankSprite;
            skipping = false;
        }

        private void TurnOffUI()
        {
            DialoguePanel.gameObject.SetActive(false);
            OptionsPanel.gameObject.SetActive(false);
            NpcIcon.gameObject.SetActive(false);
            SetState(eState.Off);
            gameObject.SetActive(false);
            SetConversationContinueIndicator(false);
#if UNITY_EDITOR
            // Debug.Log("[ConversationManager]: Conversation UI off.");
#endif
        }

        private void ClearOptions()
        {
            while (m_uiOptions.Count != 0)
            {
                GameObject.Destroy(m_uiOptions[0].gameObject);
                m_uiOptions.RemoveAt(0);
            }
        }

        private void SetColorAlpha(MaskableGraphic graphic, float a)
        {
            Color col = graphic.color;
            col.a = a;
            graphic.color = col;
        }

        private void SetSelectedOption(int index)
        {
            if (m_uiOptions.Count == 0) { return; }

            if (index < 0)
                index = 0;
            if (index > m_uiOptions.Count - 1)
                index = m_uiOptions.Count - 1;

            if (m_currentSelectedIndex >= 0)
                m_uiOptions[m_currentSelectedIndex].SetHovering(false);
            m_currentSelectedIndex = index;
            m_uiOptions[index].SetHovering(true);
        }

        private void UnselectOption()
        {
            if (m_currentSelectedIndex < 0) { return; }

            m_uiOptions[m_currentSelectedIndex].SetHovering(false);
            m_currentSelectedIndex = -1;
        }

        public void ToggleBacklog()
        {
            //bool active = BacklogGameObject.activeSelf;
            //BacklogGameObject.SetActive(!active); // toggle backlog
            //BacklogScrollRect.normalizedPosition = new Vector2(0.5f, 0); // scroll to buttom
        }
        Button pingTarget;
        public void AskForOneTimePing(Button target) {
            FreezeConversation();
            pingTarget = target;
            target.onClick.AddListener(OneTimeUnpause);
        }
        public void OneTimeUnpause() {
            UnfreezeConversation();
            pingTarget.onClick.RemoveListener(OneTimeUnpause);
        }

        public void SetSkipConversationButton(bool val)
        {
            if(SkipConversationButton.activeSelf != val)
                SkipConversationButton.SetActive(val);
        }
        
        private void SetConversationContinueIndicator(bool isShown)
        {
            dialogueContinueIndicator.gameObject.SetActive(isShown);
            if (isShown)
            {
                float bounceDist = 15f;
                Vector2 pos = dialogueContinueIndicator.anchoredPosition;
                if (dialogueIndicatorTween == null)
                {
                    dialogueIndicatorTween = DOTween.To(
                        a =>
                        {
                            if (dialogueContinueIndicator)
                                dialogueContinueIndicator.anchoredPosition = new Vector2(pos.x, a);
                        },
                        pos.y,
                        pos.y - bounceDist,
                        0.4f
                    ).SetLoops(-1, LoopType.Yoyo);
                }

                DOTween.Play(dialogueIndicatorTween);
            }
            else
            {
                DOTween.Pause(dialogueIndicatorTween);
            }
        }

        /*
         * CUSTOM METHODS
         */
        private bool Progress()
        {
            // Advance by click only if the progress UI button was pressed down and the ui is not blocking the dialogue
            bool buttonClickAdvanceInput = progressUIButtonDown && UIBlockerSettings.OperationIsAvailable("Dialogue");

            // Only use the advance input if some typing UI object is not currently selected
            bool keyAdvanceInput = UseAdvanceInput && Input.GetButtonDown(AdvanceInput);

            // Progress if the advance button was clicked or the button in the input axes was just pressed
            return buttonClickAdvanceInput || keyAdvanceInput;
        }
    }
}
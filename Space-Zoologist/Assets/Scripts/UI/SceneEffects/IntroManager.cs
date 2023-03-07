using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [SerializeField] float speed = default;

    [SerializeField] TMPro.TextMeshProUGUI Introduction = default;
    [SerializeField] List<string> IntroductionTexts = default;
    [SerializeField] List<Sprite> PrologueIllustrations = default;
    [SerializeField] Image BackgroundImage = default;
    [SerializeField] Image PreviousBackgroundImage = default;
    [SerializeField] Image TextBoxImage = default;
    [SerializeField] RectTransform dialogueContinueIndicator;
    [SerializeField] MusicManager musicManager = default;
    private int Index = 0;
    private bool hasLoadedMenu = false;
    
    private bool isInTransition = false;
    private float transitionTimer = 0f;

    private Tween dialogueIndicatorTween;

    private void Start()
    {
        var color = Introduction.color;
        color.a = 255;
        Introduction.color = color;
        SetupNextScene();
        SetConversationContinueIndicator(false);
    }

    private void OnDestroy()
    {
        
    }

    public void Update()
    {
        if (!isInTransition && Input.GetMouseButtonDown(0) && Index <= IntroductionTexts.Count)
        {
            SetupNextScene();
        }
        FadeNextSceneIn(Introduction);
    }

    private void FadeNextSceneIn(TMPro.TextMeshProUGUI line)
    {
        if (transitionTimer > 0f)
        {
            transitionTimer -= Time.deltaTime * speed;

            float t = 1f - transitionTimer;
            
            if (Index == IntroductionTexts.Count + 1) {
                var c = TextBoxImage.color;
                c.a = 1 - t;
                TextBoxImage.color = c;
                c = Introduction.color;
                c.a = 1 - t;
                Introduction.color = c;
            }
            else
            {
                if (transitionTimer <= 0f)
                {
                    isInTransition = false;
                    SetConversationContinueIndicator(true);
                }
            }
            // Line transition
            var color = line.color;
            color.a = t;
            line.color = color;
        
            // Background transition
            color = BackgroundImage.color;
            color.a = t;
            BackgroundImage.color = color;
            color = PreviousBackgroundImage.color;
            color.a = 1 - t;
            PreviousBackgroundImage.color = color;
        }
        else if (Index == IntroductionTexts.Count + 1 && !hasLoadedMenu)
        {
            hasLoadedMenu = true;
            DOTween.Kill(dialogueIndicatorTween);
            SceneNavigator.LoadScene ("MainMenu"/*, LoadSceneMode.Additive*/);/*
            SceneManager.SetActiveScene (SceneManager.GetSceneByName ("MainMenu"));
            SceneManager.UnloadSceneAsync ("Introduction");*/
        }

        
    }

    private void SetupNextScene()
    {
        SetConversationContinueIndicator(false);
        transitionTimer = 1f;
        isInTransition = true;
        
        if (Index > IntroductionTexts.Count) return;
        
        var color = Introduction.color;
        color.a = 0;
        Introduction.color = color;
        PreviousBackgroundImage.sprite = BackgroundImage.sprite;

        // Set next image - reuse a black copy of the previous image as the transition fader
        if (Index == IntroductionTexts.Count)
        {
            float delay = musicManager.StartTransition(true);
            print ("Starting transition");
            int numIntroBeats = 3;
            speed = 1.0f / (delay + MusicManager.SECONDS_PER_BAR / MusicManager.BEATS_PER_BAR * numIntroBeats);
            color = Color.black;
            Introduction.alignment = TMPro.TextAlignmentOptions.Center;
            Introduction.text = "";
        }
        else
        {
            Introduction.text = IntroductionTexts[Index];
            BackgroundImage.sprite = PrologueIllustrations[Index];
            color = BackgroundImage.color;
        }
        
        color.a = 0;
        BackgroundImage.color = color;
        Index++;
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
                        if(dialogueContinueIndicator)
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
}

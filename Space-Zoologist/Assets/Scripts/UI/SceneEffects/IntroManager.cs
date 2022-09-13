using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] MusicManager musicManager = default;
    private int Index = 0;
    private bool hasLoadedMenu = false;

    private void Start()
    {
        var color = Introduction.color;
        color.a = 255;
        Introduction.color = color;
        SetupNextScene();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && Index <= IntroductionTexts.Count)
        {
            SetupNextScene();
        }
        FadeNextSceneIn(Introduction);
    }

    private void FadeNextSceneIn(TMPro.TextMeshProUGUI line)
    {
        if (line.color.a <= 1)
        {
            var color = line.color;
            color.a += Time.deltaTime * speed;
            line.color = color;
        }
        if (BackgroundImage.color.a <= 1)
        {
            var color = BackgroundImage.color;
            color.a += Time.deltaTime * speed;
            BackgroundImage.color = color;
            color = PreviousBackgroundImage.color;
            color.a += Time.deltaTime * speed;
            PreviousBackgroundImage.color = color;
        }
        else if (Index == IntroductionTexts.Count + 1 && !hasLoadedMenu)
        {
            hasLoadedMenu = true;
            SceneNavigator.LoadScene ("MainMenu"/*, LoadSceneMode.Additive*/);/*
            SceneManager.SetActiveScene (SceneManager.GetSceneByName ("MainMenu"));
            SceneManager.UnloadSceneAsync ("Introduction");*/
        }

        if (Index == IntroductionTexts.Count + 1) {
            var color = TextBoxImage.color;
            color.a -= Time.deltaTime * speed;
            TextBoxImage.color = color;
            color = Introduction.color;
            color.a -= Time.deltaTime * speed;
            Introduction.color = color;
        }
    }

    private void SetupNextScene()
    {
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
}

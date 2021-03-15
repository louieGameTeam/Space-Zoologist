using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEffects : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] float speed = default;

    [SerializeField] TMPro.TextMeshProUGUI Introduction = default;
    [SerializeField] List<string> IntroductionTexts = default;
    [SerializeField] SceneNavigator SceneNavigator = default;
    private int Index = 0;


    private void Start()
    {
        var color = Introduction.color;
        color.a = 255;
        Introduction.color = color;
        SetNextText();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && Index < IntroductionTexts.Count)
        {
            SetNextText();
            if (Index == IntroductionTexts.Count)
            {
                Introduction.alignment = TMPro.TextAlignmentOptions.TopLeft;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            SceneNavigator.LoadMainMenu();
        }
        FadeTextIn(Introduction);
    }

    private void FadeTextIn(TMPro.TextMeshProUGUI line)
    {
        if (line.color.a <= 255)
        {
            var color = line.color;
            color.a += Time.deltaTime * speed;
            line.color = color;
        }
    }

    private void SetNextText()
    {
        var color = Introduction.color;
        color.a = 0;
        Introduction.color = color;
        Introduction.text = IntroductionTexts[Index];
        Index++;
    }
}

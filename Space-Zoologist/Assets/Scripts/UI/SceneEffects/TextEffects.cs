using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEffects : MonoBehaviour
{
    [SerializeField] Scrollbar scrollbar = default;
    [Range(0, 1)]
    [SerializeField] float speed = default;
    [SerializeField] float FadeOutTime = 4f;

    [SerializeField] TMPro.TextMeshProUGUI Introduction = default;
    [SerializeField] SceneNavigator SceneNavigator = default;

    private float TimePassed = 0f;
    private Color originalColor = default;

    private void Start()
    {
        scrollbar.value = 1;
        var color = Introduction.color;
        color.a = 255;
        Introduction.color = color;
        originalColor = Introduction.color;
    }

    public void Update()
    {
        if (scrollbar.value >= 0)
        {
            scrollbar.value -= Time.deltaTime * speed;
        }
        else if (this.TimePassed <= this.FadeOutTime)
        {
            TimePassed += Time.deltaTime;
            this.Introduction.color = Color.Lerp(originalColor, Color.clear, this.TimePassed / this.FadeOutTime);
        }
        else
        {
            this.Introduction.color = Color.clear;
            SceneNavigator.LoadMainMenu();
        }
    }
}

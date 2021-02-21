using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionSetup : MonoBehaviour
{
    [SerializeField] AudioSource Audio = default;
    [SerializeField] float FadeInTime = 2f;
    [SerializeField] Image Image = default;
    [SerializeField] List<TMPro.TextMeshProUGUI> TitleScreen = default;
    [Range(0, 1)]
    [SerializeField] float speed = default;

    public void Start()
    {
        var color = Image.color;
        color.a = 0;
        Image.color = color;
        foreach (TMPro.TextMeshProUGUI line in TitleScreen)
        {
            color = line.color;
            color.a = 0;
            line.color = color;
        }
        StartFade(Audio, FadeInTime, 50);
    }

    public void Update()
    {
        if (Image.color.a <= 255)
        {
            var color = Image.color;
            color.a += Time.deltaTime * speed;
            Image.color = color;
            foreach (TMPro.TextMeshProUGUI line in TitleScreen)
            {
                if (line.color.a <= 255)
                {
                    color = line.color;
                    color.a += Time.deltaTime * speed;
                    line.color = color;
                }
            }
        }
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}

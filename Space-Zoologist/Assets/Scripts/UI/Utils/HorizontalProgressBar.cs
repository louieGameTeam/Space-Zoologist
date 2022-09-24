using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HorizontalProgressBar : MonoBehaviour
{
    public float MaxValue
    {
        get => maxValue;
        set
        {
            maxValue = value;
            UpdateBarProgress();
        }
    }
    public float CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = value;
            UpdateBarProgress();
        }
    }
    [SerializeField]
    [Tooltip("Blocking mask to change the bar's value")]
    private Image maskImage;
    [SerializeField]
    [Tooltip("Reference to the bar")]
    private Image barImage;
    [SerializeField]
    [Tooltip("Use mask to move bar instead of changing bar scale")]
    private bool useMask;
    [SerializeField]
    private float maxValue;
    [SerializeField]
    private float currentValue;

    // Gradient
    [SerializeField]
    private bool useColorGradient;
    [SerializeField]
    private Gradient barGradient;
    // Text
    [SerializeField]
    private bool displayBarText;
    [SerializeField]
    private TextMeshProUGUI barText;

    private void UpdateBarProgress()
    {
        float value = Mathf.Clamp01(currentValue / maxValue);
        if(maskImage)
        {
            maskImage.transform.localScale = new Vector3(1 - value, 1f, 1f);
        }
        else
        {
            barImage.transform.localScale = new Vector3(value, 1f, 1f);
        }
        if (displayBarText)
            barText.text = $"{currentValue}/{maxValue}";
        else
            barText.text = string.Empty;

        if (useColorGradient)
            barImage.color = barGradient.Evaluate(value);
    }
}

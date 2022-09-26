using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedSlider : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField] Text Name = null;
    [SerializeField] Slider lowSlider = null;
    [SerializeField] Slider highSlider = null;
    [SerializeField] Color lowNeedColor = Color.red;
    [SerializeField] Color highNeedColor = Color.blue;
    #endregion

    #region Monobehaviour Messages
    public void Awake()
    {
        SetValue(0);
    }
    #endregion

    #region Public Methods
    public void SetName(string name) {
        Name.text = name;
    }

    public void SetMinMax(float min, float max)
    {
        float midpoint = min + ((max - min) / 2);

        lowSlider.minValue = min;
        lowSlider.maxValue = midpoint;
        highSlider.minValue = midpoint;
        highSlider.maxValue = max;
    }

    public void SetValue(float value) {
        lowSlider.value = value;
        highSlider.value = value;

        // Try to get the fill image of the low slider
        Image fill = lowSlider.fillRect.GetComponent<Image>();

        // Check if the low slider has an image
        if (fill)
        {
            // If value is lower than the low slider max,
            // then set the low need color
            if (value < lowSlider.maxValue)
            {
                fill.color = lowNeedColor;
            }
            // Otherwise set the high need color
            else fill.color = highNeedColor;
        }
    }
    #endregion

    //private void updateGoodSlider(float value)
    //{
    //    if (value > max)
    //    {
    //        value = max;
    //    }
    //    highSlider.value = value;
    //    lowSlider.value = min;
    //}

    //private void updateBadSlider(float value)
    //{
    //    if (value < min)
    //    {
    //        value = min;
    //    }
    //    lowSlider.value = value;
    //    highSlider.value = 0;
    //}

    //public void SetCondition(NeedCondition condition) {
    //    switch (condition) {
    //        case NeedCondition.Neutral:
    //            if (!wasNeutral) {
    //                badFill.color = Color.yellow;
    //                goodFill.color = Color.yellow;
    //            }
    //            wasNeutral = true;
    //            break;
    //        default:
    //            if (wasNeutral)
    //            {
    //                badFill.color = Color.red;
    //                goodFill.color = Color.green;
    //            }
    //            wasNeutral = false;
    //            break;
    //    }
    //}
}

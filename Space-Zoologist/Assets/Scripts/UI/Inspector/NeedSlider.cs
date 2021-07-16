using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedSlider : MonoBehaviour
{
    [SerializeField] Text Name;
    [SerializeField] Slider badSlider;
    [SerializeField] Slider goodSlider;
    Image badHandle;
    Image goodHandle;

    public int max = 10;
    public int min = 0;

    public void Awake()
    {
        badHandle = badSlider.handleRect.GetComponent<Image>();
        goodHandle = goodSlider.handleRect.GetComponent<Image>();
        SetValue(0);
    }

    public void SetName(string name) {
        Name.text = name;
    }

    public void SetValue(float value) {
        badSlider.minValue = min;
        badSlider.maxValue = 0;
        goodSlider.minValue = 0;
        goodSlider.maxValue = max;
        

        if (value >= 0f)
        {
            updateGoodSlider(value);
        }
        else if (value <= 0f) {
            updateBadSlider(-value);
        }
    }

    private void updateGoodSlider(float value)
    {
        if (value > max)
        {
            value = max;
        }
        goodSlider.value = value;
        badSlider.value = min;
        badHandle.enabled = false;
        goodHandle.enabled = true;
    }

    private void updateBadSlider(float value)
    {
        if (value < min)
        {
            value = min;
        }
        badSlider.value = value;
        goodSlider.value = 0;
        badHandle.enabled = true;
        goodHandle.enabled = false;
    }

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

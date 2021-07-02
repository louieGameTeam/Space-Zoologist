using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedSlider : MonoBehaviour
{
    bool wasNegative = false;
    [SerializeField] Text Name;
    [SerializeField] Slider badSlider;
    [SerializeField] Slider goodSlider;
    Image badHandle;
    Image goodHandle;

    public void Awake()
    {
        badHandle = badSlider.handleRect.GetComponent<Image>();
        goodHandle = goodSlider.handleRect.GetComponent<Image>();
        SetValue(-1f);
    }

    public void SetName(string name) {
        Name.text = name;
    }

    public void SetValue(float value) {
        if (value > 1f)
        {
            SetValue(1f);
            return;
        }
        else if (value < -1f) {
            SetValue(-1f);
            return;
        }

        if (value >= 0f)
        {
            goodSlider.value = value;
            badSlider.value = 0f;
            if (wasNegative) {
                badHandle.enabled = false;
                goodHandle.enabled = true;
                wasNegative = false;
            }
        }
        else if (value <= 0f) {
            goodSlider.value = 0f;
            badSlider.value = -value;
            if (!wasNegative)
            {
                badHandle.enabled = true;
                goodHandle.enabled = false;
                wasNegative = true;
            }
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleSpriteSwap : UIBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the toggle")]
    private Toggle toggle;
    [SerializeField]
    [Tooltip("Graphic to change sprites on")]
    private Image image;
    [SerializeField]
    [Tooltip("Sprite while the toggle is on")]
    private Sprite onSprite;
    [SerializeField]
    [Tooltip("Sprite while the toggle is off")]
    private Sprite offSprite;

    protected override void Start()
    {
        toggle.onValueChanged.AddListener(OnToggleStateChanged);
        OnToggleStateChanged(toggle.isOn);
    }

    private void OnToggleStateChanged(bool isOn)
    {
        if (isOn) image.sprite = onSprite;
        else image.sprite = offSprite;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] Image Image = default;
    [SerializeField] Text Name = default;
    [SerializeField] Text Description = default;

    public void InitializeLevelUI(Level level)
    {
        this.Image.sprite = level.Sprite;
        this.Name.text = level.Name;
        this.Description.text = level.Description;
    }
}

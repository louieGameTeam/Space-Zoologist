using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileCount : MonoBehaviour
{
    [SerializeField] Text tileType;
    [SerializeField] Text count;
    [SerializeField] Image image;
    public void SetName(string name)
    {
        tileType.text = name;
    }

    public void SetImage(Sprite newSprite)
    {
        image.sprite = newSprite;
    }

    public void SetCount(int newCount) {
        count.text = "" + newCount;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasObjectStrobe : MonoBehaviour
{
    private bool strobing = false;

    public void StrobeColor( int _strobeCount, Color _toStrobe )
    {
        if (strobing)
            return;

        strobing = true;

        Image mySprite = this.GetComponent<Image>();
        Color oldColor = mySprite.color;
        StartCoroutine(StrobeColorHelper(0, (( _strobeCount * 2) - 1), mySprite, oldColor, _toStrobe));

    }

    public void StrobeAlpha( int _strobeCount, float a)
    {
        Image mySprite = this.GetComponent<Image>();
        Color toStrobe = new Color(mySprite.color.r, mySprite.color.b, mySprite.color.g, a);
        StrobeColor(_strobeCount, toStrobe);
    }

    private IEnumerator StrobeColorHelper( int _i, int _stopAt, Image _mySprite, Color _color, Color _toStrobe)
    {
        if(_i <= _stopAt)
        {
            if (_i % 2 == 0)
                _mySprite.color = _toStrobe;
            else
                _mySprite.color = _color;

            yield return new WaitForSeconds(.3f);
            StartCoroutine(StrobeColorHelper( (_i+1), _stopAt, _mySprite, _color, _toStrobe));
        }
        else
        {
            strobing = false;
        }
    }
}

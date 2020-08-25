using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObjectStrobe : MonoBehaviour
{
    private bool strobing = false;

    public void StrobeColor( int _strobeCount, Color _toStrobe )
    {
        if (strobing)
            return;

        strobing = true;

        SpriteRenderer mySprite = this.GetComponent<SpriteRenderer>();
        Color oldColor = mySprite.color;
        StartCoroutine(StrobeColorHelper(0, (( _strobeCount * 2) - 1), mySprite, oldColor, _toStrobe));

    }

    public void StrobeAlpha( int _strobeCount, float a)
    {
        SpriteRenderer mySprite = this.GetComponent<SpriteRenderer>();
        Color toStrobe = new Color(mySprite.color.r, mySprite.color.b, mySprite.color.g, a);
        StrobeColor(_strobeCount, toStrobe);
    }

    private IEnumerator StrobeColorHelper( int _i, int _stopAt, SpriteRenderer _mySprite, Color _color, Color _toStrobe)
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

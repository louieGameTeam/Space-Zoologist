using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidTileTester : MonoBehaviour
{
    public int liquid1 = 0;
    public int liquid2 = 0;
    public int liquid3 = 0;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        int[] liquidComposition = { liquid1, liquid2, liquid3 };
        spriteRenderer.color = RYBConverter.UpdateLiquidColor(liquidComposition);
    }
}
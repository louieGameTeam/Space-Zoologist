using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TempSurfaceEffect : MonoBehaviour
{
    // Start is called before the first frame update
    private Tilemap tilemap;
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        float alpha = Time.time * 2f;
        alpha = Mathf.Sin(alpha) * 0.4f + 0.6f;
        tilemap.color = new Vector4(1, 1, 1, alpha);
    }
}

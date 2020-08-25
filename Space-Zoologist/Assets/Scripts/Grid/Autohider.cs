using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Autohider : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.color = new Color(0, 0, 0, 0);
    }
}

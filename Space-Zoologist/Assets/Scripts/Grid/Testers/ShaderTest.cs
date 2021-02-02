using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShaderTest : MonoBehaviour
{
    public GameObject liquid;
    public GameObject grass;
    public GameObject animal;
    private SpriteRenderer animalRend;
    private TilemapRenderer liquidRend;
    private TilemapRenderer grassRend;
    [Range(-2.0f, 2.0f)]
    public float lightDirectionx;
    [Range(-2.0f, 2.0f)]
    public float lightDirectiony;
    [Range(0.0f, 1.0f)]
    public float color;
    public Color materialColor;
    // Update is called once per frame
    private void Awake()
    {
        animalRend = animal.GetComponent<SpriteRenderer>();
        liquidRend = liquid.GetComponent<TilemapRenderer>();
        grassRend = grass.GetComponent<TilemapRenderer>();
    }
    void Update()
    {
        float r = Mathf.Abs(Mathf.Sin(Time.time));
        float g = Mathf.Abs(Mathf.Sin(Time.time * 1.5f));
        float b = Mathf.Abs(Mathf.Sin(Time.time * 0.8f));
        materialColor = new Color(r, g, b, 0);
        Debug.Log(liquidRend.material.color);
        liquidRend.material.color = materialColor;
        grassRend.material.color = materialColor;
        animalRend.material.color = materialColor;
        Vector2 lightDirection = new Vector2(lightDirectionx, lightDirectiony);
        liquidRend.material.SetVector("Vector4_16D0E0A0", lightDirection);
        grassRend.material.SetVector("Vector2_7CA99A88", lightDirection);
        animalRend.material.SetVector("Vector2_7CA99A88", lightDirection);
    }
}

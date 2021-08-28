using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LiquidModificationHUD : MonoBehaviour
{
    [SerializeField] MenuManager MenuManager = default;
    [SerializeField] GameObject liquidModificationHUD = default;
    [SerializeField] List<TMPro.TMP_InputField> Values = default;
    [SerializeField] Camera mainCamera = default;
    private GridSystem GridSystem;
    private bool isOpened = false;
    private Vector3 worldPos;
    private LiquidBody liquidBody;
    void Start()
    {
        this.GridSystem = FindObjectOfType<GridSystem>();
        liquidModificationHUD.SetActive(false);
    }
    public void ParseValues()
    {
        if (this.isOpened)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                Debug.Log("start of text: " + Values[i].text[0]);
                if (Values[i].text[0].Equals("."))
                {
                    Debug.Log("Inserted 0");
                    Values[i].text.Insert(0, "0");
                }
                Debug.Log("Attempting to parse: " + Values[i].text);
                float output = 0;
                float.TryParse(Values[i].text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out output);
                Debug.Log("Parsed: " + output);
                liquidBody.contents[i] = output;
                
            }

        }
        if (liquidBody != null)
        {
            Debug.Log("Updated values");
            this.GridSystem.SetLiquidComposition(this.GridSystem.WorldToCell(this.worldPos), liquidBody.contents);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && MenuManager.IsInStore) //If clicking MMB on liquid tile, open HUD
        {
            liquidModificationHUD.SetActive(true);
            Vector3 mousePos = this.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = this.GridSystem.WorldToCell(mousePos);
            LiquidBody liquid = this.GridSystem.GetTileData(cellPosition) != null ? this.GridSystem.GetTileData(cellPosition).currentLiquidBody : null;
            if (liquid != null)
            {
                this.worldPos = new Vector3(mousePos.x, mousePos.y, mousePos.z);
                this.isOpened = true;
                this.liquidBody = liquid;
                for (int i = 0; i < Values.Count; i++)
                {
                    Values[i].text = "";
                    Values[i].text = liquidBody.contents[i].ToString();
                    
                }
            }
            else //If not clicked on a liquid tile, close HUD
            {
                liquidModificationHUD.SetActive(false);
                ParseValues();
            }
        }
        else if (!MenuManager.IsInStore && liquidModificationHUD.activeSelf)
        {
            ParseValues();
            liquidModificationHUD.SetActive(false);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LiquidModificationHUD : MonoBehaviour
{
    [SerializeField] MenuManager MenuManager = default;
    [SerializeField] GameObject liquidModificationHUD = default;
    [SerializeField] List<TMPro.TMP_InputField> Values = default;
    private TileDataController GridSystem;
    private bool isOpened = false;
    private Vector3 worldPos;
    void Start()
    {
        this.GridSystem = GameManager.Instance.m_tileDataController;
        liquidModificationHUD.SetActive(false);
    }
    public void ParseValues()
    {
        if (this.isOpened)
        {
            float[] newContents = new float[] { 0, 0, 0 };

            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i].text[0].Equals("."))
                {
                    Values[i].text.Insert(0, "0");
                }
                float output = 0;
                float.TryParse(Values[i].text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out output);
                Debug.Log("Parsed: " + output);
                newContents[i] = output;
            }

            LiquidbodyController.Instance.SetLiquidContentsAt(GridSystem.WorldToCell(worldPos), newContents);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(2) && MenuManager.IsInStore) //If clicking MMB on liquid tile, open HUD
        {
            liquidModificationHUD.SetActive(true);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = this.GridSystem.WorldToCell(mousePos);
            if (LiquidbodyController.Instance.GetLiquidContentsAt(cellPosition, out float[] contents, out bool constructing))
            {
                this.worldPos = new Vector3(mousePos.x, mousePos.y, mousePos.z);
                this.isOpened = true;
                for (int i = 0; i < Values.Count; i++)
                {
                    Values[i].text = contents.ToString();
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
            liquidModificationHUD.SetActive(false);
            ParseValues();
        }
    }
}
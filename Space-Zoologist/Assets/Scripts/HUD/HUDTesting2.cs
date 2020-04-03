using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDTesting2 : MonoBehaviour
{
    [SerializeField] private GameObject StoreDisplay = default;
    private GameObject StoreDisplayButton = default;
    private GameObject CloseDisplayButton = default;

    public void Start()
    {
        this.CloseDisplayButton = this.gameObject.transform.GetChild(0).gameObject;
        this.StoreDisplayButton = this.gameObject.transform.GetChild(1).gameObject;
    }

    public void DisplayStore()
    {
        this.StoreDisplay.SetActive(true);
        this.OpenNewDisplay();
    }

    public void OpenNewDisplay()
    {
        this.StoreDisplayButton.SetActive(false);
        this.CloseDisplayButton.SetActive(true);
    }

    public void CloseDisplays()
    {
        this.CloseStoreDisplay();
        this.StoreDisplayButton.SetActive(true);
        this.CloseDisplayButton.SetActive(false);
    }

    public void CloseStoreDisplay()
    {
        this.StoreDisplay.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Should HUD be listening for things or should game update HUD?
public class HUDTesting2 : MonoBehaviour
{
    [SerializeField] private GameObject StoreDisplay = default;
    [SerializeField] GameObject StoreDisplayButton = default;
    [SerializeField] GameObject CloseDisplayButton = default;
    [SerializeField] GameObject PlayerController = default;
    [SerializeField] GameObject PlayerFundsDisplay = default;
    public UnityEvent ItemSelected = new UnityEvent();
    private bool DisplayFunds = true;

    public void Start()
    {
        this.ItemSelected.AddListener(this.CloseStoreDisplay);
    }

    public void Update()
    {
        if (this.DisplayFunds)
        {
            // Should cache getcomponent references through a onetime method call
            PlayerController playerInfo = this.PlayerController.GetComponent<PlayerController>();
            SelectableItem itemSelected = playerInfo.ItemSelected.GetComponent<SelectableItem>();
            float cost = itemSelected.ItemInfo.ItemCost * playerInfo.NumCopies;
            this.DisplayCostOfPurchase(cost, playerInfo.PlayerFunds);
            // Have item image snap to player's cursor and follow
            // this.SpriteThatFollowsPlayerCursor = item.Sprite;
            // this.SpriteThatFollowsPlayerCurosor.transform.position = Vector3.Lerp(this.SpriteThatFollowsPlayerCurosor.transform.position, Input.mousePosition, FractionOfJourney);
        }
    }



    public void DisplayStore()
    {
        this.StoreDisplay.SetActive(true);
        this.CloseDisplayButton.SetActive(true);
        this.DisplayFunds = true;
    }

    public void CloseStoreDisplay()
    {
        this.StoreDisplay.SetActive(false);
        this.CloseDisplayButton.SetActive(false);
        this.StoreDisplayButton.SetActive(true);
    }

    public void ClosePlayerFundsDisplay()
    {
        this.DisplayFunds = false;
    }

    public void DisplayCostOfPurchase(float totalCost, float playerFunds)
    {
        if (totalCost > 0)
        {
            this.PlayerFundsDisplay.GetComponent<Text>().text = "Funds: " + playerFunds +
            "(-" + totalCost + ")";
        }
        else
        {
            this.PlayerFundsDisplay.GetComponent<Text>().text = "Funds: " + playerFunds;
        }
    }
}

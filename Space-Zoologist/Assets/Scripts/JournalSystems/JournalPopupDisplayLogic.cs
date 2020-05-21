using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets up and displays whatever is given to it
/// /// </summary>
public class JournalPopupDisplayLogic : MonoBehaviour
{
    public void RemoveSelfFromList(GameObject item)
    {
        Destroy(item);
    }

    public void UpdatePopupActiveSelf()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDesignManager : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.m_tilePlacementController.godMode = true;
    }
}

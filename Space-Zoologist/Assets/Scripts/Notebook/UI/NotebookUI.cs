using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookUI : MonoBehaviour
{
    //private void Awake()
    //{
    //    gameObject.SetActive(false);
    //}
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}

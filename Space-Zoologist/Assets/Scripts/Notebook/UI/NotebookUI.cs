using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookUI : MonoBehaviour
{
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}

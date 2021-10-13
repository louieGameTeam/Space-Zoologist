using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookDebugging : MonoBehaviour
{
    public Transform[] cornerObjects;

    private void OnDrawGizmos()
    {
        MoveCornerObjects();
    }

    private void Start()
    {
        MoveCornerObjects();
    }

    private void MoveCornerObjects()
    {
        Vector3[] corners = new Vector3[4];
        (transform as RectTransform).GetWorldCorners(corners);

        for (int i = 0; i < corners.Length && i < cornerObjects.Length; i++)
        {
            cornerObjects[i].position = corners[i];
        }
    }
}

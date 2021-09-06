using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CanvasDrawingObject : MonoBehaviour
{
    #region Public Properties
    public DrawingCursor CurrentCursor { get; private set; }
    #endregion

    #region Public Methods
    public virtual void StartDrawing(DrawingCursor owner)
    {
        CurrentCursor = owner;
    }
    public abstract void UpdateDrawing();
    public abstract void FinishDrawing();
    #endregion
}

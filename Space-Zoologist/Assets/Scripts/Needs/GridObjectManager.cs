using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all managers of objects on the grid. 
/// Call base.Start() in inherited members
/// </summary>

public class GridObjectManager : MonoBehaviour
{
    // Start is called before the first frame update
    protected string MapObjectName { get { return GetMapObjectName(); } } //Name of the type of object managed, used in S/L save files
    /// <summary>
    /// Registers Manager in GridIO to be referenced at S/L, needs to be done before parsing (at least before parsing map objects)
    /// </summary>
    public virtual void Start()
    {
        PlotIO gridIO = FindObjectOfType<PlotIO>();
        gridIO.RegisterManager(this);
    }
    public virtual void Serialize(SerializedMapObjects serializedMapObjects)
    {

    }
    public virtual void Parse(SerializedMapObjects serializedMapObjects)
    {

    }
    protected virtual string GetMapObjectName()
    {
        return null;
    }
}

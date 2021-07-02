using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all managers of objects on the grid. 
/// </summary>

public class GridObjectManager : MonoBehaviour
{
    protected SerializedMapObjects SerializedMapObjects;
    // Start is called before the first frame update
    public string MapObjectName { get { return GetMapObjectName(); } } //Name of the type of object managed, used in S/L save files

    public virtual void Serialize(SerializedMapObjects serializedMapObjects)
    {

    }
    public virtual void Parse()
    {
        
    }
    public void Store(SerializedMapObjects serializedMapObjects)
    {
        this.SerializedMapObjects = serializedMapObjects;
    }
    protected virtual string GetMapObjectName()
    {
        return null;
    }
}

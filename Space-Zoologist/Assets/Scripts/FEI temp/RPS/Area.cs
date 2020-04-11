using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Deprecated] A class that represents the area that a population lives on
/// </summary>
public class Area
{
    //only contain a List of Vector3Int, defines what positions are accessible
    public List<Vector3Int> map;
    public Area(){
        map = new List<Vector3Int>();
    }
    public Area(List<Vector3Int> field) {
        map = field;
    }
    public List<Vector3Int> GetMap() {
        return map;
    }
    public virtual Atmosphere GetAtmosphere() {
        //TODO, return public area's atmosphere
        return null;
    }
}

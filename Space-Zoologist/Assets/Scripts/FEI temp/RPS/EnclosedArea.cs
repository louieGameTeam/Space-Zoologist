using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Deprecated] A class that represents the closed area that a population lives on
/// </summary>
public class EnclosedArea : Area
{
    public Atmosphere Atmosphere { get; private set; }
        
    //constructors
    public EnclosedArea() : base() {}
    public EnclosedArea(List<Vector3Int> field) : base(field){
        Atmosphere = new Atmosphere();
    }
    public EnclosedArea(List<Vector3Int> field, float gasx, float gasy, float gasz, float temp) : base(field) {
        Atmosphere = new Atmosphere(gasx, gasy, gasz, temp);
    }
    public EnclosedArea(List<Vector3Int> field, Atmosphere atm) : base(field)
    {
        Atmosphere = atm;
    }
}

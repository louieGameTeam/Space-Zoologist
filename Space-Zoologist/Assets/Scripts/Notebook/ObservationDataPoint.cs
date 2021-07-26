using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObservationDataPoint : System.IComparable<ObservationDataPoint>
{
    public string Label { get; set; }
    public Vector2 Point { get; set; }

    public ObservationDataPoint(string Label, Vector2 Point)
    {
        this.Label = Label;
        this.Point = Point;
    }

    public int CompareTo(ObservationDataPoint other)
    {
        return Point.x.CompareTo(other.Point.x);
    }
}

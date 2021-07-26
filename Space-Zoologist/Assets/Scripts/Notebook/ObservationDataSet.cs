using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObservationDataSet
{
    private string label;
    private Color color;
    private List<ObservationDataPoint> points;

    public ObservationDataSet(string label, Color color)
    {
        this.label = label;
        this.color = color;
        points = new List<ObservationDataPoint>();
    }

    public void AddPoint(ObservationDataPoint point)
    {
        points.Add(point);
        points.Sort();
    }
}

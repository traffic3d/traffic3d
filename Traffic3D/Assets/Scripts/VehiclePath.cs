using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehiclePath
{
    public Color lineColor;
    public List<Transform> nodes = new List<Transform>();

    public VehiclePath(List<Transform> nodes)
    {
        this.nodes = nodes;
    }
}

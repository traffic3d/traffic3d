using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNodePathFindInfo
{
    public RoadNode roadNode;
    public float gCost;
    public float hCost;
    public RoadNodePathFindInfo parent;

    public RoadNodePathFindInfo(RoadNode roadNode)
    {
        this.roadNode = roadNode;
    }
}

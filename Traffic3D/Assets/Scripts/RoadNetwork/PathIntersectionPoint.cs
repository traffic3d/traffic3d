using System;
using UnityEngine;

public class PathIntersectionPoint
{
    public PathIntersectionLine line1;
    public PathIntersectionLine line2;
    public Vector3 intersection;
    public float angleFromLine1ToLine2;

    public PathIntersectionPoint(Transform line1p1Node, Transform line1p2Node, Transform line2p1Node, Transform line2p2Node, Vector3 intersection)
    {
        line1 = new PathIntersectionLine(line1p1Node, line1p2Node, intersection);
        line2 = new PathIntersectionLine(line2p1Node, line2p2Node, intersection);
        this.intersection = intersection;
        Vector3 line1Vector = line1p2Node.position - line1p1Node.position;
        Vector3 line2Vector = line2p2Node.position - line2p1Node.position;
        Debug.Log("Main: " + line1Vector.ToString());
        Debug.Log("Line2: " + line2Vector.ToString());
        angleFromLine1ToLine2 = Vector3.SignedAngle(line1Vector, line2Vector, Vector3.up);
        Debug.Log("Angle: " + angleFromLine1ToLine2);
    }

    public bool IsNodeBeforeIntersection(Transform node)
    {
        return line1.IsFirstPoint(node) || line2.IsFirstPoint(node);
    }

    public bool IsNodeAfterIntersection(Transform node)
    {
        return line1.IsLastPoint(node) || line2.IsLastPoint(node);
    }

    public bool IsNodeAheadOfIntersection(Transform node, VehiclePath vehiclePath)
    {
        return vehiclePath.nodes.IndexOf(GetLineFromPath(vehiclePath).firstPoint) < vehiclePath.nodes.IndexOf(node);
    }

    public bool IsOtherPathComingFromTheLeft(PathIntersectionLine currentIntersecionLine)
    {
        return GetAngleFromLine(currentIntersecionLine) > 0;
    }

    public bool IsOtherPathComingFromTheRight(PathIntersectionLine currentIntersecionLine)
    {
        return GetAngleFromLine(currentIntersecionLine) < 0;
    }

    public PathIntersectionLine GetLineFromPath(VehiclePath vehiclePath)
    {
        if (vehiclePath.nodes.Contains(line1.firstPoint) && vehiclePath.nodes.Contains(line1.lastPoint))
        {
            return line1;
        }
        else if (vehiclePath.nodes.Contains(line2.firstPoint) && vehiclePath.nodes.Contains(line2.lastPoint))
        {
            return line2;
        }
        else
        {
            throw new ArgumentException("Vehicle path doesn't have this intersection.");
        }
    }

    public float GetAngleFromLine(PathIntersectionLine intersecionLine)
    {
        if (intersecionLine == line1)
        {
            return angleFromLine1ToLine2;
        }
        else if (intersecionLine == line2)
        {
            return -angleFromLine1ToLine2;
        }
        else
        {
            throw new ArgumentException("PathIntersectionLine doesn't exist in this intersection.");
        }
    }

    public PathIntersectionLine GetPathIntersectionLineFromNodes(Transform firstIntersectionNode, Transform lastIntersectionNode)
    {
        if (line1.IsFirstPoint(firstIntersectionNode) && line1.IsLastPoint(lastIntersectionNode))
        {
            return line1;
        }
        else if (line2.IsFirstPoint(firstIntersectionNode) && line2.IsLastPoint(lastIntersectionNode))
        {
            return line2;
        }
        else
        {
            return null;
        }
    }

    public float CalculateVehicleDistanceToIntersectionWithinIntersection(Transform vehicleTransform, Transform firstIntersectionNode, Transform lastIntersectionNode)
    {
        PathIntersectionLine line = GetPathIntersectionLineFromNodes(firstIntersectionNode, lastIntersectionNode);
        if (line == null)
        {
            // Unable to calculate as nodes are not within interesection line.
            return float.NaN;
        }
        float distanceFromFirstNode = Vector3.Distance(firstIntersectionNode.position, vehicleTransform.position);
        float distanceFromLastNode = Vector3.Distance(lastIntersectionNode.position, vehicleTransform.position);
        float vehicleNodeRatioFromFirst = distanceFromFirstNode / (distanceFromFirstNode + distanceFromLastNode);
        float nodeRatioFromFirst = line.distanceFromFirstToIntersection / line.distanceTotal;
        return Mathf.Max((nodeRatioFromFirst - vehicleNodeRatioFromFirst) * line.distanceTotal, 0);
    }
}

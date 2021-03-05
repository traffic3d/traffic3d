using UnityEngine;

public class PathIntersectionPoint
{
    public PathIntersectionLine line1;
    public PathIntersectionLine line2;
    public Vector3 intersection;

    public PathIntersectionPoint(Transform line1p1Node, Transform line1p2Node, Transform line2p1Node, Transform line2p2Node, Vector3 intersection)
    {
        line1 = new PathIntersectionLine(line1p1Node, line1p2Node, intersection);
        line2 = new PathIntersectionLine(line2p1Node, line2p2Node, intersection);
        this.intersection = intersection;
    }

    public bool IsNodeBeforeIntersection(Transform node)
    {
        return line1.IsFirstPoint(node) || line2.IsFirstPoint(node);
    }

    public bool IsNodeAfterIntersection(Transform node)
    {
        return line1.IsLastPoint(node) || line2.IsLastPoint(node);
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

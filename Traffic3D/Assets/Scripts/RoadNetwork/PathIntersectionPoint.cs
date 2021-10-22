using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Holds values of an intersection such as the intersection position itself, the two lines, and the angle from line 1 to 2.
/// </summary>
public class PathIntersectionPoint
{
    public PathIntersectionLine line1;
    public PathIntersectionLine line2;
    public Vector3 intersection;
    public float angleFromLine1ToLine2;

    /// <summary>
    /// Constructs the Intersection object.
    /// Lines are a section of path.
    /// </summary>
    /// <param name="line1p1Node">The first point on the first line</param>
    /// <param name="line1p2Node">The last point on the first line</param>
    /// <param name="line2p1Node">The first point on the second line</param>
    /// <param name="line2p2Node">The last point on the second line</param>
    /// <param name="intersection">The position of where the lines intersect</param>
    public PathIntersectionPoint(Transform line1p1Node, Transform line1p2Node, Transform line2p1Node, Transform line2p2Node, Vector3 intersection)
    {
        line1 = new PathIntersectionLine(line1p1Node, line1p2Node, intersection);
        line2 = new PathIntersectionLine(line2p1Node, line2p2Node, intersection);
        this.intersection = intersection;
        Vector3 line1Vector = line1p2Node.position - line1p1Node.position;
        Vector3 line2Vector = line2p2Node.position - line2p1Node.position;
        angleFromLine1ToLine2 = Vector3.SignedAngle(line1Vector, line2Vector, Vector3.up);
    }

    /// <summary>
    /// Checks if the input node the first node on either lines.
    /// e.g. Normally before the main intersection.
    /// </summary>
    /// <param name="node">The node to check</param>
    /// <returns>True if node is a first node in the intersection</returns>
    public bool IsNodeFirstPointOfIntersection(Transform node)
    {
        return line1.IsFirstPoint(node) || line2.IsFirstPoint(node);
    }

    /// <summary>
    /// Checks if the input node the last node on either lines.
    /// e.g. Normally before the main intersection.
    /// </summary>
    /// <param name="node">The node to check</param>
    /// <returns>True if node is a last node in the intersection</returns>
    public bool IsNodeLastPointOfIntersection(Transform node)
    {
        return line1.IsLastPoint(node) || line2.IsLastPoint(node);
    }

    /// <summary>
    /// Checks if the input node is ahead of the main intersection point.
    /// </summary>
    /// <param name="node">The node to check</param>
    /// <param name="vehiclePath">The vehicle path to check</param>
    /// <returns>True if the input node is ahead of the main intersection point</returns>
    public bool IsNodeAheadOfIntersection(Transform node, VehiclePath vehiclePath)
    {
        return vehiclePath.nodes.IndexOf(GetLineFromPath(vehiclePath).firstPoint) < vehiclePath.nodes.IndexOf(node);
    }

    /// <summary>
    /// Checks whether the incoming path (the other line) is coming from the inputted direction of the current intersection line.
    /// </summary>
    /// <param name="currentIntersecionLine">The current line to check</param>
    /// <returns>True if its from the left</returns>
    public bool IsIncomingPathFromDirection(Direction direction, PathIntersectionLine currentIntersecionLine)
    {
        float angle = GetAngleFromLineToOtherLine(currentIntersecionLine);
        switch (direction)
        {
            case Direction.LEFT:
                return angle > 0;
            case Direction.RIGHT:
                return angle < 0;
            case Direction.STRAIGHT_AHEAD:
                return angle == 0;
        }
        throw new ArgumentException("Unknown direction: " + direction);
    }

    /// <summary>
    /// Gets the current line the vehicle path will use when crossing this intersection.
    /// </summary>
    /// <param name="vehiclePath">The vehicle path to check</param>
    /// <returns>The line that will be used by the vehicle path</returns>
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
            VehicleNavigation vehicle = GameObject.FindObjectsOfType<VehicleNavigation>().Where(v => v.path == vehiclePath).FirstOrDefault();
            string vehicleName = (vehicle != null) ? vehicle.gameObject.name : "Null";
            throw new ArgumentException("Vehicle path doesn't have this intersection. Vehicle: " + vehicleName + " - Intersection: " + ToString());
        }
    }

    /// <summary>
    /// Gets the smallest angle from the inputted line to the other intersection line
    /// </summary>
    /// <param name="intersecionLine">The line to measure</param>
    /// <returns>The smallest angle from the inputted line to the other line</returns>
    public float GetAngleFromLineToOtherLine(PathIntersectionLine intersecionLine)
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
            throw new ArgumentException("PathIntersectionLine doesn't exist in this intersection. PathIntersectionLine: " + intersecionLine.ToString());
        }
    }

    /// <summary>
    /// Gets Path Intersection Line from the inputted nodes
    /// </summary>
    /// <param name="firstIntersectionNode">The first point</param>
    /// <param name="lastIntersectionNode">The last point</param>
    /// <returns>PathIntersectionLine with the specified nodes, returns null if unknown</returns>
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

    /// <summary>
    /// Calculates the estimated distance from the vehicle to the intersection point when the vehicle is between the first and the last node of the intersection line.
    /// </summary>
    /// <param name="vehicleTransform">The vehicle to check</param>
    /// <param name="firstIntersectionNode">The first node of the intersection</param>
    /// <param name="lastIntersectionNode">The last node of the intersection</param>
    /// <returns>The distance to the intersection point from the vehicle as a float</returns>
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

    public override string ToString()
    {
        return "PathIntersectionPoint: " +
            "Line1: (" + line1.ToString() + ") - " +
            "Line2: (" + line2.ToString() + ") - " +
            "Intersection point: " + intersection.ToString();
    }

    /// <summary>
    /// The direction an incoming path is from.
    /// </summary>
    public enum Direction
    {
        RIGHT,
        LEFT,
        STRAIGHT_AHEAD
    }
}

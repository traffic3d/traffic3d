using UnityEngine;

public class PathIntersectionLine
{
    public Transform firstPoint;
    public Transform lastPoint;
    public float distanceTotal;
    public float distanceFromFirstToIntersection;
    public float distanceFromIntersectionToLast;

    public PathIntersectionLine(Transform firstPoint, Transform lastPoint, Vector3 intersection)
    {
        this.firstPoint = firstPoint;
        this.lastPoint = lastPoint;
        this.distanceTotal = Vector3.Distance(firstPoint.position, lastPoint.position);
        this.distanceFromFirstToIntersection = Vector3.Distance(firstPoint.position, intersection);
        this.distanceFromIntersectionToLast = Vector3.Distance(intersection, lastPoint.position);
    }

    public bool IsFirstPoint(Transform point)
    {
        return firstPoint == point;
    }

    public bool IsLastPoint(Transform point)
    {
        return lastPoint == point;
    }

    public override string ToString()
    {
        return "PathIntersectionLine: " +
            "firstPoint: " + firstPoint.name + " - " +
            "lastPoint: " + lastPoint.name + " - ";
    }
}

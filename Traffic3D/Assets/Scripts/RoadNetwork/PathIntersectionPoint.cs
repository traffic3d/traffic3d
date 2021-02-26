using UnityEngine;

public class PathIntersectionPoint
{
    public Transform line1p1Node;
    public Transform line1p2Node;
    public Transform line2p1Node;
    public Transform line2p2Node;
    public Vector3 intersection;

    public PathIntersectionPoint(Transform line1p1Node, Transform line1p2Node, Transform line2p1Node, Transform line2p2Node, Vector3 intersection)
    {
        this.line1p1Node = line1p1Node;
        this.line1p2Node = line1p2Node;
        this.line2p1Node = line2p1Node;
        this.line2p2Node = line2p2Node;
        this.intersection = intersection;
    }

    public bool IsNodeBeforeIntersection(Transform node)
    {
        return line1p1Node == node || line2p1Node == node;
    }

    public bool IsNodeAfterIntersection(Transform node)
    {
        return line1p2Node == node || line2p2Node == node;
    }
}

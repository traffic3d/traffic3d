using UnityEngine;

public class GizmoRay
{
    private Vector3 start;
    private Vector3 dir;
    private Color color;

    public GizmoRay(Vector3 start, Vector3 dir, Color color)
    {
        this.start = start;
        this.dir = dir;
        this.color = color;
    }

    public void Draw()
    {
        Gizmos.color = color;
        Gizmos.DrawRay(start, dir);
    }
}

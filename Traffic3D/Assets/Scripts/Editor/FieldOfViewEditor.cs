using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fieldOfView = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360, fieldOfView.viewRadius);
        Vector3 viewAngleA = fieldOfView.DirectionFromAngle(-fieldOfView.viewAngle / 2);
        Vector3 viewAngleB = fieldOfView.DirectionFromAngle(fieldOfView.viewAngle / 2);

        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleA * fieldOfView.viewRadius);
        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleB * fieldOfView.viewRadius);

        Handles.color = Color.red;

        foreach (Pedestrian visibleTarget in fieldOfView.allVisiblePedestrians)
        {
            Handles.DrawLine(fieldOfView.transform.position, visibleTarget.transform.position);
        }
    }
}

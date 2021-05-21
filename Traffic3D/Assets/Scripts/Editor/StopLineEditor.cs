using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds an Update Road Mesh button to the 'RoadMeshUpdator' Component
/// </summary>
[CustomEditor(typeof(StopLineRenderer))]
public class StopLineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StopLineRenderer stopLineRenderer = (StopLineRenderer)target;
        if (GUILayout.Button("Re-render Stop Line"))
        {
            stopLineRenderer.RenderStopLine();
        }
    }
}

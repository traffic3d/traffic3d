using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Adds an Update Road Mesh button to the 'RoadMeshUpdator' Component
/// </summary>
[CustomEditor(typeof(RoadMeshUpdater))]
public class RoadMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoadMeshUpdater roadMeshUpdater = (RoadMeshUpdater)target;
        if (GUILayout.Button("Update Road Mesh"))
        {
            roadMeshUpdater.UpdateRoadMesh();
        }
    }
}

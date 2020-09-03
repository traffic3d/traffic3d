using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadMeshUpdater))]
public class RoadMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoadMeshUpdater roadMeshUpdater = (RoadMeshUpdater)target;
        if (GUILayout.Button("Update Road Mesh"))
        {
            try
            {
                roadMeshUpdater.UpdateRoadMesh();
            }
            catch (MissingReferenceException)
            {
                Debug.Log("Error: After modifying a vehicle path, please click on " +
                    "the nodes' parent object (with the <Path> component attached) " +
                    "to refresh the list of nodes, before updating the road mesh.");
            }
        }
    }
}

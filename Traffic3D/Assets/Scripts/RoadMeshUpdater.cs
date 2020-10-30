using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component: Allows users to update a Road Mesh, by using the corresponding vehicle Path.
/// </summary>
public class RoadMeshUpdater : MonoBehaviour
{
    public GameObject road; // Road to which this Script is attached
    public Path vehiclePath;
    public int numLanes;
    public float laneWidth; // Width of each lane

    /// <summary>
    /// Set the public values of the component
    /// </summary>
    /// <param name="numLanes"></param>
    /// <param name="road">GameObject to which this script is attached to</param>
    /// <param name="path">GameObject of corresponding vehicle Path. Must have 'Path' component</param>
    /// <param name="laneWidth">Width of each lane</param>
    public void SetValues(int numLanes, GameObject road, Path path, float laneWidth)
    {
        this.road = road;
        this.vehiclePath = path;
        this.numLanes = numLanes;
        this.laneWidth = laneWidth;
    }

    /// <summary>
    /// Update Mesh of current road gameObject to match the positions of the nodes along the corresponding vehicle path.
    /// Ensures Road label is repositioned above center node.
    /// Paths with less than 2 nodes will have an empty mesh.
    /// </summary>
    public void UpdateRoadMesh()
    {
        Mesh mesh = new Mesh(); //default = empty mesh

        if (vehiclePath != null)
        {
            vehiclePath.SetNodes(); //Updates node list to match child objects

            if (vehiclePath.nodes.Count >1)
            {
                //road center should be relative to path and no longer the map
                road.transform.position = vehiclePath.transform.position;

                Transform roadNameLabel = GetRoadLabel();
                List<Vector3> roadNodePositions = ConvertPathToRoadVectorList(vehiclePath);
                RoadGenerationHandler rgh = new RoadGenerationHandler();

                mesh = rgh.CreateRoadMesh(roadNodePositions, numLanes, laneWidth); // Generate Mesh
                UpdateRoadLabelPosition(roadNodePositions); // update label position
            }
        }

        road.GetComponent<MeshFilter>().mesh = mesh;
    }

    /// <summary>
    /// Position label above middle node
    /// </summary>
    /// <param name="nodePositions">List of node positions, as Vector3s'</param>
    void UpdateRoadLabelPosition(List<Vector3> nodePositions)
    {
        Transform roadNameLabel = GetRoadLabel();

        //If road has a name/label, move it to roads new position
        if (roadNameLabel != null)
        {
            //Get middle Node index in path. (Round down to nearest int. -1 for index position)
            int middleIndex = middleIndex = (int)Math.Floor((decimal)((nodePositions.Count - 1) / 2));

            //Get mid + next node position
            Vector3 midNodePos = nodePositions[middleIndex];
            Vector3 nextNodePos = nodePositions[middleIndex + 1];

            RoadGenerationHandler rgh = new RoadGenerationHandler();

            //Move label to updated position
            rgh.PositionRoadLabel(midNodePos, nextNodePos, nodePositions.Count, roadNameLabel.gameObject, roadNameLabel.GetComponent<TextMesh>(),true);
        }
    }

    /// <summary>
    /// creates a list of Vector3s for the positions of the nodes in a Path
    /// </summary>
    /// <param name="path">Vehicle Path</param>
    /// <returns>List of Vector3s for the nodes in a path</returns>
    List<Vector3> ConvertPathToRoadVectorList(Path path)
    {
        List<Vector3> nodePos = new List<Vector3>(path.nodes.Count);

        foreach (Transform t in path.nodes)
        {
            //Vector position of road is -1 in Y-Axis of node y-position
            Vector3 v = new Vector3(t.localPosition.x, t.localPosition.y -1, t.localPosition.z );
            nodePos.Add(v);
        }
        return nodePos;
    }

    Transform GetRoadLabel()
    {
        if (road.transform.childCount > 0)
            return road.transform.GetChild(0);
        return null;
    }

}

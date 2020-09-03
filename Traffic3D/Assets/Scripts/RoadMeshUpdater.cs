using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMeshUpdater : MonoBehaviour
{
    public GameObject road;
    public GameObject pathObject;
    public int numLanes;
    public float laneWidth;

    public void setValues(int numLanes, GameObject road, GameObject path, float laneWidth)
    {
        this.road = road;
        this.pathObject = path;
        this.numLanes = numLanes;
        this.laneWidth = laneWidth;
    }

    public void UpdateRoadMesh()
    {
        Mesh mesh = new Mesh();

        if (pathObject != null)
        {
            Path vehiclePath = pathObject.GetComponent<Path>();
            if (vehiclePath.nodes.Count >1)
            {

                //road center should be relative to path and no longer the map
                road.transform.position = pathObject.transform.position;
                Transform roadNameLabel = GetRoadLabel();
                List<Vector3> roadNodePositions = ConvertPathToRoadVectorList(vehiclePath);


                RoadGenerationHandler rgh = new RoadGenerationHandler();

                //Generate Mesh
                mesh = rgh.CreateRoadMesh(roadNodePositions, numLanes, laneWidth);
                UpdateRoadLabelPosition(roadNodePositions);
            }
            
        }

        road.GetComponent<MeshFilter>().mesh = mesh;


    }

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

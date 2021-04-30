using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the creation of the road mesh and positioning of label above road
/// </summary>
public class RoadGenerationHandler
{
    private const float roadLabelHeightAboveRoad = 12;

    /// <summary>
    /// Uses a list of node positions to generate a Road Mesh
    /// </summary>
    /// <param name="nodePositions">List of Vector3s, creating the road. (Vector3s' should be the position of nodes along the road)</param>
    /// <param name="roadWidth">Total Width of the road</param>
    /// <returns> Returns new mesh for road. OR, if list doesn't contain atleast 2 nodes, returns empty mesh. </returns>
    public Mesh CreateRoadMesh(List<Vector3> nodePositions, float laneWidth)
    {
        int numNodes = nodePositions.Count;

        if (numNodes <= 1)
            return new Mesh();

        Vector3[] verts = new Vector3[numNodes * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int numTris = 2 * (numNodes - 1);
        int[] tris = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < numNodes; i++)
        {
            Vector3 currentNodeLoc = nodePositions[i]; // Next Nodes' Location
            Vector2 forward = Vector2.zero;
            //For all but last node: Get forward between current & next Node
            if (i < numNodes - 1)
            {
                Vector3 nextNodeLoc = nodePositions[i + 1];// Next Nodes' Location
                Vector2 cur = new Vector2(currentNodeLoc.x, currentNodeLoc.z);
                Vector2 next = new Vector2(nextNodeLoc.x, nextNodeLoc.z);
                forward += next - cur;
            }
            //For all but first node: Get forward between current & previous Node
            if (i > 0)
            {
                Vector3 prevNodeLoc = nodePositions[i - 1]; // Next Nodes' Location
                Vector2 cur = new Vector2(currentNodeLoc.x, currentNodeLoc.z);
                Vector2 prev = new Vector2(prevNodeLoc.x, prevNodeLoc.z);
                forward += cur - prev;
            }
            forward.Normalize(); // Determine the average between the two forward vectors

            //Calculate offseted positions for mesh
            Vector2 left = new Vector2(-forward.y, forward.x);
            Vector3 vleft = new Vector3(left.x, 0, left.y);
            verts[vertIndex] = currentNodeLoc + vleft * laneWidth * .5f; //node to left of current
            verts[vertIndex + 1] = currentNodeLoc - vleft * laneWidth * .5f; //node to right of current
            float positionAcrossMesh = i / (float)(numNodes - 1); // % Across mesh
            float uvCoordinate = 1 - Mathf.Abs(2 * positionAcrossMesh - 1); // uv co-ordinate
            uvs[vertIndex] = new Vector2(0, uvCoordinate); // Set x to 0 and y to uvCoordinate
            uvs[vertIndex + 1] = new Vector2(1, uvCoordinate); // Set x to 1 and y to uvCoordinate on next UV

            if (i < numNodes - 1)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
            }
            vertIndex += 2;
            triIndex += 6;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        return mesh;
    }

    /// <summary>
    /// Positions the label (showing the road name) above the center node and orientate along road.
    /// </summary>
    /// <param name="midNode">Middle node in road</param>
    /// <param name="nextNode">Node after the middle node in road</param>
    /// <param name="numNodes">Total number of nodes in the road</param>
    /// <param name="roadName">GameObject holding Component with TestMesh</param>
    /// <param name="roadNameLabel">TeshMesh Component holding road Name</param>
    /// <param name="localPosition">Boolean. Whether roadName is positioned using the localPosition or global position.</param>
    public void PositionRoadLabel(Vector3 midNode, Vector3 nextNode, int numNodes, GameObject roadName, TextMesh roadNameLabel, bool localPosition)
    {
        Vector3 relativePos = nextNode - midNode;
        Quaternion rotation = Quaternion.Euler(Vector3.zero);
        if (relativePos != Vector3.zero)
            rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        //Position label in centre of road
        roadName.transform.rotation = rotation; //rotate parallel to road
        roadName.transform.Rotate(90, 90, 0); // make flat and along road
        //if only 2 nodes, position label in centre of road
        if (numNodes <= 2)
        {
            Vector3 midPoint = (midNode + nextNode) / 2f;
            if (localPosition)
                roadName.transform.localPosition = midNode;
            else
                roadName.transform.position = midNode;
        }
        else
        {
            if (localPosition)
                roadName.transform.localPosition = midNode;
            else
                roadName.transform.position = midNode;
        }
        //increase label height above road
        roadName.transform.position = new Vector3(roadName.transform.position.x, roadName.transform.position.y + roadLabelHeightAboveRoad, roadName.transform.position.z);
    }
}

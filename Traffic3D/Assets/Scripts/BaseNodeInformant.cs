using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds shared information and functionality between PathGenerator and JunctionGenerator
/// Information is used to merge roads without the continious need for large loops.
/// </summary>
public abstract class BaseNodeInformant 
{
    //Maps all nodes at beginning of a path to a HashSet of all nodes sharing the same "Start_Nodes" -- (Key: Start_Node_Position, Value: "Dictionary = {Key: Road_Name, Value:List_of_Roads}" )
    protected static Dictionary<Vector3, Dictionary<string, HashSet<GameObject>>> startNodes;
    //All nodes at sharing the same end node: Key="End Node Position", Value = "Set of all VehiclePaths with same end Node"
    protected static Dictionary<Vector3, HashSet<GameObject>> endNodes;
    //Get all vehiclePaths connected to a node - Key: - Node position, Value: HashSet of all VehiclePath connected to node
    protected static Dictionary<Vector3, HashSet<GameObject>> roadsIndexdByNodes;
    //All nodes that connect to another road but aren't a end/start node
    protected static Dictionary<Vector3, HashSet<GameObject>> midConnectedNodes;
    //Links NodeID to VehiclePath
    protected static Dictionary<string, GameObject> nodeObjectsByNodeId;
    //Holds all created VehiclePaths
    protected static HashSet<GameObject> createdRoads;
    //Holds VehiclePaths that were deleted after merging roads
    protected static List<GameObject> deletedVehiclePaths;
    // Root for all Road Nodes
    protected static GameObject roadNodeRootParent;

    protected void InitializeVariables()
    {
        startNodes = new Dictionary<Vector3, Dictionary<string, HashSet<GameObject>>>();
        endNodes = new Dictionary<Vector3, HashSet<GameObject>>();
        roadsIndexdByNodes = new Dictionary<Vector3, HashSet<GameObject>>();
        midConnectedNodes = new Dictionary<Vector3, HashSet<GameObject>>();
        createdRoads = new HashSet<GameObject>();
        deletedVehiclePaths = new List<GameObject>();
        nodeObjectsByNodeId = new Dictionary<string, GameObject>();
        if(roadNodeRootParent == null)
        {
            roadNodeRootParent = new GameObject("RoadNodes");
        }
    }

    /// <summary>
    /// Will insert a new node in the path if the distance between the current node and next node in junction is too far
    /// </summary>
    /// <param name="maxDistanceAprat">threshold before new node made</param>
    /// <param name="newNodedistanceApart">how far new node is from target</param>
    /// <param name="vehiclePath">Path script containing list of nodes</param>
    /// <param name="currentNode">target </param>
    /// <param name="previousNode"></param>
    /// <param name="currentNodeIndex">index of node in path</param>
    /// <param name="addBeforeCurrentNode">bool: do you want new node before the current node or after</param>
    /// <returns>New node created between two target nodes, or NULL if no new node created</returns>
    protected RoadNode InsertPathNodeBetweenDistantNodes(float maxDistanceAprat, float newNodedistanceApart, RoadWay roadWay, Transform currentNode, RoadNode previousNode, int currentNodeIndex, bool addBeforeCurrentNode)
    {
        //Get distance between junction node and previous node
        float distance = Vector3.Distance(currentNode.transform.position, previousNode.transform.position);
        if (distance > maxDistanceAprat)
        {
            GameObject newNodeGameObject = new GameObject();
            newNodeGameObject.transform.SetParent(roadNodeRootParent.transform, true);
            RoadNode newNode = newNodeGameObject.AddComponent<RoadNode>();
            //Add node to roadway
            if (addBeforeCurrentNode)
                roadWay.nodes.Insert(currentNodeIndex, newNode); //add just before junction
            else
                roadWay.nodes.Insert(currentNodeIndex + 1, newNode); //add just after junction

            newNode.transform.position = currentNode.transform.position;
            //Move towards previous node
            newNode.transform.position = Vector3.MoveTowards(newNode.transform.position, previousNode.transform.position, newNodedistanceApart);
            //Add layer to ignore to raycasts
            newNodeGameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            newNodeGameObject.name = PathGenerator.GenerateRoadNodeName(newNode.transform.position);
            return newNode;
        }
        return null;
    }

    //testing
    public int GetNumOfTotalEndNodes()
    {
        int totalEndNodes = 0;
        foreach (HashSet<GameObject> endNodeRoads in endNodes.Values)
                totalEndNodes += endNodeRoads.Count;

        return totalEndNodes;
    }

    //testing
    public int GetNumOfTotalStartNodes()
    {
        int totalStartNodes = 0;
        foreach (Dictionary<string, HashSet<GameObject>> roadsIndexedByName in startNodes.Values)
        {
            foreach (HashSet<GameObject> roadsWithSameName in roadsIndexedByName.Values)
                totalStartNodes += roadsWithSameName.Count;
        }
        return totalStartNodes;
    }

    //testing
    public int GetNumCreatedRoads()
    {
        return createdRoads.Count;
    }

    //testing
    public int GetNumMidConnectedRoads()
    {
        int totalMidConnectedNodes = 0;
        foreach (HashSet<GameObject> midNodeRoads in midConnectedNodes.Values)
            totalMidConnectedNodes += midNodeRoads.Count;

        return totalMidConnectedNodes;
    }
}

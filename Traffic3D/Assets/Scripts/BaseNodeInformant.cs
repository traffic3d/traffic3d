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
    protected static Dictionary<ulong, GameObject> nodeObjectsByNodeId;

    //Holds all created VehiclePaths
    protected static HashSet<GameObject> createdRoads;

    //Holds VehiclePaths that were deleted after merging roads
    protected static List<GameObject> deletedVehiclePaths;

    protected void InitializeVariables()
    {
        startNodes = new Dictionary<Vector3, Dictionary<string, HashSet<GameObject>>>();

        endNodes = new Dictionary<Vector3, HashSet<GameObject>>();

        roadsIndexdByNodes = new Dictionary<Vector3, HashSet<GameObject>>();

        midConnectedNodes = new Dictionary<Vector3, HashSet<GameObject>>();

        createdRoads = new HashSet<GameObject>();

        deletedVehiclePaths = new List<GameObject>();

        nodeObjectsByNodeId = new Dictionary<ulong, GameObject>();
    }


    /// <summary>
    /// Will merge the secondary road into the primary road.
    /// Requires roads to be travelling in the same direction.
    /// </summary>
    /// <param name="primary">GameObject with 'path' component. This road will be extended</param>
    /// <param name="secondary">GameObject with 'path' component. This road Will be deleted</param>
    /// <returns>Bool, If merged roads successfully (will fail if merging into itself e.g roundabout)</returns>
    protected bool MergeTwoRoads(GameObject primary, GameObject secondary)
    {
        //Check if road is merging with itself (e.g roundabouts)
        if (primary == secondary)
            return false;

        if (primary == null | secondary == null)
            return false;

        int primaryNodesCount = primary.GetComponent<Path>().nodes.Count;

        DeleteMergingRoadReferences(primary, secondary);

        //Move all nodes from secondary to primary path
        foreach (var node in secondary.GetComponent<Path>().nodes)
        {
            //skip first node, as Secondary path begins where Primary path ends
            if (node != secondary.GetComponent<Path>().nodes[0])
            {
                //re-name node
                primaryNodesCount++;
                node.name = "node" + primaryNodesCount;

                //Make node a child of primary path
                node.transform.SetParent(primary.transform, true);

                //Replace node references
                roadsIndexdByNodes[node.position].Remove(secondary);
                roadsIndexdByNodes[node.position].Add(primary);
            }
        }

        //Replace node reference for first node (skipped in foreach loop)
        roadsIndexdByNodes[secondary.GetComponent<Path>().nodes[0].position].Remove(secondary);

        //Update Path class to recognise new nodes
        primary.GetComponent<Path>().SetNodes();

        //Mark as deleted
        deletedVehiclePaths.Add(secondary);

        //update road list
        createdRoads.Remove(secondary); //remove deleted node

        if (primary.name != secondary.name)
        {
            primary.transform.parent.name = primary.name + " + " + secondary.name;
        }

        return true;
    }

    /// <summary>
    /// Delete & update 'start' and 'end' node references for the two merging roads 
    /// </summary>
    /// <param name="primary">GameObject with 'path' component. This road will be extended</param>
    /// <param name="secondary">GameObject with 'path' component. This road Will be deleted</param>
    void DeleteMergingRoadReferences(GameObject primary, GameObject secondary)
    {
        List<Transform> primaryPathNodes = primary.GetComponent<Path>().nodes;
        List<Transform> secondaryPathNodes = secondary.GetComponent<Path>().nodes;

        Vector3 secondaryPathEndNode = secondaryPathNodes[secondaryPathNodes.Count - 1].position;
        Vector3 nodeWherePathsMeet = primaryPathNodes[primaryPathNodes.Count - 1].position;

        // -- Primary path references

        //Primary path will no longer end at node -> Delete end node reference
        endNodes[nodeWherePathsMeet].Remove(primary);

        //Primary path will end where secondary path ends -> Update end node reference
        endNodes[secondaryPathEndNode].Add(primary);

        // -- Secondary path references

        //Secondary Path will be deleted -> Delete from "start_nodes"
        startNodes[nodeWherePathsMeet][secondary.name].Remove(secondary);

        //Check if 'any' roads with 'same name' still start at node
        if (startNodes[nodeWherePathsMeet][secondary.name].Count == 0)
        {
            startNodes[nodeWherePathsMeet].Remove(secondary.name);

            //check if 'any' roads still start at node
            if (startNodes[nodeWherePathsMeet].Count == 0)
                startNodes.Remove(nodeWherePathsMeet); // remove key
        }

        //Secondary Path will be deleted -> delete from "End_nodes"
        endNodes[secondaryPathEndNode].Remove(secondary);

        //Check if any paths still end at node
        if (endNodes[nodeWherePathsMeet].Count == 0)
            endNodes.Remove(nodeWherePathsMeet); // remove key

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
    protected GameObject InsertPathNodeBetweenDistantNodes(float maxDistanceAprat, float newNodedistanceApart, Path vehiclePath, Transform currentNode, Transform previousNode, int currentNodeIndex, bool addBeforeCurrentNode)
    {
        //Get distance between junction node and previous node
        float distance = Vector3.Distance(currentNode.position, previousNode.position);

        if (distance > maxDistanceAprat)
        {
            GameObject newNode = new GameObject();
            newNode.name = "node" + (vehiclePath.nodes.Count + 1);


            //Add node to vehicle path
            newNode.transform.parent = vehiclePath.transform;

            if (addBeforeCurrentNode)
                newNode.transform.SetSiblingIndex(currentNodeIndex); //add just before junction
            else
                newNode.transform.SetSiblingIndex(currentNodeIndex + 1); //add just after junction

            newNode.transform.position = currentNode.position;

            vehiclePath.SetNodes();

            //Move towards previous node
            newNode.transform.position = Vector3.MoveTowards(newNode.transform.position, previousNode.position, newNodedistanceApart);

            //Add layer to ignore to raycasts
            newNode.layer = LayerMask.NameToLayer("Ignore Raycast");

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

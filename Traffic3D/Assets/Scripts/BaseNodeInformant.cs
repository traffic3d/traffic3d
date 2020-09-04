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
    /// Will join the two passed roads together
    /// </summary>
    /// <param name="primary">GameObject with <path> component. {End Node}</param>
    /// <param name="secondary">GameObject with <path> component. Will be deleted {Start Node}</param>
    /// <returns>Bool, If merged roads successfully (will fail if merging into self e.g roundabout)</returns>
    protected bool MergeTwoRoads(GameObject primary, GameObject secondary)
    {

        //Check if road is merging with itself (e.g roundabouts)
        if (primary == secondary)
            return false;

        if (primary == null | secondary == null)
            return false;

        List<Transform> primaryPathNodes = primary.GetComponent<Path>().nodes;
        List<Transform> secondaryPathNodes = secondary.GetComponent<Path>().nodes;

        //get latest nodes id
        int id = primaryPathNodes.Count;


        //delete primary_path "end node reference" 
        endNodes[primaryPathNodes[primaryPathNodes.Count - 1].position].Remove(primary);

        //Add primary_path "end node reference" to secondary "End Node" reference 
        endNodes[secondaryPathNodes[secondaryPathNodes.Count - 1].position].Add(primary);

        //Move all nodes from connected road to current road
        foreach (var node in secondary.GetComponent<Path>().nodes)
        {
            //skip first node since primary path end node = secondarys' path first node
            if (node != secondary.GetComponent<Path>().nodes[0])
            {

                //increment node id
                id++;

                node.name = "node" + id; //rename node to math new list

                //Make node a child of current road
                node.transform.SetParent(primary.transform, true);


                //Replace references to old road with new road
                roadsIndexdByNodes[node.position].Remove(secondary);
                roadsIndexdByNodes[node.position].Add(primary);
            }

        }

        //Update Path class to deal with new nodes
        primary.GetComponent<Path>().SetNodes();

        // - Delete references to old path

        //delete old Vehicle path from "start_nodes" - StartNodes.[FirstNode].[RoadName].Remove[road]
        startNodes[secondaryPathNodes[0].position][secondary.name].Remove(secondary);


        //delete old Vehicle path from "End_nodes" - StartNodes.[FirstNode].[RoadName].Remove[road]
        endNodes[secondaryPathNodes[secondaryPathNodes.Count - 1].position].Remove(secondary);

        //Mark as deleted
        deletedVehiclePaths.Add(secondary);

        //update road list
        createdRoads.Remove(secondary); //remove deleted node

        //if merging two nodes who have difference names
        if (primary.name != secondary.name)
        {
            primary.transform.parent.name = primary.name + " + " + secondary.name;
        }

        return true;
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
            {
                newNode.transform.SetSiblingIndex(currentNodeIndex); //add just before junction
            }
            else
            {
                newNode.transform.SetSiblingIndex(currentNodeIndex + 1); //add just after junction
            }

            newNode.transform.position = currentNode.position;
            vehiclePath.SetNodes();

            //Move towards previous node
            newNode.transform.position = Vector3.MoveTowards(newNode.transform.position, previousNode.position, newNodedistanceApart);

            //Add layer to ignore to raycasts
            newNode.layer = 2;

            return newNode;
        }

        return null;

    }

}

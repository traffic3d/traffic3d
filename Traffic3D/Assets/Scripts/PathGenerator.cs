﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Generates paths for vehicle to drive along (using Path component)
/// </summary>
public class PathGenerator : BaseNodeInformant
{
    private OpenStreetMapReader osmMapReader;
    private GameObject vehiclePath;
    private VehicleFactory vehicleFactory;

    public PathGenerator(OpenStreetMapReader osmMapReader, VehicleFactory vehicleFactory)
    {
        //initialize base calss variables
        InitializeVariables();
        
        //Create list
        List<Path> path = new List<Path>();

        //Initialize list in VF
        vehicleFactory.paths = path;

        this.vehicleFactory = vehicleFactory;
        this.osmMapReader = osmMapReader;
    }

    /// <summary>
    /// Loop through ways and create path for each road
    /// </summary>
    /// <param name="wayObjects">Dictionary linking parent gameObject to way - {Key: way, Value: parent game Object}</param>
    public void AddPathsToRoads(Dictionary<MapXmlWay, GameObject> wayObjects)
    {

        // Iterate through each way...
        foreach (var way in wayObjects.Keys)
        {
            //if way is a road...
            if (way.IsRoad)
            {
                //Create GameObject to hold vehicle path 
                vehiclePath = new GameObject();
                vehiclePath.AddComponent<Path>();

                //Create new vehicle path and add to vehicleFactory
                CreatePath(way, way.Name, vehiclePath, vehicleFactory);

                //Make vehicle path child of way objects' parent
                SetParent(way, wayObjects);

                //record as created
                createdRoads.Add(vehiclePath);
            }
        }
        PopulateVehicleFactory();
    }

    /// <summary>
    /// Connect roads with the same name, if they join at same node
    /// </summary>
    public void JoinRoadsWithSameName()
    {
       HashSet<GameObject> temp = new HashSet<GameObject>(createdRoads);

        //Loop through all roads
        foreach (GameObject road in temp)
        {
            //Check if is a deleted road
            if (!deletedVehiclePaths.Contains(road))
                MergeToSimularEndNode(road);
            
        }
    }

    public List<GameObject> GetDeletedVehiclePaths()
    {
        return deletedVehiclePaths;
    }

    /// <summary>
    /// Creates a Vehicle Path and adds to vehicle factory
    /// - Creates a new gameObject with "Path" Component. 
    /// - Populates Path component using nodes found in "Way" 
    /// </summary>
    /// <param name="way">Way holding all nodes in the road</param>
    /// <param name="pathName">Name of the new vehicle path</param>
    /// <param name="vehiclePath">GameObject to hold "Path" Script</param>
    /// <param name="vf">GameObject with Vehicle Factory script</param>
    void CreatePath(MapXmlWay way, string pathName, GameObject vehiclePath, VehicleFactory vehicleFactory)
    {
        vehiclePath.name = string.IsNullOrEmpty(pathName) ? "Road_Path" : (pathName + "_Path");
        Vector3 origin = GetCentre(way);
        
        //position path
        vehiclePath.transform.position = origin - osmMapReader.bounds.Centre;

        //Add layer to 'ignore raycasts' to path object
        vehiclePath.layer = LayerMask.NameToLayer("Ignore Raycast");

        //Loop through all nodes in the road
        for (int i = 0; i < way.NodeIDs.Count; i++)
        {
            //Create GameObject for node
            string name = "node" + (i + 1);
            GameObject singleNode = new GameObject(name);

            //Add layer to ignore to raycasts to node
            singleNode.layer = LayerMask.NameToLayer("Ignore Raycast");

            MapXmlNode currentNodeLocation = osmMapReader.nodes[way.NodeIDs[i]];// Current Nodes' Location
            Vector3 vCurrentNodeLocation = origin - currentNodeLocation;// Node vector location

            //move up along y-axis so node is above road
            vCurrentNodeLocation.y = vCurrentNodeLocation.y - 1;

            //Set position of Node to the vector
            singleNode.transform.position = vCurrentNodeLocation * (-1f);

            //Make node a child of main roads' path 
            singleNode.transform.SetParent(vehiclePath.transform, false);

            //Store node by ID
            StoreNodeObjectByNodeId(singleNode, way.NodeIDs[i]);

            //Rotate the first node so it faces to the second. (Vehicle spawn in the direction of the first node)
            if (i == 1)
            {
                //Direction from 1st to 2nd node
                Transform firstNode = singleNode.transform.parent.GetChild(0).transform;
                Vector3 relativePos = singleNode.transform.position - firstNode.position;

                // the second argument, upwards, defaults to Vector3.up
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

                firstNode.rotation = rotation;
            }
        }

        Path path = vehiclePath.GetComponent<Path>();
        
        // Sets the nodes of the path using the child game objects.
        path.SetNodes();

        //Set the line colour
        path.lineColor = Color.white;
        path.lineColor.a = 1;
        
        List<Path> vf_path = vehicleFactory.paths;

        vf_path.Add(path); //Add path to vehicle factory

        //Save details about nodes used in current path
        RecordPathData(vehiclePath);

    }

    /// <summary>
    /// Add node to dictionary
    /// </summary>
    /// <param name="singleNode">Node GameObject</param>
    /// <param name="id">node id</param>
    void StoreNodeObjectByNodeId(GameObject singleNode, ulong id)
    {
        //If node not already stored, create new Key-Value pair
        if (!nodeObjectsByNodeId.ContainsKey(id))
            nodeObjectsByNodeId.Add(id, singleNode);
        else
            nodeObjectsByNodeId[id] = singleNode; //overwrite old value
    }

    /// <summary>
    /// Return all nodes in road network
    /// </summary>
    /// <returns>Dictionary {Key: Node (MapXmlWay) ID, Value: Node Game Object}</returns>
    public Dictionary<ulong, GameObject> GetAllNodesInRoadNetwork()
    {
        return nodeObjectsByNodeId;
    }

    /// <summary>
    /// Make Vehicle child of ways parent GameObject
    /// </summary>
    /// <param name="way">way</param>
    /// <param name="wayObjects">Dictionary linking way to its parent object</param>
    public void SetParent(MapXmlWay way, Dictionary<MapXmlWay, GameObject> wayObjects)
    {
        GameObject parent;

        //if way has parent
        if (wayObjects.ContainsKey(way))
        {
            //get parent
            parent = wayObjects[way];
        }
        else
        {
            //create new parent
            parent = new GameObject();
            parent.name = vehiclePath.name;

            wayObjects.Add(way, parent);
        }

        //make vehiclePath child of parent
        vehiclePath.transform.parent = parent.transform;

    }

    /// <summary>
    /// Record: Start, Middle, End and used nodes in current Vehicle Path
    /// </summary>
    /// <param name="vehiclePath">Gameobject with 'Path' component attached, whose node data is being recorded</param>
    void RecordPathData(GameObject vehiclePath)
    {
        //Get nodes in current path
        List<Transform> nodes = vehiclePath.GetComponent<Path>().nodes;

        Vector3 startNode = nodes[0].position;
        Vector3 endNode = nodes[nodes.Count-1].position;

        // - Record Start Node 

        //if startNode already added
        if (startNodes.ContainsKey(startNode))
        {
            //Get dictionary {Key: Road_Name, Value: HashSet of roads}
            Dictionary<string, HashSet<GameObject>> roadsByName = startNodes[startNode];

            //If road_name is already a key
            if (roadsByName.ContainsKey(vehiclePath.name))
            {
                //Add current road as value to key
                roadsByName[vehiclePath.name].Add(vehiclePath);//Add road 
            }
            else
            {
                //make new Key-Value pair {Key: Road_Name, Value: Hashset -> road}
                HashSet<GameObject> roads = new HashSet<GameObject>();
                roads.Add(vehiclePath);
                roadsByName.Add(vehiclePath.name, roads);
            }
        }
        else
        {
            //Create Dictionary linking "road_name" to a Hashset of "road_objects"
            Dictionary<string, HashSet<GameObject>> roadsByName = new Dictionary<string, HashSet<GameObject>>();
            HashSet<GameObject> roads = new HashSet<GameObject>();
            roads.Add(vehiclePath);
            roadsByName.Add(vehiclePath.name, roads);

            //Link roadsByName to startNodes
            startNodes.Add(startNode, roadsByName);
        }

        // - Record End Node 

        //if EndNode already added
        if (endNodes.ContainsKey(endNode))
        {
            //Get Set of all vehicle_Path with same end node
            HashSet<GameObject> roads = endNodes[endNode];
            //Add vehiclePath to set
            roads.Add(vehiclePath);
        }
        else
        {
            //Create & populate HashSet linking "VehiclePath" to "EndNode"
            HashSet<GameObject> roads = new HashSet<GameObject>();
            roads.Add(vehiclePath);
            endNodes.Add(endNode, roads);
        }

        
        // - Record all nodes used in path
        foreach (Transform node in nodes)
        {
            //if node already added
            if (roadsIndexdByNodes.ContainsKey(node.position))
            {
                //add current vehiclePath to hashSet
                roadsIndexdByNodes[node.position].Add(vehiclePath);
            }
            else
            {
                //create hashset
                HashSet<GameObject> roads = new HashSet<GameObject>();
                roads.Add(vehiclePath);
                //add new Key-Value pair for current node
                roadsIndexdByNodes.Add(node.position, roads);
            }
        }
    }
    

    /// <summary>
    /// Checks if the end of the current road connects to another road with 'the same name' & 'travelling in the same direction',
    /// and will merge the roads together.
    /// </summary>
    /// <remarks>
    /// The road object passed will be extended by having the connected roads' nodes added to it.
    /// The second road will be deleted.
    /// This is a recursive method which merges roads until no simular roads are left.
    /// </remarks>
    /// <param name="road">Game Object with 'Path' component attached, which will be extended if merged with another road.</param>
    void MergeToSimularEndNode(GameObject road)
    {
        Path path = road.GetComponent<Path>();

        Vector3 endNode = path.nodes[path.nodes.Count - 1].position; //get paths' end node

        //If there are roads connected to the end of the current road
        if (startNodes.ContainsKey(endNode))
        {
            //Get all who start at the end of the current path/road
            Dictionary<string, HashSet<GameObject>> startNodesRoadsByName = startNodes[endNode];

            //Check if any of the roads connected to the end of current road have the same name
            if (startNodesRoadsByName.ContainsKey(road.name))
            {
                //Get all connected roads, with same name 
                HashSet<GameObject> roadsConnectedAtEnd = startNodesRoadsByName[road.name];

                //if connects to only 1 road (Traffic3D doesn't currently support paths forking)
                if (roadsConnectedAtEnd.Count == 1)
                {
                    //Get the only road in hashset
                    GameObject vehiclePathToMerge = roadsConnectedAtEnd.ElementAt(0);

                    //Merge two roads
                    bool successfulMerge = MergeTwoRoads(road, vehiclePathToMerge);

                    //if failed to merge roads, return
                    if (!successfulMerge)
                        return;

                    //After merging, the end node was updated => Call recursive method to merge again
                    MergeToSimularEndNode(road);
                }
            }
        }
    }

    /// <summary>
    /// Checks for any path with only 2 nodes and adds a 3rd node, just before the last node. 
    /// Dev Note: This is to prevent errors with the Vehicle Factory, which requires paths to have 3+ nodes
    /// </summary>
    public void RemovePathsWithTwoNodes()
    {
        foreach (GameObject road in createdRoads)
        {
            Path path = road.GetComponent<Path>();
            //if path only has 2 nodes, add a third node.
            if (path.nodes.Count == 2)
            {
                InsertPathNodeBetweenDistantNodes(0f, 0.5f, path, path.nodes[1], path.nodes[0], 1, true);
                path.SetNodes();
            }
        }
    }

    /// <summary>
    /// Empties and re-populates the Vehicle Factory with road paths.
    /// </summary>
    public void PopulateVehicleFactory()
    {
        //Create list
        List<Path> paths = new List<Path>();

        //Loop through all roads
        foreach (GameObject road in createdRoads)
        {
            //if not a deleted road
            if (!deletedVehiclePaths.Contains(road))
                paths.Add(road.GetComponent<Path>());
        }
        vehicleFactory.paths = paths;
    }


    /// <summary>
    /// Returns the centre of an object
    /// </summary>
    /// <param name="way">MapXmlWay object</param>
    /// <returns></returns>
    protected Vector3 GetCentre(MapXmlWay way)
    {
        Vector3 total = Vector3.zero;

        foreach (var id in way.NodeIDs)
        {
            total = total + osmMapReader.nodes[id];
        }

        return total / way.NodeIDs.Count;
    }


}

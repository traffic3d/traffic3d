using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates paths for vehicle to drive along (using Path component)
/// </summary>
public class PathGenerator : BaseNodeInformant
{
    private OpenStreetMapReader osmMapReader;
    private RoadWay roadWay;

    public PathGenerator(OpenStreetMapReader osmMapReader)
    {
        // Initialize base class variables
        InitializeVariables();
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
            // if way is a road...
            if (way.IsRoad)
            {
                // Create GameObject to hold vehicle path 
                roadWay = new GameObject().AddComponent<RoadWay>();

                // Create new path
                CreateRoadWay(way, way.Name, roadWay);

                // Make vehicle path child of way objects' parent
                SetParent(way, wayObjects);

                // record as created
                createdRoads.Add(roadWay.gameObject);
            }
        }
    }

    public List<GameObject> GetDeletedVehiclePaths()
    {
        return deletedVehiclePaths;
    }

    /// <summary>
    /// Creates a Vehicle Path
    /// - Creates a new gameObject with "Path" Component. 
    /// - Populates Path component using nodes found in "Way" 
    /// </summary>
    /// <param name="way">Way holding all nodes in the road</param>
    /// <param name="pathName">Name of the new vehicle path</param>
    void CreateRoadWay(MapXmlWay way, string pathName, RoadWay roadWay)
    {
        roadWay.name = string.IsNullOrEmpty(pathName) ? "Road_Path" : (pathName + "_Path");
        Vector3 origin = GetCentre(way);
        // Add layer to 'ignore raycasts' to path object
        roadWay.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        // Loop through all nodes in the road
        GameObject firstNode = null;
        for (int i = 0; i < way.NodeIDs.Count; i++)
        {
            RoadNode roadNode;
            // Check if node already exists
            if (nodeObjectsByNodeId.ContainsKey(way.NodeIDs[i]))
            {
                GameObject existingNode = nodeObjectsByNodeId[way.NodeIDs[i]];
                roadNode = existingNode.GetComponent<RoadNode>();
            }
            else
            {
                // Create GameObject for node
                string name = "node" + way.NodeIDs[i];
                GameObject singleNode = new GameObject(name);
                // Add layer to ignore to raycasts to node
                singleNode.layer = LayerMask.NameToLayer("Ignore Raycast");
                MapXmlNode currentNodeLocation = osmMapReader.nodes[way.NodeIDs[i]];// Current Nodes' Location
                Vector3 vCurrentNodeLocation = osmMapReader.bounds.Centre - currentNodeLocation;// Node vector location
                // move up along y-axis so node is above road
                vCurrentNodeLocation.y = vCurrentNodeLocation.y - 1;
                // Set position of Node to the vector
                singleNode.transform.position = vCurrentNodeLocation * (-1f);
                // Make node a child of main root
                singleNode.transform.SetParent(roadNodeRootParent.transform, true);
                // Store node by ID
                StoreNodeObjectByNodeId(singleNode, way.NodeIDs[i]);
                // Rotate the first node so it faces to the second. (Vehicle spawn in the direction of the first node)
                if (i == 1 && firstNode != null)
                {
                    // Direction from 1st to 2nd node
                    Vector3 relativePos = singleNode.transform.position - firstNode.transform.position;
                    // the second argument, upwards, defaults to Vector3.up
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                    firstNode.transform.rotation = rotation;
                }
                roadNode = singleNode.AddComponent<RoadNode>();
                if (i == 0)
                {
                    roadNode.startNode = true;
                    firstNode = singleNode;
                }
            }
            roadWay.nodes.Add(roadNode);
        }
        // Save details about nodes used in current path
        RecordRoadWayData(roadWay.gameObject);
    }

    /// <summary>
    /// Add node to dictionary
    /// </summary>
    /// <param name="singleNode">Node GameObject</param>
    /// <param name="id">node id</param>
    void StoreNodeObjectByNodeId(GameObject singleNode, ulong id)
    {
        // If node not already stored, create new Key-Value pair
        if (!nodeObjectsByNodeId.ContainsKey(id))
            nodeObjectsByNodeId.Add(id, singleNode);
        else
            nodeObjectsByNodeId[id] = singleNode; // overwrite old value
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

        // if way has parent
        if (wayObjects.ContainsKey(way))
        {
            // get parent
            parent = wayObjects[way];
        }
        else
        {
            // create new parent
            parent = new GameObject();
            parent.name = roadWay.name;

            wayObjects.Add(way, parent);
        }

        // make vehiclePath child of parent
        roadWay.transform.parent = parent.transform;

    }

    /// <summary>
    /// Record: Start, Middle, End and used nodes in current Vehicle Path
    /// </summary>
    /// <param name="roadWayObject">Gameobject with 'RoadWay' component attached, whose node data is being recorded</param>
    void RecordRoadWayData(GameObject roadWayObject)
    {
        // Get nodes in current path
        List<RoadNode> nodes = roadWayObject.GetComponent<RoadWay>().nodes;

        Vector3 startNode = nodes[0].transform.position;
        Vector3 endNode = nodes[nodes.Count - 1].transform.position;

        // - Record Start Node 

        // if startNode already added
        if (startNodes.ContainsKey(startNode))
        {
            // Get dictionary {Key: Road_Name, Value: HashSet of roads}
            Dictionary<string, HashSet<GameObject>> roadsByName = startNodes[startNode];

            // If road_name is already a key
            if (roadsByName.ContainsKey(roadWayObject.name))
            {
                // Add current road as value to key
                roadsByName[roadWayObject.name].Add(roadWayObject); // Add road 
            }
            else
            {
                // make new Key-Value pair {Key: Road_Name, Value: Hashset -> road}
                HashSet<GameObject> roads = new HashSet<GameObject>();
                roads.Add(roadWayObject);
                roadsByName.Add(roadWayObject.name, roads);
            }
        }
        else
        {
            // Create Dictionary linking "road_name" to a Hashset of "road_objects"
            Dictionary<string, HashSet<GameObject>> roadsByName = new Dictionary<string, HashSet<GameObject>>();
            HashSet<GameObject> roads = new HashSet<GameObject>();
            roads.Add(roadWayObject);
            roadsByName.Add(roadWayObject.name, roads);

            // Link roadsByName to startNodes
            startNodes.Add(startNode, roadsByName);
        }

        // - Record End Node 

        // if EndNode already added
        if (endNodes.ContainsKey(endNode))
        {
            // Get Set of all vehicle_Path with same end node
            HashSet<GameObject> roads = endNodes[endNode];
            // Add vehiclePath to set
            roads.Add(roadWayObject);
        }
        else
        {
            // Create & populate HashSet linking "VehiclePath" to "EndNode"
            HashSet<GameObject> roads = new HashSet<GameObject>();
            roads.Add(roadWayObject);
            endNodes.Add(endNode, roads);
        }


        // - Record all nodes used in path
        foreach (RoadNode node in nodes)
        {
            // if node already added
            if (roadsIndexdByNodes.ContainsKey(node.transform.position))
            {
                // add current vehiclePath to hashSet
                roadsIndexdByNodes[node.transform.position].Add(roadWayObject);
            }
            else
            {
                // create hashset
                HashSet<GameObject> roads = new HashSet<GameObject>();
                roads.Add(roadWayObject);
                //add new Key-Value pair for current node
                roadsIndexdByNodes.Add(node.transform.position, roads);
            }
        }
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

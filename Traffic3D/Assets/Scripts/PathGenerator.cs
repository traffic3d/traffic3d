using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates paths for vehicle to drive along (using Path component)
/// </summary>
public class PathGenerator : BaseNodeInformant
{
    private OpenStreetMapReader osmMapReader;

    private Dictionary<Vector3, int> sameNodeCount;
    private List<Vector3> connectionPositions;
    private const float maxConnectionDistance = 20f;
    private bool isLeftHandDrive;

    public PathGenerator(OpenStreetMapReader osmMapReader, bool isLeftHandDrive)
    {
        // Initialize base class variables
        InitializeVariables();
        this.osmMapReader = osmMapReader;
        this.isLeftHandDrive = isLeftHandDrive;
        connectionPositions = new List<Vector3>();
        sameNodeCount = new Dictionary<Vector3, int>();
    }

    /// <summary>
    /// Loop through ways and create path for each road
    /// </summary>
    /// <param name="wayObjects">Dictionary linking parent gameObject to way - {Key: way, Value: parent game Object}</param>
    public void AddPathsToRoads(Dictionary<MapXmlWay, GameObject> wayObjects)
    {
        foreach (var way in wayObjects.Keys)
        {
            if (way.IsRoad)
            {
                // Create new paths
                List<RoadWay> roadWays = CreateRoadWays(way, way.Name);

                // Make vehicle path child of way objects' parent
                foreach (RoadWay roadWay in roadWays)
                {
                    RecordRoadWayData(roadWay.gameObject);
                    SetParent(way, roadWay, wayObjects);
                    createdRoads.Add(roadWay.gameObject);
                }

                Road road = wayObjects[way].GetComponentInChildren<Road>();
                if (road != null)
                {
                    road.roadWays.AddRange(roadWays);
                    road.numberOfLanes = roadWays.Count;
                }
            }
        }
        AddConnectionsBetweenRoads();
    }

    public void AddConnectionsBetweenRoads()
    {
        connectionPositions = sameNodeCount.Where(e => e.Value > 1).Select(e => e.Key).ToList();
        RoadNetworkManager.GetInstance().Reload();
        foreach (Vector3 connectionPosition in connectionPositions)
        {
            Dictionary<RoadNode, float> closeRoadNodesAndDistance = RoadNetworkManager.GetInstance().GetNodes()
                .ToDictionary(r => r, r => Vector3.Distance(connectionPosition, r.transform.position)).OrderBy(e => e.Value)
                .Where(e => e.Value < maxConnectionDistance).ToDictionary(e => e.Key, e => e.Value);
            List<List<RoadWay>> roadWaysConnected = new List<List<RoadWay>>();
            foreach (KeyValuePair<RoadNode, float> roadNodeDistanceCurrent in closeRoadNodesAndDistance)
            {
                foreach (KeyValuePair<RoadNode, float> roadNodeDistanceCompare in closeRoadNodesAndDistance)
                {
                    if (roadNodeDistanceCurrent.Key == roadNodeDistanceCompare.Key)
                    {
                        continue;
                    }
                    List<RoadWay> currentRoadWays = RoadNetworkManager.GetInstance().GetRoadWaysFromNode(roadNodeDistanceCurrent.Key);
                    List<RoadWay> compareRoadWays = RoadNetworkManager.GetInstance().GetRoadWaysFromNode(roadNodeDistanceCompare.Key);
                    foreach (RoadWay currentRoadWay in currentRoadWays)
                    {
                        foreach (RoadWay compareRoadWay in compareRoadWays)
                        {
                            if (roadWaysConnected.Any(l => l.First().Equals(currentRoadWay) && l.Last().Equals(compareRoadWay)))
                            {
                                continue;
                            }
                            // If same road (under same parent) then continue.
                            if (currentRoadWay.transform.parent.Equals(compareRoadWay.transform.parent))
                            {
                                continue;
                            }
                            RoadWay newRoadWay = CreateRoadWay(currentRoadWay.name + "_and_" + compareRoadWay.name);
                            newRoadWay.nodes.Add(roadNodeDistanceCurrent.Key);
                            newRoadWay.nodes.Add(roadNodeDistanceCompare.Key);
                            newRoadWay.transform.parent = currentRoadWay.transform.parent;
                            roadWaysConnected.Add(new List<RoadWay>() { currentRoadWay, compareRoadWay });
                        }
                    }
                }
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
    List<RoadWay> CreateRoadWays(MapXmlWay way, string pathName)
    {

        int forwardLanes = 0;
        int backwardLanes = 0;

        if (way.Tags.ContainsKey("oneway") && (way.Tags["oneway"] == "yes" || way.Tags["oneway"] == "true" || way.Tags["oneway"] == "1"))
        {
            forwardLanes = 1;
            backwardLanes = 0;
            if (way.Tags.ContainsKey("lanes"))
            {
                int.TryParse(way.Tags["lanes"], out forwardLanes);
            }
        }
        else
        {
            forwardLanes = 1;
            backwardLanes = 1;
            if (way.Tags.ContainsKey("lanes"))
            {
                int.TryParse(way.Tags["lanes"], out forwardLanes);
                int.TryParse(way.Tags["lanes"], out backwardLanes);
            }
            if (way.Tags.ContainsKey("lanes:forward"))
            {
                int.TryParse(way.Tags["lanes:forward"], out forwardLanes);
            }
            if (way.Tags.ContainsKey("lanes:backward"))
            {
                int.TryParse(way.Tags["lanes:backward"], out backwardLanes);
            }
        }

        List<Vector3> nodePositions = new List<Vector3>();
        for (int i = 0; i < way.NodeIDs.Count; i++)
        {
            MapXmlNode currentNodeLocation = osmMapReader.nodes[way.NodeIDs[i]]; // Current Nodes' Location
            Vector3 vCurrentNodeLocation = osmMapReader.bounds.Centre - currentNodeLocation; // Node vector location
            vCurrentNodeLocation.y = vCurrentNodeLocation.y - 1; // move up along y-axis so node is above road
            vCurrentNodeLocation = vCurrentNodeLocation * (-1f);
            nodePositions.Add(vCurrentNodeLocation);
            if (sameNodeCount.ContainsKey(vCurrentNodeLocation))
            {
                sameNodeCount[vCurrentNodeLocation]++;
            }
            else
            {
                sameNodeCount.Add(vCurrentNodeLocation, 1);
            }
        }
        // Save details about nodes used in current path
        return CreateRoadWays(way, nodePositions, forwardLanes, backwardLanes);
    }

    private List<RoadWay> CreateRoadWays(MapXmlWay way, List<Vector3> nodePositions, int forwardLanes, int backwardLanes)
    {
        int numNodes = nodePositions.Count;
        Vector3[] verts = new Vector3[numNodes * 2];
        int vertIndex = 0;
        Dictionary<float, List<RoadNode>> roadNodesWithLaneSpaces = new Dictionary<float, List<RoadNode>>();
        List<float> laneSpaces = new List<float>();
        int totalLanes = forwardLanes + backwardLanes;

        for (float i = -(totalLanes / 2f) + 0.5f; i <= (totalLanes / 2f) - 0.5f; i++)
        {
            laneSpaces.Add(i);
        }

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

            Vector2 left = new Vector2(-forward.y, forward.x);
            Vector3 vleft = new Vector3(left.x, 0, left.y);
            foreach (float laneSpace in laneSpaces)
            {
                Vector3 nodeLocation = currentNodeLoc + vleft * RoadGenerator.defaultLaneWidth * laneSpace;
                RoadNode roadNode = CreateRoadNode(Random.value + "", nodeLocation); // TODO CHANGE RANDOM VALUE STRING
                if (!roadNodesWithLaneSpaces.ContainsKey(laneSpace))
                {
                    roadNodesWithLaneSpaces[laneSpace] = new List<RoadNode>();
                }
                roadNodesWithLaneSpaces[laneSpace].Add(roadNode);
            }
        }
        for (int i = 0; i < laneSpaces.Count; i++)
        {
            // Depending on left hand drive skip all forward lanes but reverse backwards lanes
            if((isLeftHandDrive && i >= forwardLanes) || (!isLeftHandDrive && i < forwardLanes))
            {
                continue;
            }
            roadNodesWithLaneSpaces[laneSpaces[i]].Reverse();
        }

        List<RoadWay> roadWays = new List<RoadWay>();
        foreach (KeyValuePair<float, List<RoadNode>> entry in roadNodesWithLaneSpaces)
        {
            RoadNode firstNode = null;
            RoadWay roadWay = CreateRoadWay(way.Name + "_" + entry.Key);
            for (int i = 0; i < entry.Value.Count; i++)
            {
                if (i == 1 && firstNode != null)
                {
                    // Direction from 1st to 2nd node
                    Vector3 relativePos = entry.Value[i].transform.position - firstNode.transform.position;
                    // the second argument, upwards, defaults to Vector3.up
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                    firstNode.transform.rotation = rotation;
                }
                if (i == 0)
                {
                    entry.Value[i].startNode = true;
                    firstNode = entry.Value[i];
                }
                roadWay.nodes.Add(entry.Value[i]);
            }
            roadWays.Add(roadWay);
        }

        return roadWays;

    }

    private RoadWay CreateRoadWay(string name)
    {
        RoadWay roadWay = new GameObject().AddComponent<RoadWay>();
        roadWay.name = string.IsNullOrEmpty(name) ? "Road_Path" : (name + "_Path");
        // Add layer to 'ignore raycasts' to path object
        roadWay.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        return roadWay;
    }

    private RoadNode CreateRoadNode(string id, Vector3 position)
    {
        // Create GameObject for node
        string name = "node" + id;
        GameObject singleNode = new GameObject(name);
        // Add layer to ignore to raycasts to node
        singleNode.layer = LayerMask.NameToLayer("Ignore Raycast");
        // Set position of Node to the vector
        singleNode.transform.position = position;
        // Make node a child of main root
        singleNode.transform.SetParent(roadNodeRootParent.transform, true);
        StoreNodeObjectByNodeId(singleNode, "" + id);
        return singleNode.AddComponent<RoadNode>();
    }

    /// <summary>
    /// Add node to dictionary
    /// </summary>
    /// <param name="singleNode">Node GameObject</param>
    /// <param name="id">node id</param>
    void StoreNodeObjectByNodeId(GameObject singleNode, string id)
    {
        if (!nodeObjectsByNodeId.ContainsKey(id))
            nodeObjectsByNodeId.Add(id, singleNode);
        else
            nodeObjectsByNodeId[id] = singleNode; // overwrite old value
    }

    /// <summary>
    /// Return all nodes in road network
    /// </summary>
    /// <returns>Dictionary {Key: Node (MapXmlWay) ID, Value: Node Game Object}</returns>
    public Dictionary<string, GameObject> GetAllNodesInRoadNetwork()
    {
        return nodeObjectsByNodeId;
    }

    /// <summary>
    /// Make Vehicle child of ways parent GameObject
    /// </summary>
    /// <param name="way">way</param>
    /// <param name="wayObjects">Dictionary linking way to its parent object</param>
    public void SetParent(MapXmlWay way, RoadWay roadWay, Dictionary<MapXmlWay, GameObject> wayObjects)
    {
        GameObject parent;
        if (wayObjects.ContainsKey(way))
        {
            parent = wayObjects[way];
        }
        else
        {
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
        List<RoadNode> nodes = roadWayObject.GetComponent<RoadWay>().nodes;
        Vector3 startNode = nodes[0].transform.position;
        Vector3 endNode = nodes[nodes.Count - 1].transform.position;

        // - Record Start Node 
        if (startNodes.ContainsKey(startNode))
        {
            // Get dictionary {Key: Road_Name, Value: HashSet of roads}
            Dictionary<string, HashSet<GameObject>> roadsByName = startNodes[startNode];

            if (roadsByName.ContainsKey(roadWayObject.name))
            {
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
        if (endNodes.ContainsKey(endNode))
        {
            // Get Set of all vehicle_Path with same end node
            HashSet<GameObject> roads = endNodes[endNode];
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
            if (roadsIndexdByNodes.ContainsKey(node.transform.position))
            {
                roadsIndexdByNodes[node.transform.position].Add(roadWayObject);
            }
            else
            {
                HashSet<GameObject> roads = new HashSet<GameObject>();
                roads.Add(roadWayObject);
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

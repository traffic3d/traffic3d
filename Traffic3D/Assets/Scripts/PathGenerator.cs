using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates paths for vehicle to drive along (using Path component)
/// </summary>
public class PathGenerator : BaseNodeInformant
{
    private OpenStreetMapReader osmMapReader;

    // A dictionary of node positions with a list of ways connected to this node position.
    private Dictionary<Vector3, List<MapXmlWay>> nodeWays;
    // A dictionary of ways with a list of connected node positions.
    private Dictionary<MapXmlWay, List<Vector3>> wayNodes;
    private List<Vector3> connectionPositions;
    private const float maxConnectionDistance = 10f;
    private const float nodeConnectionBackwardsOffset = 2f;
    private const float minDistanceBetweenNodesForMovement = 0.1f;
    private bool isLeftHandDrive;
    private static int nodeNumber = 0;

    public PathGenerator(OpenStreetMapReader osmMapReader, bool isLeftHandDrive)
    {
        // Initialize base class variables
        InitializeVariables();
        this.osmMapReader = osmMapReader;
        this.isLeftHandDrive = isLeftHandDrive;
        connectionPositions = new List<Vector3>();
        nodeWays = new Dictionary<Vector3, List<MapXmlWay>>();
        wayNodes = new Dictionary<MapXmlWay, List<Vector3>>();
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
                CalculateWayNodes(way);
            }
        }
        connectionPositions = nodeWays.Where(e => e.Value.Count > 1).Select(e => e.Key).ToList();
        foreach (KeyValuePair<MapXmlWay, List<Vector3>> entry in wayNodes)
        {
            if (entry.Key.IsRoad)
            {
                // Create new paths
                List<RoadWay> roadWays = CreateRoadWays(entry.Key, entry.Value);

                // Make vehicle path child of way objects' parent
                foreach (RoadWay roadWay in roadWays)
                {
                    SetParent(entry.Key, roadWay, wayObjects);
                }

                Road road = wayObjects[entry.Key].GetComponentInChildren<Road>();
                if (road != null)
                {
                    road.roadWays.AddRange(roadWays);
                    road.numberOfLanes = roadWays.Count;
                }
            }
        }
        AddConnectionsBetweenRoads();
        RenderStopLines();
        RoadNetworkManager.GetInstance().Reload();
        foreach (RoadWay roadWay in RoadNetworkManager.GetInstance().GetWays())
        {
            RecordRoadWayData(roadWay.gameObject);
            createdRoads.Add(roadWay.gameObject);
        }
    }

    public void AddConnectionsBetweenRoads()
    {
        RoadNetworkManager.GetInstance().Reload();
        foreach (Vector3 connectionPosition in connectionPositions)
        {
            Dictionary<RoadNode, float> closeRoadNodesAndDistance = RoadNetworkManager.GetInstance().GetNodes()
                .ToDictionary(r => r, r => Vector3.Distance(connectionPosition, r.transform.position)).OrderBy(e => e.Value)
                .Where(e => e.Value < maxConnectionDistance + (RoadGenerator.defaultLaneWidth * RoadNetworkManager.GetInstance().GetRoadsFromNode(e.Key).Select(r => r.numberOfLanes).DefaultIfEmpty(1).Max()))
                .ToDictionary(e => e.Key, e => e.Value);
            List<List<RoadWay>> roadWaysEvaluated = new List<List<RoadWay>>();
            foreach (KeyValuePair<RoadNode, float> roadNodeDistanceCurrent in closeRoadNodesAndDistance)
            {
                foreach (KeyValuePair<RoadNode, float> roadNodeDistanceCompare in closeRoadNodesAndDistance)
                {
                    if (roadNodeDistanceCurrent.Key == roadNodeDistanceCompare.Key)
                    {
                        continue;
                    }
                    RoadNode currentRoadNode = roadNodeDistanceCurrent.Key;
                    RoadNode compareRoadNode = roadNodeDistanceCompare.Key;
                    List<RoadWay> currentRoadWays = RoadNetworkManager.GetInstance().GetRoadWaysFromNode(currentRoadNode);
                    List<RoadWay> compareRoadWays = RoadNetworkManager.GetInstance().GetRoadWaysFromNode(compareRoadNode);
                    foreach (RoadWay currentRoadWay in currentRoadWays)
                    {
                        foreach (RoadWay compareRoadWay in compareRoadWays)
                        {
                            if (roadWaysEvaluated.Any(l => l.First().Equals(currentRoadWay) && l.Last().Equals(compareRoadWay)))
                            {
                                continue;
                            }
                            roadWaysEvaluated.Add(new List<RoadWay>() { currentRoadWay, compareRoadWay });
                            // If same road (under same parent) then continue.
                            if (currentRoadWay.transform.parent.Equals(compareRoadWay.transform.parent))
                            {
                                continue;
                            }
                            if (currentRoadNode.Equals(currentRoadWay.nodes.First()))
                            {
                                continue;
                            }
                            if (compareRoadNode.Equals(compareRoadWay.nodes.Last()))
                            {
                                continue;
                            }
                            RoadNode connectionCurrentRoadNode = null;
                            RoadNode connectionCompareRoadNode = null;
                            Road currentRoad = RoadNetworkManager.GetInstance().GetRoadFromRoadWay(currentRoadWay);
                            Road compareRoad = RoadNetworkManager.GetInstance().GetRoadFromRoadWay(compareRoadWay);
                            int numberOfLanesCurrentRoad = 1;
                            int numberOfLanesCompareRoad = 1;
                            if (currentRoad != null)
                            {
                                numberOfLanesCurrentRoad = currentRoad.numberOfLanes;
                            }
                            if (compareRoad != null)
                            {
                                numberOfLanesCompareRoad = compareRoad.numberOfLanes;
                            }
                            int indexOfCurrentRoadNode = currentRoadWay.nodes.IndexOf(currentRoadNode);
                            int indexOfCompareRoadNode = compareRoadWay.nodes.IndexOf(compareRoadNode);
                            float distanceFromMainNode = Mathf.Min(numberOfLanesCurrentRoad, numberOfLanesCompareRoad) * RoadGenerator.defaultLaneWidth;
                            // For the current road node, if its not a start node or an end node then create a new node for the connection (middle of a road)
                            if (indexOfCurrentRoadNode != 0 && indexOfCurrentRoadNode != currentRoadWay.nodes.Count - 1)
                            {
                                RoadNode previousNode = currentRoadWay.nodes[indexOfCurrentRoadNode - 1];
                                // If previous node is already at distance required then use that else create a new node.
                                if (Mathf.Abs(Vector3.Distance(currentRoadNode.transform.position, previousNode.transform.position) - distanceFromMainNode) < 1)
                                {
                                    connectionCurrentRoadNode = previousNode;
                                }
                                else
                                {
                                    connectionCurrentRoadNode = InsertPathNodeBetweenDistantNodes(1, distanceFromMainNode, currentRoadWay, currentRoadNode.transform, previousNode, indexOfCurrentRoadNode, true);
                                }
                            }
                            // For the compare road node, if its not a start node or an end node then create a new node for the connection (middle of a road)
                            if (indexOfCompareRoadNode != 0 && indexOfCompareRoadNode != compareRoadWay.nodes.Count - 1)
                            {
                                RoadNode nextNode = compareRoadWay.nodes[indexOfCompareRoadNode + 1];
                                // If previous node is already at distance required then use that else create a new node.
                                if (Mathf.Abs(Vector3.Distance(compareRoadNode.transform.position, nextNode.transform.position) - distanceFromMainNode) < 1)
                                {
                                    connectionCompareRoadNode = nextNode;
                                }
                                else
                                {
                                    connectionCompareRoadNode = InsertPathNodeBetweenDistantNodes(1, distanceFromMainNode, compareRoadWay, compareRoadNode.transform, nextNode, indexOfCompareRoadNode, false);
                                }
                            }
                            if (connectionCurrentRoadNode == null)
                            {
                                connectionCurrentRoadNode = currentRoadNode;
                            }
                            if (connectionCompareRoadNode == null)
                            {
                                connectionCompareRoadNode = compareRoadNode;
                            }
                            RoadWay newRoadWay = CreateRoadWay(currentRoadWay.name + "_and_" + compareRoadWay.name, currentRoadWay.speedLimit);
                            newRoadWay.nodes.Add(connectionCurrentRoadNode);
                            newRoadWay.nodes.Add(connectionCompareRoadNode);
                            newRoadWay.transform.parent = currentRoadWay.transform.parent;
                        }
                    }
                }
            }
        }
    }

    public void RenderStopLines()
    {
        foreach (StopLineRenderer stopLineRenderer in GameObject.FindObjectsOfType<StopLineRenderer>())
        {
            stopLineRenderer.RenderStopLine();
        }
    }

    public List<GameObject> GetDeletedVehiclePaths()
    {
        return deletedVehiclePaths;
    }

    /// <summary>
    /// Calculates the unity positions for each node.
    /// </summary>
    /// <param name="way">Way holding all nodes in the road</param>
    /// <param name="pathName">Name of the new vehicle path</param>
    void CalculateWayNodes(MapXmlWay way)
    {
        List<Vector3> nodePositions = new List<Vector3>();
        for (int i = 0; i < way.NodeIDs.Count; i++)
        {
            MapXmlNode currentNodeLocation = osmMapReader.nodes[way.NodeIDs[i]]; // Current Nodes' Location
            Vector3 vCurrentNodeLocation = osmMapReader.bounds.Centre - currentNodeLocation; // Node vector location
            vCurrentNodeLocation.y = vCurrentNodeLocation.y - 1; // move up along y-axis so node is above road
            vCurrentNodeLocation = vCurrentNodeLocation * (-1f);
            nodePositions.Add(vCurrentNodeLocation);
            if (!nodeWays.ContainsKey(vCurrentNodeLocation))
            {
                nodeWays[vCurrentNodeLocation] = new List<MapXmlWay>();
            }
            nodeWays[vCurrentNodeLocation].Add(way);
        }
        this.wayNodes.Add(way, nodePositions);
    }

    /// <summary>
    /// Creates a Vehicle Path
    /// - Creates a new gameObject with "Path" Component. 
    /// - Populates Path component using nodes found in "Way" 
    /// </summary>
    /// <param name="way">Way holding all nodes in the road</param>
    /// <param name="nodePositions">Unity positions of all nodes</param>
    private List<RoadWay> CreateRoadWays(MapXmlWay way, List<Vector3> nodePositions)
    {
        int forwardLanes = way.ForwardLanes;
        int backwardLanes = way.BackwardLanes;
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

        bool placeStopLine;

        for (int i = 0; i < numNodes; i++)
        {
            // Reset Variables
            placeStopLine = false;
            // For first node & last node:
            // - Check for extra roads connecting and move backwards if connecting to a road.
            // - Place stop lines if needed.
            if ((i == 0 || i == numNodes - 1) && numNodes > 1)
            {
                List<MapXmlWay> waysOnNode = nodeWays[nodePositions[i]];
                if (waysOnNode.Count > 1)
                {
                    int maxLanes = 0;
                    foreach (MapXmlWay wayOnNode in waysOnNode)
                    {
                        if (wayOnNode.Equals(way))
                        {
                            continue;
                        }
                        // Get max lanes for checking if road needs to move backwards
                        if (wayOnNode.ForwardLanes > maxLanes)
                        {
                            maxLanes = wayOnNode.ForwardLanes;
                        }
                        if (wayOnNode.BackwardLanes > maxLanes)
                        {
                            maxLanes = wayOnNode.BackwardLanes;
                        }
                        // Check if able to place stop lines
                        // Two way check (this current road and another with the connection in the middle of the road).
                        if (waysOnNode.Count == 2)
                        {
                            int index = wayNodes[wayOnNode].IndexOf(nodePositions[i]);
                            // If index is in the middle of the road path.
                            if (index > 0 && index < wayNodes[wayOnNode].Count - 1)
                            {
                                placeStopLine = true;
                            }
                        }
                    }
                    float distanceToMoveBack = maxLanes * RoadGenerator.defaultLaneWidth + nodeConnectionBackwardsOffset;
                    nodePositions[i] = MoveStartOrEndNodeTowardsMainPath(nodePositions, nodePositions[i], distanceToMoveBack);
                }
            }
            Vector3 currentNodeLoc = nodePositions[i]; // Next Nodes' Location
            Vector2 forward = Vector2.zero;
            // For all but last node: Get forward between current & next Node
            if (i < numNodes - 1)
            {
                Vector3 nextNodeLoc = nodePositions[i + 1];// Next Nodes' Location
                Vector2 cur = new Vector2(currentNodeLoc.x, currentNodeLoc.z);
                Vector2 next = new Vector2(nextNodeLoc.x, nextNodeLoc.z);
                forward += next - cur;
            }
            // For all but first node: Get forward between current & previous Node
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
                RoadNode roadNode = CreateRoadNode(GenerateRoadNodeName(nodeLocation), nodeLocation);
                if (placeStopLine)
                {
                    roadNode.gameObject.AddComponent<StopLine>();
                    roadNode.gameObject.AddComponent<StopLineRenderer>();
                }
                if (!roadNodesWithLaneSpaces.ContainsKey(laneSpace))
                {
                    roadNodesWithLaneSpaces[laneSpace] = new List<RoadNode>();
                }
                roadNodesWithLaneSpaces[laneSpace].Add(roadNode);
            }
        }
        // Only reverse road ways if its not one way
        if (!way.IsOneWay)
        {
            for (int i = 0; i < laneSpaces.Count; i++)
            {
                // Depending on left hand drive skip all forward lanes but reverse backwards lanes
                if ((isLeftHandDrive && i >= forwardLanes) || (!isLeftHandDrive && i < forwardLanes))
                {
                    continue;
                }
                roadNodesWithLaneSpaces[laneSpaces[i]].Reverse();
            }
        }
        List<RoadWay> roadWays = new List<RoadWay>();
        foreach (KeyValuePair<float, List<RoadNode>> entry in roadNodesWithLaneSpaces)
        {
            RoadNode firstNode = null;
            RoadWay roadWay = CreateRoadWay(way.Name + "_" + entry.Key, RoadSpeedLimit.GetSpeedLimitFromOSM(way.RoadType));
            for (int i = 0; i < entry.Value.Count; i++)
            {
                // Rotate towards the next node.
                if (i < entry.Value.Count - 1)
                {
                    entry.Value[i].transform.LookAt(entry.Value[i + 1].transform);
                }
                else if (i == entry.Value.Count - 1 && i != 0)
                {
                    // Look away from the previous node.
                    entry.Value[i].transform.rotation = Quaternion.LookRotation(entry.Value[i].transform.position - entry.Value[i - 1].transform.position);
                }
                // Add specific settings for the first node.
                if (i == 0)
                {
                    entry.Value[i].startNode = true;
                    firstNode = entry.Value[i];
                    // Check if first node has stop line and remove.
                    StopLine stopLine = firstNode.gameObject.GetComponent<StopLine>();
                    if (stopLine != null)
                    {
                        GameObject.DestroyImmediate(stopLine);
                        GameObject.DestroyImmediate(firstNode.gameObject.GetComponent<StopLineRenderer>());
                    }
                }
                roadWay.nodes.Add(entry.Value[i]);
            }
            roadWays.Add(roadWay);
        }

        return roadWays;

    }

    private RoadWay CreateRoadWay(string name, int speedLimit)
    {
        RoadWay roadWay = new GameObject().AddComponent<RoadWay>();
        roadWay.speedLimit = speedLimit;
        roadWay.name = string.IsNullOrEmpty(name) ? "Road_Path" : (name + "_Path");
        // Add layer to 'ignore raycasts' to path object
        roadWay.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        return roadWay;
    }

    private RoadNode CreateRoadNode(string id, Vector3 position)
    {
        // Create GameObject for node
        GameObject singleNode = new GameObject(id);
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

    private Vector3 MoveStartOrEndNodeTowardsMainPath(List<Vector3> nodePath, Vector3 nodeToMove, float distanceToMove)
    {
        if (nodePath.Count <= 1)
        {
            Debug.Log("Unable to move node as path only has 1 node or less. Node: " + nodeToMove.ToString());
            return nodeToMove;
        }
        Vector3 target;
        if (nodePath[0].Equals(nodeToMove))
        {
            target = nodePath[1];
        }
        else if (nodePath.Last().Equals(nodeToMove))
        {
            target = nodePath[nodePath.Count - 2];
        }
        else
        {
            Debug.Log("Unable to move node, must be start or end of path. Node: " + nodeToMove.ToString());
            return nodeToMove;
        }
        float distanceToTargetNode = Vector3.Distance(nodeToMove, target);
        if (distanceToMove >= distanceToTargetNode - minDistanceBetweenNodesForMovement)
        {
            distanceToMove = distanceToTargetNode - minDistanceBetweenNodesForMovement;
        }
        return Vector3.MoveTowards(nodeToMove, target, distanceToMove);
    }

    public static string GenerateRoadNodeName(Vector3 nodeLocation)
    {
        string nodeName = "node_" + nodeNumber + "_" + nodeLocation.x + "," + nodeLocation.y + "," + nodeLocation.z;
        nodeNumber++;
        return nodeName;
    }

}

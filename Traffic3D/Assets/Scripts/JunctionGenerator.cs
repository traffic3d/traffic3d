using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static JunctionState;

/// <summary>
/// Generates Junctions in a scene and spawns trafficlights
/// Currently, junctions are only generated where 2 roads cross i.e Cross-junction (+)
/// If more than two roads make up the the Cross-junction, it will merge "Entry->Exit" roads together so there's only 2 roads
/// 
/// NOTE: Class should be extended to create junctions at T-junctions. 
/// However, this requires the 'Path' script to support nodes connecting to more than one other node.
/// </summary>
public class JunctionGenerator : BaseNodeInformant
{
    TrafficLightGenerator trafficLightGenerator;
    static ulong trafficLightId = 1;

    static int junctionId = 1;
    GameObject rootParent = new GameObject("Junctions"); //store all junctions under same gameobject to keep things tidy
    int numCreatedJunctions = 0;
    private const float defaultJunctionWidth = 0.25f;
    private const float junctionObjectPadding = 0.15f;
    private const float maxNodeDistanceApart = 10f;
    private const float newNodeDistanceApart = 12f;
    private const float maxDensityNodeDistanceApartPadding = 8f;
    private const float newDensityNodeDistanceApartPadding = 3f;
    private readonly Vector3 defaultDensityNodeSize = new Vector3(3f, 5f, 3f);

    public void GenerateJunctions(TrafficLightGenerator trafficLightGenerator)
    {
        this.trafficLightGenerator = trafficLightGenerator;
        CalculateMidConnectedNodes();
        CreateJunctions();
    }

    //Find which (non-End/Start) nodes are used by multiple roads
    void CalculateMidConnectedNodes()
    {
        HashSet<Vector3> checkedNodes = new HashSet<Vector3>();
        midConnectedNodes = new Dictionary<Vector3, HashSet<GameObject>>();
        RoadNetworkManager.GetInstance().Reload();

        //loop through all roads
        foreach (Road road in RoadNetworkManager.GetInstance().GetRoads())
        {
            //loop through all roadways
            foreach (RoadWay roadWay in road.roadWays)
            {
                //loop through all nodes in Path
                foreach (RoadNode node in roadWay.nodes)
                {
                    Vector3 nodePos = node.transform.position;

                    //if not start/end node
                    if (!endNodes.ContainsKey(nodePos) && !startNodes.ContainsKey(nodePos))
                    {
                        //if already checked, implies another non-start/non-end road intersects this node => mid_connected_node (cross-junction)
                        if (checkedNodes.Contains(nodePos))
                        {
                            if (!midConnectedNodes.ContainsKey(nodePos))
                            {
                                //flag as mid_connection node (Key: {node} Value: {All road passing through node})
                                midConnectedNodes.Add(nodePos, roadsIndexdByNodes[nodePos]);
                            }
                            else
                            {
                                //update value
                                midConnectedNodes[nodePos] = roadsIndexdByNodes[nodePos];
                            }
                        }
                        else
                        {
                            //flag as checked
                            checkedNodes.Add(nodePos);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates junctions at mid nodes, where two roads meet (i.e Cross-Junctions)
    /// Calls methods to merge roads if 'one road ends' where 'another road begins'.
    /// </summary>
    void CreateJunctions()
    {
        //prevent duplicates
        HashSet<Vector3> used = new HashSet<Vector3>();

        //original collections modified during loops
        var tempEndNodes = new Dictionary<Vector3, HashSet<GameObject>>(endNodes);
        var tempStartNodes = new Dictionary<Vector3, Dictionary<string, HashSet<GameObject>>>(startNodes);

        //Create junction at each end node
        foreach (Vector3 endNode in tempEndNodes.Keys)
        {
            if (!used.Contains(endNode))
            {
                //Attempt to merge roads at current node
                MergeJunctionNodes(endNode);

                //Mark as used
                used.Add(endNode);
            }
        }

        //Create junction at each start node
        foreach (Vector3 startNode in tempStartNodes.Keys)
        {
            if (!used.Contains(startNode))
            {
                //Attempt to merge roads at current node
                MergeJunctionNodes(startNode);

                //Mark as used
                used.Add(startNode);
            }
        }

        CalculateMidConnectedNodes(); //check & update, if the number of mid nodes (i.e Cross-junctions) has changed

        //Create junction at each mid node (point where roads meet)
        foreach (Vector3 midNode in midConnectedNodes.Keys)
        {
            GameObject junctionObject = CreateJunctionGameObjectAtNode(midNode);

            //Spawn TrafficLights
            List<GameObject> trafficLights = AddTrafficLightsToJunction(junctionObject, midNode);

            if (trafficLights == null)
                GameObject.DestroyImmediate(junctionObject);
            else
            {
                AddJunctionStates(junctionObject, trafficLights);
                AddDensityMeasurePoint(junctionObject, midConnectedNodes[midNode]);
                numCreatedJunctions++;
            }
        }

    }

    /// <summary>
    /// Creates junction object with scripts and name
    /// </summary>
    /// <param name="node">Vector3 position of junction</param>
    /// <returns>Junction Object with scripts and camera</returns>
    GameObject CreateJunctionGameObjectAtNode(Vector3 node)
    {
        GameObject parent = new GameObject("Junction");
        parent.transform.parent = rootParent.transform;

        HashSet<GameObject> roads = roadsIndexdByNodes[node];

        string name = "Junction - ";
        foreach (var road in roads)
        {
            if (road != null)
                name = name + road.name + " - ";
        }
        parent.name = name;

        GameObject junctionObject = CreateJunctionGameObject(node, parent);

        AddScriptsToJunction(junctionObject);
        AddCameraToJunction(junctionObject, parent);

        //Add Ignore Raycasts layer
        junctionObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        return junctionObject;
    }


    /// <summary>
    /// Create a plane object and position at current node & scale to road size
    /// </summary>
    /// <param name="junctionNode">targetNode Node to spawn plane</param>
    /// <param name="parent">parent gameObject</param>
    /// <returns>Plane Object</returns>
    GameObject CreateJunctionGameObject(Vector3 junctionNode, GameObject parent)
    {
        //create plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Junction Plane Object";
        plane.transform.position = new Vector3(junctionNode.x, 0f, junctionNode.z); //spawn just below road mesh
        plane.transform.parent = parent.transform;

        //Scale
        int numLanes = 1;
        float planeHeight = 1f;

        try
        {
            //get largest number of lanes for roads connected to junction
            HashSet<GameObject> vehiclePaths = roadsIndexdByNodes[junctionNode];
            foreach (GameObject vehiclePath in vehiclePaths)
            {
                int currNumLanes = vehiclePath.transform.parent.GetComponentInChildren<Road>().numberOfLanes;
                if (currNumLanes > numLanes)
                    numLanes = currNumLanes;
            }
        }
        catch { }

        float scaledWidth = defaultJunctionWidth * numLanes;
        if (scaledWidth <= 0)
            scaledWidth = defaultJunctionWidth;

        plane.transform.localScale = new Vector3(junctionObjectPadding + scaledWidth, planeHeight, junctionObjectPadding + scaledWidth);

        //Transform
        Vector3 pos = plane.transform.position;
        pos.y = 0;
        plane.transform.position = pos;

        return plane;
    }

    void AddScriptsToJunction(GameObject junctionObject)
    {
        //Box Collider
        junctionObject.AddComponent<BoxCollider>();
        BoxCollider col = junctionObject.GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(col.size.x, 8f, col.size.z);

        //JunctionTrigger
        junctionObject.AddComponent<JunctionTrigger>();

        //Junction
        junctionObject.AddComponent<Junction>();
        Junction junction = junctionObject.GetComponent<Junction>();
        junction.junctionId = "" + junctionId;

        //Mesh Renderer
        junctionObject.GetComponent<MeshRenderer>().enabled = false;

        //Mesh collider
        junctionObject.GetComponent<MeshCollider>().enabled = false;

        junctionId++;
    }

    void AddCameraToJunction(GameObject junctionObject, GameObject parent)
    {
        GameObject camera = new GameObject("Camera");
        camera.transform.parent = parent.transform;
        camera.AddComponent<Camera>();
        camera.AddComponent<FlareLayer>();
        camera.AddComponent<AudioListener>();
        camera.AddComponent<CameraManager>();
        camera.transform.position = new Vector3(junctionObject.transform.position.x, 8, junctionObject.transform.position.z - 15f);
        camera.transform.LookAt(junctionObject.transform.position);

        junctionObject.GetComponent<Junction>().junctionCamera = camera.GetComponent<Camera>();
    }

    /// <summary>
    /// Randomly links Entry->Exit (Start/End) nodes in a junction.
    /// Requires number of start and end nodes to be the same => Roads merged using 1-1 relationship. 
    /// </summary>
    /// <param name="junctionNode">the junction node whose roads are being merged</param>
    void MergeJunctionNodes(Vector3 junctionNode)
    {
        if (GetNumStartNodesAtTarget(junctionNode) == GetNumEndNodesAtTarget(junctionNode))
        {
            //Check if juction has start & end nodes
            if (GetNumStartNodesAtTarget(junctionNode) > 0 && GetNumEndNodesAtTarget(junctionNode) > 0)
            {
                //holds which start node was linked to which end node
                Dictionary<GameObject, GameObject> linkedStartEndNodes = new Dictionary<GameObject, GameObject>();

                //Create list from end nodes
                List<GameObject> lsEndNodes = new List<GameObject>(endNodes[junctionNode]);
                Dictionary<string, HashSet<GameObject>> startNodesDic = startNodes[junctionNode];

                //-- Link Start and end nodes into pairs

                //iterate through all sets of roads attached to current junction node (Loops aprox. 1-3 times)
                foreach (KeyValuePair<string, HashSet<GameObject>> StartNodesAtJunction in startNodesDic)
                {
                    //loop through all values in set (Loops aprox 1-2 times => O(1))
                    foreach (GameObject startNode in StartNodesAtJunction.Value)
                    {
                        //link a "Start-Node Road" to first "End-Node Road"
                        linkedStartEndNodes.Add(startNode, lsEndNodes[0]);
                        lsEndNodes.RemoveAt(0);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Evaluates the number of trafficlights needed at a junction and manages creation of trafficlights
    /// 
    /// NOTE: currently only spawns trafficlights at junctions with 2 roads.
    /// </summary>
    /// <param name="junctionObject">Junction Gameobject with Junction Scripts</param>
    /// <param name="midNode">midConnectedNodes.Key</param>
    /// <returns>List of TrafficLight objects OR null</returns>
    List<GameObject> AddTrafficLightsToJunction(GameObject junctionObject, Vector3 midNode)
    {
        //Get all roads traveling through junction node
        HashSet<GameObject> roads = midConnectedNodes[midNode];

        //Currently only supporting junctions with two lanes and two trafficLights
        if (roads.Count == 2)
        {
            return AddTwoLaneTrafficLight(junctionObject, midNode);
        }

        return null;
    }

    /// <summary>
    /// Adds TrafficLights to a Junction with 2 one-way roads
    /// </summary>
    /// <param name="junctionObject">Junction GameObject with junction scripts</param>
    /// <param name="midNode">midConnectedNodes.Key</param>
    /// <returns>List of created trafficLights for junction</returns>
    List<GameObject> AddTwoLaneTrafficLight(GameObject junctionObject, Vector3 midNode)
    {
        List<GameObject> createdTrafficLights = new List<GameObject>(2);

        //Get all roads traveling through junction node
        HashSet<GameObject> vehiclePaths = midConnectedNodes[midNode];

        //trafficLight parent
        GameObject trafficLightParent = new GameObject("Junction__TrafficLights");
        int trafficLightCount = 0;

        if (junctionObject.transform.parent)
            trafficLightParent.transform.parent = junctionObject.transform.parent;
        else
            trafficLightParent.transform.parent = junctionObject.transform;

        // For each Path connected to Junction
        foreach (GameObject vehiclePath in vehiclePaths)
        {
            int numLanes = 1;

            // attempt to update number of road lanes
            try
            {
                numLanes = vehiclePath.transform.parent.GetComponentInChildren<Road>().numberOfLanes;
            }
            catch { }

            trafficLightCount++;

            RoadWay roadWay = vehiclePath.GetComponent<RoadWay>();
            int indexOfJunctionNode = 0;

            indexOfJunctionNode = GetIndexOfTransformInPath(roadWay, junctionObject.transform);

            RoadNode NodeBeforeJunction = roadWay.nodes[indexOfJunctionNode - 1];
            RoadNode spawnTrafficLightPosition = roadWay.nodes[indexOfJunctionNode - 1];

            GameObject tmp = new GameObject();
            tmp.transform.position = midNode;

            // If nodes far apart, create new node and insert into path
            RoadNode newNode = InsertPathNodeBetweenDistantNodes(newNodeDistanceApart, maxNodeDistanceApart, roadWay, tmp.transform, NodeBeforeJunction, indexOfJunctionNode, true);

            GameObject.DestroyImmediate(tmp);
            if (newNode != null)
            {
                spawnTrafficLightPosition = newNode;
            }
            else
            {
                if (indexOfJunctionNode > 1)
                {
                    NodeBeforeJunction = roadWay.nodes[indexOfJunctionNode - 2];
                }
            }

            GameObject trafficLight = trafficLightGenerator.CreateTrafficLightModel(trafficLightParent, spawnTrafficLightPosition.transform.position, trafficLightId, NodeBeforeJunction.transform.position, trafficLightCount, numLanes);
            createdTrafficLights.Add(trafficLight);

            // Add Stop Node To TrafficLight
            trafficLight.GetComponent<TrafficLight>().stopNodes.Add(spawnTrafficLightPosition);

            trafficLightId++;

            // Add node before trafficLight
            InsertPathNodeBetweenDistantNodes(20f, 18f, roadWay, spawnTrafficLightPosition.transform, roadWay.nodes[indexOfJunctionNode - 1], indexOfJunctionNode, true);
        }

        return createdTrafficLights;
    }



    /// <summary>
    /// Add junction states to junction 
    /// </summary>
    /// <param name="junction">Junction GameObject with Junction Scripts</param>
    /// <param name="trafficLights">trafficlights in junction</param>
    void AddJunctionStates(GameObject junction, List<GameObject> trafficLights)
    {
        //create junction state
        for (int i = 1; i <= trafficLights.Count; i++)
        {
            //initializse new State Object
            GameObject state = new GameObject("State" + i);
            state.AddComponent<JunctionState>();
            state.transform.parent = junction.transform;

            //Get States
            JunctionState junctionState = state.GetComponent<JunctionState>();
            junctionState.stateNumber = i;
            junctionState.trafficLightStates = new TrafficLightState[trafficLights.Count];

            //Add each trafficLightState to the junctionState
            for (int j = 1; j <= trafficLights.Count; j++)
            {
                TrafficLightState tls;

                //Make all states Red, Except 1
                if (i == j)
                {
                    tls = new TrafficLightState("" + trafficLights[j - 1].GetComponent<TrafficLight>().trafficLightId, TrafficLight.LightColour.GREEN);
                }
                else
                {
                    tls = new TrafficLightState("" + trafficLights[j - 1].GetComponent<TrafficLight>().trafficLightId, TrafficLight.LightColour.RED);
                }

                junctionState.trafficLightStates.SetValue(tls, j - 1);
            }
        }
    }

    /// <summary>
    /// Add Traffic3D required script (DensityMeasurePoint) to node right after junction
    /// </summary>
    /// <param name="junction">junction Gameobject</param>
    /// <param name="vehiclePaths">list of all vehicle paths travelling through junction</param>
    void AddDensityMeasurePoint(GameObject junction, HashSet<GameObject> vehiclePaths)
    {
        //For each Path connected to Junction - 2 loops
        foreach (GameObject vehiclePath in vehiclePaths)
        {
            RoadWay roadWay = vehiclePath.GetComponent<RoadWay>();

            int indexOfJunctionNode = GetIndexOfTransformInPath(roadWay, junction.transform);

            if (indexOfJunctionNode == -1)
            {
                Debug.Log("ERROR ROAD NOT FOUND. No Density Measurement Point for ->" + vehiclePath.name + " -> Junction ID: " + junction.GetComponent<Junction>().junctionId);
                continue;
            }

            //Get radious of junction
            float junctionRadius = junction.GetComponent<BoxCollider>().bounds.extents.magnitude;

            RoadNode NodeAfterJunction = roadWay.nodes[indexOfJunctionNode + 1];

            //If nodes far apart, create new node and insert into path
            RoadNode densityNode = InsertPathNodeBetweenDistantNodes(junctionRadius + maxDensityNodeDistanceApartPadding, junctionRadius + newDensityNodeDistanceApartPadding, roadWay, junction.transform, NodeAfterJunction, indexOfJunctionNode, false);

            //null => NodeAfterJunction is close enough to be density measure point 
            if (densityNode != null)
                densityNode.transform.position = new Vector3(densityNode.transform.position.x, 1, densityNode.transform.position.z);
            else
                densityNode = NodeAfterJunction;

            densityNode.gameObject.AddComponent<BoxCollider>();
            BoxCollider col = densityNode.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = defaultDensityNodeSize;
            densityNode.gameObject.AddComponent<DensityMeasurePoint>();

        }

    }

    /// <returns>Index of node in the Path</returns>
    int GetIndexOfTransformInPath(RoadWay roadWay, Transform pathNode)
    {
        int pathIndex = 0;

        //get index of junction node in path.nodes
        foreach (RoadNode roadNode in roadWay.nodes)
        {
            //check if transforms have same X & Y positions
            if ((roadNode.transform.position.x == pathNode.position.x) && (roadNode.transform.position.z == pathNode.position.z))
            {
                return pathIndex;
            }
            pathIndex++;
        }

        return -1;
    }


    /// <summary>
    /// Get number of start nodes connected to target
    /// </summary>
    /// <param name="targetNodePos"></param>
    /// <returns>number of start nodes connected to target</returns>
    int GetNumStartNodesAtTarget(Vector3 targetNodePos)
    {
        int numStartNodes = 0;

        if (!startNodes.ContainsKey(targetNodePos))
            return 0;

        //Get dictionary of all roads starting at current node
        Dictionary<string, HashSet<GameObject>> startNodeDic = startNodes[targetNodePos];

        //Loop through all nodes connected to current node (Typically 1-3 loops => O(1) )
        foreach (KeyValuePair<string, HashSet<GameObject>> item in startNodeDic)
        {
            //Count total number of roads in set
            numStartNodes += item.Value.Count;
        }

        return numStartNodes;
    }

    /// <summary>
    /// Get number of end nodes connected to target
    /// </summary>
    /// <param name="targetNodePos"></param>
    /// <returns>number of end nodes</returns>
    int GetNumEndNodesAtTarget(Vector3 targetNodePos)
    {
        int numEndNodes = 0;

        if (!endNodes.ContainsKey(targetNodePos))
            return 0;

        //Get dictionary of all roads starting at current node
        HashSet<GameObject> endNodesSet = endNodes[targetNodePos];

        //Count total number of roads in set
        numEndNodes += endNodesSet.Count;

        return numEndNodes;
    }

    public int GetNumCreatedJunctions()
    {
        return numCreatedJunctions;
    }
}

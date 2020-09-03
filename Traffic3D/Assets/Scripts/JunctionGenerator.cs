using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static JunctionState;

public class JunctionGenerator : BaseNodeInformant
{

    TrafficLightGenerator trafficLightGenerator;
    static ulong trafficLightId = 1;

    static int junctionId = 1;
    GameObject rootParent = new GameObject("Junctions"); //store all junctions under same gameobject to keep things tidy


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

        //loop through all roads
        foreach (GameObject road in createdRoads)
        {   
            //loop through all nodes in rode Path
            foreach (Transform node in road.GetComponent<Path>().nodes)
            {
                Vector3 nodePos = node.position;

                //if not start/end node
                if (!endNodes.ContainsKey(nodePos) && !startNodes.ContainsKey(nodePos))
                {

                    //if already checked, implies another non-stat/non-end road intersects this node => mid_connected_node
                    if (checkedNodes.Contains(nodePos))
                    {
                        if (!midConnectedNodes.ContainsKey(nodePos))
                        {
                            //flag as mid_connection node (e.g node depicting T-Junction) (Key: {node} Value: {All road passing through node})
                            midConnectedNodes.Add(nodePos, roadsIndexdByNodes[nodePos]);
                            
                        }
                        else
                        {//update value
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

    /// <summary>
    /// create junction at mid nodes where roads cross eachother
    /// </summary>
    void CreateJunctions()
    {
        //prevent duplicates
        HashSet<Vector3> used = new HashSet<Vector3>();

        //Create junction at each end node
        foreach (Vector3 endNode in endNodes.Keys)
        {

            /*
             * If (haven't already created junction) && (Junction connects to other roads) && (Number of end_nodes and start_nodes is the same )
             * Effectively, there should be a 1-1 relationship between roads in a junction so e.g T-Junctions aren't supported
            */
            if (!used.Contains(endNode))
            {
                //check if junction and if number of start and end nodes  is equal (1-1 relationship)
             //   GameObject junctionObject = CreateJunctionGameObjectAtNode(endNode);

                //Merge Junctions
                MergeJunctionNodes(endNode);

                //Spawn TrafficLights
                //AddTrafficLightsToJunction(junctionObject, node);

                //Mark as used
                used.Add(endNode);
                
            }
        }

        //Create junction at each start node
        foreach (Vector3 startNode in startNodes.Keys)
        {
            if (!used.Contains(startNode))
            {
                //Merge Junctions
                MergeJunctionNodes(startNode);

                //Spawn TrafficLights
                //AddTrafficLightsToJunction(junctionObject, node);

                //Mark as used
                used.Add(startNode);
                
            }
        }

        CalculateMidConnectedNodes();

        //Create junction at each mid node
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

                AddDensityMeasurePoint(junctionObject, midConnectedNodes[midNode] );
            }
                
        }

    }

    /// <summary>
    /// Creates junction object at node
    /// </summary>
    /// <param name="node">Vector3 position of junction</param>
    /// <returns></returns>
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

       return junctionObject;
    }


    /// <summary>
    /// Create plane object
    /// </summary>
    /// <param name="junctionNode">targetNode Node to spawn plane</param>
    /// <param name="parent">parent gameObject</param>
    /// <returns>Plane Object</returns>
    GameObject CreateJunctionGameObject(Vector3 junctionNode, GameObject parent)
    {

        //create plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Junction Plane Object";
        plane.transform.position = new Vector3(junctionNode.x,0f,junctionNode.z); //spawn just below road mesh
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
                int currNumLanes = vehiclePath.transform.parent.GetComponentInChildren<RoadMeshUpdater>().numLanes;
                if (currNumLanes > numLanes)
                    numLanes = currNumLanes;
            }
        }
        catch { }

        float scaledWidth= 0.25f * numLanes;
        if (scaledWidth <= 0)
            scaledWidth = 0.25f;

        
        plane.transform.localScale = new Vector3(0.15f+scaledWidth, planeHeight, 0.15f + scaledWidth);

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
        col.size = new Vector3(col.size.x, 5, col.size.z);

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

    //
    /// <summary>
    /// Randomly links nodes in a junction.
    /// Requires number of start and end nodes to be the same. => Roads merged using 1-1 relationship 
    /// </summary>
    /// <param name="junctionNode">the junction node whose roads are being merged</param>
    void MergeJunctionNodes(Vector3 junctionNode)
    {
        if (GetNumStartNodes(junctionNode) == GetNumEndNodes(junctionNode))
        {

            //Check if juction has start & end nodes
            if (GetNumStartNodes(junctionNode) > 0 && GetNumEndNodes(junctionNode) > 0)
            {

                //"linkedStartEndNodes" must be a "new" dictionary as values of dictionary will be modified during iteration
                Dictionary<GameObject, GameObject> linkedStartEndNodes = new Dictionary<GameObject, GameObject>();


                //Create list from end nodes
                List<GameObject> lsEndNodes = new List<GameObject>(endNodes[junctionNode]);
                Dictionary<string, HashSet<GameObject>> startNodesDic = startNodes[junctionNode];

                //-- Link Start and end nodes into pairs

                //iterate through all sets of roads attached to current junction node (Loops aprox. 1-3 times => O(1) )
                foreach (KeyValuePair<string, HashSet<GameObject>> startNodeSimularNodesDic in startNodesDic)
                {

                    //loop through all values in set (Loops aprox 1-2 times => O(1))
                    foreach (GameObject road in startNodeSimularNodesDic.Value)
                    {
                        //randomly link a "Start-Node Road" to an "End-Node Road"
                        linkedStartEndNodes.Add(road, lsEndNodes[0]);
                        lsEndNodes.RemoveAt(0);
                        
                    }

                }

                //-- Loop through and merge pair
                foreach (KeyValuePair<GameObject, GameObject> nodePairs in linkedStartEndNodes)
                {
                    MergeTwoRoads(nodePairs.Value, nodePairs.Key);
                }


            }
        }

    }

    /// <summary>
    /// Evaluates the number of trafficlights needed at a junction and manages creation of trafficlights
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
    /// Adds TrafficLights to a Junction with 2 roads, one-way roads
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


        //For each Path connected to Junction
        foreach (GameObject vehiclePath in vehiclePaths)
        {
            int numLanes = 1;
            try
            {
                RoadMeshUpdater roadMeshUpdater = vehiclePath.transform.parent.GetComponentInChildren<RoadMeshUpdater>();
                numLanes = roadMeshUpdater.numLanes;
            }
            catch { }

            trafficLightCount++;

            Path path = vehiclePath.GetComponent<Path>();

            int indexOfJunctionNode = 0;

            indexOfJunctionNode = GetIndexOfTransformInPath(path, junctionObject.transform);

            Transform NodeBeforeJunction = path.nodes[indexOfJunctionNode - 1];

            Transform spawnTrafficLightPosition = path.nodes[indexOfJunctionNode - 1];

            GameObject tmp = new GameObject();
            tmp.transform.position = midNode;

            //If nodes far apart, create new node and insert into path
            GameObject newNode = InsertPathNodeBetweenDistantNodes(12f, 10f, path, tmp.transform, NodeBeforeJunction, indexOfJunctionNode, true);

            GameObject.DestroyImmediate(tmp);
            if (newNode != null)
            {
                spawnTrafficLightPosition = newNode.transform;
            }
            else
            {
                NodeBeforeJunction = path.nodes[indexOfJunctionNode - 2];
            }
            
            GameObject trafficLight = trafficLightGenerator.CreateTrafficLightModel(trafficLightParent, spawnTrafficLightPosition.position, trafficLightId, NodeBeforeJunction.position, trafficLightCount, numLanes);
            createdTrafficLights.Add(trafficLight);

            //Add Stop Node To TrafficLight
            trafficLight.GetComponent<TrafficLight>().stopNodes.Add(spawnTrafficLightPosition);

            trafficLightId++;

            //Add node before trafficLight
            InsertPathNodeBetweenDistantNodes(20f, 18f, path, spawnTrafficLightPosition, path.nodes[indexOfJunctionNode - 1], indexOfJunctionNode, true);
        }

        return createdTrafficLights;
    }



    /// <summary>
    /// Add junction state objects
    /// </summary>
    /// <param name="junction">game object or parent junction holding states</param>
    /// <param name="trafficLights">trafficlights in junction</param>
    void AddJunctionStates(GameObject junction, List<GameObject> trafficLights)
    {
        //Typically 2-4 loops: O(1) 
        for (int i=1; i<= trafficLights.Count;i++)
        {

            //initializse new State Object
            GameObject state = new GameObject("State" + i);
            state.AddComponent<JunctionState>();
            state.transform.parent = junction.transform;
            

            //Get States
            JunctionState junctionState = state.GetComponent<JunctionState>();
            junctionState.stateNumber = i;
            junctionState.trafficLightStates = new TrafficLightState[trafficLights.Count];


            //Add each trafficLightState to junctionState
            for (int j= 1; j <= trafficLights.Count; j++)
            {
                TrafficLightState tls;

                //Make all states Red, Except 1
                if (i == j)
                {
                    tls = new TrafficLightState("" + trafficLights[j-1].GetComponent<TrafficLight>().trafficLightId, TrafficLight.LightColour.GREEN);
                }
                else
                {
                    tls = new TrafficLightState("" + trafficLights[j-1].GetComponent<TrafficLight>().trafficLightId, TrafficLight.LightColour.RED);
                }

                junctionState.trafficLightStates.SetValue(tls, j-1);
            }

        }

      
    }

    /// <summary>
    /// add Traffic3D required script to junction node
    /// </summary>
    /// <param name="junction">junction Gameobject</param>
    /// <param name="vehiclePaths">list of vehilce paths travelling through junction</param>
    void AddDensityMeasurePoint(GameObject junction, HashSet<GameObject> vehiclePaths)
    {
        //For each Path connected to Junction - 2 loops
        foreach (GameObject vehiclePath in vehiclePaths)
        {

            Path path = vehiclePath.GetComponent<Path>();

            int indexOfJunctionNode = GetIndexOfTransformInPath(path, junction.transform);

            if (indexOfJunctionNode == -1)
            {
                Debug.Log("ERROR ROAD NOT FOUND. No Density Measurement Point for ->" + vehiclePath.name +" -> Junction ID: "+junction.GetComponent<Junction>().junctionId);
                continue;
            }

            //Get radious of junction
            float radius = junction.GetComponent<MeshRenderer>().bounds.extents.magnitude;

            Transform NodeAfterJunction = path.nodes[indexOfJunctionNode+1];

            //If nodes far apart, create new node and insert into path
            GameObject densityNode = InsertPathNodeBetweenDistantNodes(radius+8, radius, path, junction.transform, NodeAfterJunction, indexOfJunctionNode, false);

            //null => NodeAfterJunction is close enough to be density measure point 
            if (densityNode != null)
                densityNode.transform.position = new Vector3(densityNode.transform.position.x, 1, densityNode.transform.position.z);
            else
                densityNode = NodeAfterJunction.gameObject;

            densityNode.AddComponent<BoxCollider>();
            densityNode.GetComponent<BoxCollider>().isTrigger = true;
            densityNode.AddComponent<DensityMeasurePoint>();

        }
           
    }


    int GetIndexOfTransformInPath(Path path, Transform pathNode)
    {
        int pathIndex = 0;

        //get index of junction node in path.nodes
        foreach (Transform t in path.nodes)
        {
            //check if transforms have same X & Y positions
            if ((t.transform.position.x == pathNode.position.x) && (t.transform.position.z == pathNode.position.z))
            {
                return pathIndex;
            }
            pathIndex++;
        }

        return -1;
    }


    /// <summary>
    /// Get number of start nodes connect to target
    /// </summary>
    /// <param name="targetNodePos"></param>
    /// <returns>number of start nodes connected to target</returns>
    int GetNumStartNodes(Vector3 targetNodePos)
    {
        int numStartNodes = 0;

        if (!startNodes.ContainsKey(targetNodePos))
            return 0;

        //count number of node types connected to current road

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
    int GetNumEndNodes(Vector3 targetNodePos)
    {
        int numEndNodes = 0;

        if (!endNodes.ContainsKey(targetNodePos))
            return 0;

        //count number of node types connected to current road

        //Get dictionary of all roads starting at current node
        HashSet<GameObject> endNodesSet = endNodes[targetNodePos];
            
        //Count total number of roads in set
        numEndNodes += endNodesSet.Count;
            

        return numEndNodes;
    }
}

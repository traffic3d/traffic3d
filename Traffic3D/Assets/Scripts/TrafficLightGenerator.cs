
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightGenerator
{
    private MapReader map;
    private GameObject trafficLight_model;
    Dictionary<ulong, GameObject> trafficLights;
    int trafficLightCount = 0;

    //unique ID for each trafficlight in simulation
    private static int trafficLightId = 1;

    // Start is called before the first frame update
    public TrafficLightGenerator(MapReader mapReader)
    {
        map = mapReader;
        trafficLight_model = Resources.Load("Models/TrafficLight_small") as GameObject;
        trafficLights = new Dictionary<ulong, GameObject>();

        GameObject tlm = new GameObject("TrafficLightManager");
        tlm.AddComponent<TrafficLightManager>();
    }

    //Create TrafficLight For every MapXmlNode marked as "hasTrafficLight"
    //return Dictionary<ulong, GameObject> {NodeID, TrafficLight}
    public Dictionary<ulong, GameObject> AddTrafficLightsByWay(Dictionary<MapXmlWay,GameObject> parentObjectsForWays)
    {

        // Iterate through each way...
        foreach (var way in map.ways)
        {
            //if way is a road...
            if (way.IsRoad)
            {
                CreateTrafficLight(way, way.Name, way.Lanes, parentObjectsForWays);
            }
        }

        return trafficLights;

    }
    

    void CreateTrafficLight(MapXmlWay way, string pathName, int numLanes, Dictionary<MapXmlWay, GameObject> parentObjectsForWays)
    {
       
        //Holds all trafficlights for current way
        GameObject parentObject = new GameObject();

        trafficLightCount = 0;

        Vector3 origin = GetCentre(way);

        //postion go in road centre
        parentObject.transform.position = origin - map.bounds.Centre;
        parentObject.name = way.Name + "_TrafficLights";

        bool hasTrafficlights = false;

        //(Dont add trafficLight to first node)
        for (int i = 1; i < way.NodeIDs.Count; i++)
        {
            ulong nodeID = way.NodeIDs[i];
            MapXmlNode node = map.nodes[way.NodeIDs[i]];


            //if node has a New trafficLight 
            if (node.hasTrafficLight && !trafficLights.ContainsKey(nodeID))
            {
                trafficLightCount++;
                hasTrafficlights = true;
                MapXmlNode prevNode = map.nodes[way.NodeIDs[i - 1]];// Next Nodes' Location
                Vector3 prevNodeLoc = prevNode - map.bounds.Centre;
                CreateTrafficLightModel(parentObject, node- map.bounds.Centre, nodeID, prevNodeLoc, trafficLightCount, numLanes);
            }
        }

        //Destory parent if no children
        if (!hasTrafficlights)
        {
            Object.DestroyImmediate(parentObject);
        }
        else
        {
            SetParent(way, parentObjectsForWays, parentObject);
        }

    }

    //Make parent of trafficLigts in a single way a child of the root parent which contains all trafficlights

    public void SetParent(MapXmlWay way, Dictionary<MapXmlWay, GameObject> parentObjectsForWays, GameObject trafficLightsParent)
    {

        GameObject parent;

        //Get Root parent holding all trafficLight Objects Across all roads
        if (parentObjectsForWays.ContainsKey(way))
        {
            parent = parentObjectsForWays[way];
        }
        else
        {
            parent = new GameObject();
            parent.name = trafficLightsParent.name;

            parentObjectsForWays.Add(way, parent);
        }

        //Make current parent object child of root parent object
        trafficLightsParent.transform.parent = parent.transform;

    }

    /// <summary>
    /// Creates a single instance of a trafficLight Game Object 
    /// </summary>
    /// <param name="localParentObject">Parent object holding current trafficlights</param>
    /// <param name="currentNode"> Node to spawn traffic light</param>
    /// <param name="nodeID">ulong id for currentNode</param>
    /// <param name="targetDirection">TrafficLight will face this node (Usally previous node in path)</param>
    /// <param name="trafficLightCount">local count to uniquely identify trafficLight under current parent object</param>
    /// <param name="numLanes">number of lans in road. Used to offset trafficLight to side of the road</param>
    /// <returns>Game Object trafficLight model</returns>
    public GameObject CreateTrafficLightModel(GameObject localParentObject, Vector3 currentNode, ulong nodeID, Vector3 targetDirection, int trafficLightCount, int numLanes)
    {

        //instantiate model
        GameObject trafficLight = GameObject.Instantiate(trafficLight_model) as GameObject;

        //Add Ignore raycast layer
        trafficLight.layer = 2;

        if (!trafficLights.ContainsKey(nodeID))
        {
            trafficLights.Add(nodeID, trafficLight);
        }
        else
        {
            trafficLights[nodeID] = trafficLight;
        }
        

        trafficLight.transform.parent = localParentObject.transform;

        trafficLight.name = "TrafficLight_" + trafficLightCount;

        //Set position of Node to the vector
        trafficLight.transform.position = currentNode;

        //set TrafficLight Unique ID
        trafficLight.GetComponent<TrafficLight>().trafficLightId = "" + trafficLightId;
        trafficLightId++;




        //rotate towards previous node
        Vector3 relativePos = targetDirection - trafficLight.transform.position;

        // the second argument, upwards, defaults to Vector3.up
        if (relativePos != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            trafficLight.transform.rotation = rotation;
        }


        //move too the side of the road
        trafficLight.transform.Translate(Vector3.right * numLanes* 5);
        trafficLight.transform.Translate(Vector3.up * 2);
        float height = trafficLight.GetComponent<MeshRenderer>().bounds.size.y;
        trafficLight.transform.position = new Vector3(trafficLight.transform.position.x,(height/2), trafficLight.transform.position.z);

        return trafficLight;
    }



    //Add "Stop Nodes" to each TrafficLight
    //@pram: Dictionary{Key: MapXmlNode ID, Value: a Node GameObject} - Hold all the gameobjects for all nodes in all roads, accessible by their node-(MapXmlNode) ID 
    public void AddStopNodesToTrafficLights(Dictionary<ulong, GameObject> nodeObjectsById)
    {

        if (nodeObjectsById.Count > 0)
        {

            //Loop through all nodes
            foreach (KeyValuePair<ulong, GameObject> node in nodeObjectsById)
            {
                //Check if current Node has TrafficLight
                if (trafficLights.ContainsKey(map.nodes[node.Key].ID))
                {
                    //Get TrafficLight for current Node
                    GameObject trafficLight = trafficLights[map.nodes[node.Key].ID];
                    //add stop node to TrafficLight
                    trafficLight.GetComponent<TrafficLight>().stopNodes.Add(node.Value.transform);
                }
            }
        }
    }

    /*
        Returns the centre of an object
    */
    protected Vector3 GetCentre(MapXmlWay way)
    {
        Vector3 total = Vector3.zero;

        foreach (var id in way.NodeIDs)
        {
            total = total + map.nodes[id];
        }

        return total / way.NodeIDs.Count;
    }

}

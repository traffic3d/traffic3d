using System.Collections.Generic;
using UnityEngine;

public class TrafficLightGenerator
{
    private OpenStreetMapReader osmMapReader;
    private GameObject trafficLight_model;
    Dictionary<ulong, GameObject> trafficLights;
    int trafficLightCount = 0;
    private const int trafficLightToSideRoadOffset = 5;

    //unique ID for each trafficlight in simulation
    private static int trafficLightId = 1;


    public TrafficLightGenerator(OpenStreetMapReader mapReader)
    {
        osmMapReader = mapReader;
        trafficLight_model = Resources.Load("Models/TrafficLight_small") as GameObject;
        trafficLights = new Dictionary<ulong, GameObject>();
    }


    /// <summary>
    /// Loop through all ways and create a trafficlight on roads with a traffic light
    /// </summary>
    /// <param name="parentObjectsForWays">Dictionary linking way to parent gameObject= {Key: Way, Value: Parent gameObject}</param>
    /// <returns>Dictionary(ulong, GameObject) = {Key: NodeID where trafficlight spawned, Value: TrafficLight}</returns>
    public Dictionary<ulong, GameObject> AddTrafficLightsByWay(Dictionary<MapXmlWay, GameObject> parentObjectsForWays)
    {
        // Iterate through each way...
        foreach (var way in osmMapReader.ways)
        {
            //if way is a road...
            if (way.IsRoad)
            {
                CreateTrafficLight(way, way.Lanes, parentObjectsForWays);
            }
        }

        return trafficLights;
    }

    /// <summary>
    /// Spawn Trafficlight on any nodes along roads (ways) flagged as containing a trafficlight. 
    /// Make trafficlight a child of the ways' parent gameobject, using the passed Dictionary.
    /// </summary>
    /// <param name="way">Current way/road</param>
    /// <param name="numLanes">number of lanes in way/road</param>
    /// <param name="parentObjectsForWays">Dictionary linking way to parent gameObject = {Key: Way, Value: Parent gameObject}</param>
    void CreateTrafficLight(MapXmlWay way, int numLanes, Dictionary<MapXmlWay, GameObject> parentObjectsForWays)
    {

        //Holds all the trafficlights for current way
        GameObject parentObject = new GameObject();

        //Trafficlights in road
        trafficLightCount = 0;

        Vector3 origin = GetCentre(way);

        //postion gameObject in road centre
        parentObject.transform.position = origin - osmMapReader.bounds.Centre;
        parentObject.name = way.Name + "_TrafficLights";

        bool hasTrafficlights = false;

        //Loop through nodes in way, and spawn trafficlights on nodes with a trafficlight (Dont add trafficLight to first node)
        for (int i = 1; i < way.NodeIDs.Count; i++)
        {
            //get node
            ulong nodeID = way.NodeIDs[i];
            MapXmlNode node = osmMapReader.nodes[way.NodeIDs[i]];

            //Check node doesn't already have a trafficlight
            if (node.hasTrafficLight && !trafficLights.ContainsKey(nodeID))
            {
                trafficLightCount++;
                hasTrafficlights = true;
                MapXmlNode prevNode = osmMapReader.nodes[way.NodeIDs[i - 1]];// Next Nodes' Location
                Vector3 prevNodeLoc = prevNode - osmMapReader.bounds.Centre;
                //create Trafficlight on node
                CreateTrafficLightModel(parentObject, node - osmMapReader.bounds.Centre, nodeID, prevNodeLoc, trafficLightCount, numLanes);
            }
        }

        //Destory gameobject if no trafficlights in current way
        if (!hasTrafficlights)
            Object.DestroyImmediate(parentObject);
        else
            SetParent(way, parentObjectsForWays, parentObject); // Make way/road and trafficlights share same parent gameObject

    }


    /// <summary>
    /// Make way/road and trafficlights share same parent gameObject
    /// </summary>
    /// <param name="way">Way/road to which the trafficlights belong</param>
    /// <param name="parentObjectsForWays">Dictionary linking 'ways' to 'parent gameObjects'</param>
    /// <param name="trafficLightsParent">gameobject holding all trafficlights in way/road</param>
    public void SetParent(MapXmlWay way, Dictionary<MapXmlWay, GameObject> parentObjectsForWays, GameObject trafficLightsParent)
    {

        GameObject parent;

        //Check if road has a parent gameObject
        if (parentObjectsForWays.ContainsKey(way))
        {
            //get parent
            parent = parentObjectsForWays[way];
        }
        else
        {
            //create new parent
            parent = new GameObject();
            parent.name = trafficLightsParent.name;

            parentObjectsForWays.Add(way, parent);
        }

        //Make Gameobject holding all trafficlights a child of the roads' parent. (Road and trafficlights will now share same parent)
        trafficLightsParent.transform.parent = parent.transform;

    }

    /// <summary>
    /// Creates a single instance of a trafficLight GameObject at a specific node and offset the trafficlight to the righ by the roads width.
    /// Trafficlights are orientated towards the target node 
    /// </summary>
    /// <param name="localParentObject">Parent object holding current trafficlights</param>
    /// <param name="currentNode"> Node to spawn traffic light</param>
    /// <param name="nodeID">ulong id for currentNode</param>
    /// <param name="targetDirection">TrafficLight will face towards this position</param>
    /// <param name="trafficLightCount">local count to uniquely identify trafficLight under current parent object</param>
    /// <param name="numLanes">number of lans in road. Used to offset trafficLight to side of the road</param>
    /// <returns>TrafficLight GameObject</returns>
    public GameObject CreateTrafficLightModel(GameObject localParentObject, Vector3 currentNode, ulong nodeID, Vector3 targetDirection, int trafficLightCount, int numLanes)
    {

        //instantiate model
        GameObject trafficLight = GameObject.Instantiate(trafficLight_model) as GameObject;

        //Add 'Ignore raycast' layer
        trafficLight.layer = LayerMask.NameToLayer("Ignore Raycast");

        //Check if TrafficLight already created
        if (!trafficLights.ContainsKey(nodeID))
            trafficLights.Add(nodeID, trafficLight); //add to list
        else
            trafficLights[nodeID] = trafficLight; //Overwrite existing trafficlight


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

        //offset to side of road
        trafficLight.transform.Translate(Vector3.right * numLanes * trafficLightToSideRoadOffset);
        trafficLight.transform.Translate(Vector3.up * 2);
        float height = trafficLight.GetComponent<MeshRenderer>().bounds.size.y; //height
        trafficLight.transform.position = new Vector3(trafficLight.transform.position.x, (height / 2), trafficLight.transform.position.z);

        return trafficLight;
    }


    /// <summary>
    /// Add "Stop Nodes" to each TrafficLight
    /// </summary>
    /// <param name="nodeObjectsById">Dictionary linking nodeId to node gameObject - {Key: MapXmlNode ID, Value: a Node GameObject}</param>
    public void AddStopNodesToTrafficLights(Dictionary<ulong, GameObject> nodeObjectsById)
    {

        if (nodeObjectsById.Count > 0)
        {

            //Loop through all nodes
            foreach (KeyValuePair<ulong, GameObject> node in nodeObjectsById)
            {
                //Check if current Node has TrafficLight
                if (trafficLights.ContainsKey(osmMapReader.nodes[node.Key].ID))
                {
                    //Get TrafficLight for current Node
                    GameObject trafficLight = trafficLights[osmMapReader.nodes[node.Key].ID];
                    //add stop node to TrafficLight
                    trafficLight.GetComponent<TrafficLight>().stopNodes.Add(node.Value.GetComponent<RoadNode>());
                }
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

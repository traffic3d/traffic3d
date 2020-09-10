using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles all key scripts required to generate a scene.
/// Calls the map reader to read map data
/// Calls scripts that are responsible for the generation of different scene elements
/// </summary>
public class ImportOsmUiWrapper
{
    //GUI
    ImportOsmGui gui;
    string filePath;

    //Materials
    Material road_material;
    Material building_material;
    Material floor_material;

    //Vars
    public Dictionary<MapXmlWay, GameObject> parentObjectsForWays; //{Key: Way, Value: Parent_object} - Ensures all elements of a Way are all connected to the same Parent_Object
    OpenStreetMapReader osmMapReader;

    //Generators
    public BuildingGenerator buildingGenerator;
    public RoadGenerator roadGenerator;
    public PathGenerator pathGenerator;
    public TrafficLightGenerator trafficLightGenerator;
    public JunctionGenerator junctionGenerator;


    public ImportOsmUiWrapper( string osmFile, Material roadMat, Material floorMat, Material buildingMat)
    {
        filePath = osmFile;
        road_material = roadMat;
        floor_material = floorMat;
        building_material = buildingMat;
       
    }

    /// <summary>
    /// Calls all method and classes responsible generating a Road Network
    /// </summary>
    /// <returns>True if successfully uploaded data</returns>
    public bool Import()
    {
      
        //Return if failed to Read Data
        if (!ReadMapData())
            return false;
        
        CreateFloor();

        //Create Traffic3D required Objects & scripts
        CreatePythonManager();
        CreateTrafficLightManager();
        GameObject vehicleFactory = CreateVehicleFactory();


        //Defines the parent game object connected to each each Way (Key: "Way", Value: "Parent_Object") 
        parentObjectsForWays = new Dictionary<MapXmlWay, GameObject>(osmMapReader.ways.Count);

        //Create scene objects 
        buildingGenerator = new BuildingGenerator(osmMapReader, building_material);
        roadGenerator = new RoadGenerator(osmMapReader, road_material);
        pathGenerator = new PathGenerator(osmMapReader, vehicleFactory);
        trafficLightGenerator = new TrafficLightGenerator(osmMapReader);
        junctionGenerator = new JunctionGenerator();


        // - Spawn and connect each element in the Scene -

        GenerateBuildings();

        GenerateRoads();

        MergeRoadsAndPaths();

        GenerateJunctions();

        pathGenerator.RemovePathsWithTwoNodes();

        pathGenerator.PopulateVehicleFactory();


        //GenerateTrafficLights(pathGenerator.GetAllNodesInRoadNetwork()); //NOTE: No Longer supporting real-world trafficLights due to lack of trafficLights in map data. Junctions now spawn the trafficlights

        return true;
    }

    void GenerateBuildings()
    {
        // Add buildings
        buildingGenerator.GenerateBuildings();
    }

    void GenerateRoads()
    {
        // Add roads
        roadGenerator.GenerateRoads();
        MergeDictionary(parentObjectsForWays, roadGenerator.GetWayObjects());

        //If roads not generated first, displays nothing
        GenerateVehiclePaths();

    }

    void GenerateVehiclePaths()
    {
        //Add Vehicle Paths
        pathGenerator.AddPathsToRoads(parentObjectsForWays); // Add vehicle paths
    }

    //Merges connected roads with same name together, and link Road mesh to vehicle path
    void MergeRoadsAndPaths()
    {
        //Combine all connected roads with the same name
        pathGenerator.JoinRoadsWithSameName();

        UpdateRoadPathConnections();
    }

    //Call after roads are merged, to update road/paths/parent-way defining objects
    void UpdateRoadPathConnections()
    {
        //remove roads linked to any deleted vehicle paths
        RemoveDeletedRoads(pathGenerator.GetDeletedVehiclePaths());

        // - Link Road meshes to Vehicle Paths
        LinkRoadToVehiclePath();
    }

    void GenerateJunctions()
    {
        UpdateRoadPathConnections();
        junctionGenerator.GenerateJunctions(trafficLightGenerator);
        UpdateRoadPathConnections();
    }

    void GenerateTrafficLights(Dictionary<ulong, GameObject> roadNodesById)
    {
        // Add Trafficlights
        Dictionary<ulong, GameObject> trafficLights = trafficLightGenerator.AddTrafficLightsByWay(parentObjectsForWays);

        //Add stop nodes to trafficLights
        trafficLightGenerator.AddStopNodesToTrafficLights(roadNodesById);
    }

    //For each road: Link the roads' "Mesh" to the roads' "Path", so if the nodes along the path are modified, then the mesh will auto update.
    void LinkRoadToVehiclePath()
    {

        foreach (KeyValuePair<MapXmlWay, GameObject> kv in parentObjectsForWays) {


            //Get the Parent_GameObject for the current way
            GameObject RoadParentObject = kv.Value;

            //RoadMesh = first child
            GameObject roadMeshHolder = RoadParentObject.transform.GetChild(0).gameObject;

            //Check if has component
            if (!roadMeshHolder.GetComponent<RoadMeshUpdater>())
            {
                //Path = last child
                GameObject path = RoadParentObject.transform.GetChild(RoadParentObject.transform.childCount - 1).gameObject;
                roadMeshHolder.AddComponent<RoadMeshUpdater>();
                //Initializse values
                roadMeshHolder.GetComponent<RoadMeshUpdater>().SetValues(kv.Key.Lanes, roadMeshHolder, path,roadGenerator.DefaultLaneWidth);
            }

            //Update road Mesh
             roadMeshHolder.GetComponent<RoadMeshUpdater>().UpdateRoadMesh();
            
        }
    }

    //Merges all elements from a secondary dictionary into the primary dictionary
    void MergeDictionary(Dictionary<MapXmlWay, GameObject> primaryDic, Dictionary<MapXmlWay, GameObject> addedDic)
    {
       
        foreach (var item in addedDic)
        {
            primaryDic.Add(item.Key, item.Value);
        }
        
    }

    //vehiclePaths - Gameobjects to remove
    void RemoveDeletedRoads( List<GameObject> deletedPaths)
    {
        //loop through deleted Vehicle Path GameObjects
        foreach(var deletedPath in deletedPaths)
        {
            if (deletedPath != null)
            {
                //Get parent to deleted gameObject
                var kvPairForDeletedPath = parentObjectsForWays.First(kvp => kvp.Value == deletedPath.transform.parent.gameObject);

                //remove deleted gameObject from dictionary storing "way->Parent_object" relationships
                parentObjectsForWays.Remove(kvPairForDeletedPath.Key);

                //remove deleted gameObject from roadGenerator
                roadGenerator.DeleteRoad(deletedPath.transform.parent.gameObject);
            }
        }

    }

    /// <summary>
    /// Testing Method. Returns number of nodes in current MapReader
    /// </summary>
    /// <returns></returns>
    public int GetNodesInScene()
    {
        return osmMapReader.nodes.Count;
    }

    //Floor for the scene
    void CreateFloor()
    {
        //Create floor object
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);

        //add scripts
        floor.AddComponent<MeshCollider>();
        Renderer rend = floor.GetComponent<Renderer>();

        rend.material = floor_material; //Add material

        //position floor at center, and just slightly below road height.
        floor.transform.position = new Vector3(0, -0.01f, 0);
        //scale to size of map bounds
        floor.transform.localScale = osmMapReader.bounds.Size;
        //Ensure Y-axis scale is set to 1
        floor.transform.localScale = new Vector3(floor.transform.localScale.x, 1f, floor.transform.localScale.z);
    }

    GameObject CreateVehicleFactory()
    {
        GameObject vehicleFactory = new GameObject("VehicleFactory");
        vehicleFactory.AddComponent<VehicleFactory>();
        return vehicleFactory;
    }

    void CreatePythonManager()
    {
        //Initialize list in PythonManager
        GameObject pm = new GameObject("PythonManager");
        pm.AddComponent<PythonManager>();
    }

    void CreateTrafficLightManager()
    {
        GameObject tlm = new GameObject("TrafficLightManager");
        tlm.AddComponent<TrafficLightManager>();
    }


    /// <summary>
    /// Use public MapReader to read Map File
    /// </summary>
    /// <returns>Bool if successfully read data</returns>
    bool ReadMapData()
    {
        osmMapReader = new OpenStreetMapReader();

        //Try to read file
        try
        {
            //Read File and create node & way objects
            osmMapReader.ImportFile(filePath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

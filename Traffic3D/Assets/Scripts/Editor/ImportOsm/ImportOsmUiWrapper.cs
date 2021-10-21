using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Handles all key scripts required to generate a scene.
/// </summary>
/// <remarks>
/// Calls the map reader to read map data.
/// Calls scripts that are responsible for the generation of different scene elements.
/// </remarks>
public class ImportOsmUiWrapper
{
    //GUI
    ImportOsmGui osmGui;
    string filePath;
    //Materials
    Material road_material;
    Material building_material;
    Material floor_material;
    float defaultLaneWidth;
    //Vars
    public Dictionary<MapXmlWay, GameObject> parentObjectsForWays; //{Key: Way, Value: Parent_object} - Ensures all elements of a Way are all connected to the same Parent_Object
    OpenStreetMapReader osmMapReader;
    public bool isLeftHandDrive;
    //Generators
    public BuildingGenerator buildingGenerator;
    public RoadGenerator roadGenerator;
    public PathGenerator pathGenerator;
    public TrafficLightGenerator trafficLightGenerator;
    public JunctionGenerator junctionGenerator;
    public StreetFurnitureGenerator streetFurnitureGenerator;

    public ImportOsmUiWrapper(ImportOsmGui osmGui, string osmFile, Material roadMat, Material floorMat, Material buildingMat, float defaultLaneWidth, bool isLeftHandDrive)
    {
        this.osmGui = osmGui;
        filePath = osmFile;
        road_material = roadMat;
        floor_material = floorMat;
        building_material = buildingMat;
        this.defaultLaneWidth = defaultLaneWidth;
        this.isLeftHandDrive = isLeftHandDrive;
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
        CreateEnvironmentSettings();
        VehicleFactory vehicleFactory = CreateVehicleFactory();
        //Defines the parent game object connected to each each Way (Key: "Way", Value: "Parent_Object") 
        parentObjectsForWays = new Dictionary<MapXmlWay, GameObject>(osmMapReader.ways.Count);
        //Create scene objects 
        buildingGenerator = new BuildingGenerator(osmMapReader, building_material);
        roadGenerator = new RoadGenerator(osmMapReader, road_material, defaultLaneWidth);
        pathGenerator = new PathGenerator(osmMapReader, isLeftHandDrive);
        trafficLightGenerator = new TrafficLightGenerator(osmMapReader);
        junctionGenerator = new JunctionGenerator();
        streetFurnitureGenerator = new StreetFurnitureGenerator(osmMapReader, isLeftHandDrive);
        //ProgressBar values
        int totalTasks = 5;
        int tasksComplete = 0;
        // - Spawn and connect each element in the Scene -
        ImportProgress("Generating Buildings", totalTasks, tasksComplete++);
        GenerateBuildings();
        ImportProgress("Generating Roads", totalTasks, tasksComplete++);
        GenerateRoads();
        ImportProgress("Merging Roads & Paths", totalTasks, tasksComplete++);
        MergeRoadsAndPaths();
        ImportProgress("Generating Junctions", totalTasks, tasksComplete++);
        GenerateJunctions();
        ImportProgress("Generating Street Furniture", totalTasks, tasksComplete++);
        GenerateStreetFurniture();
        // NOTE: No Longer supporting real-world trafficLights due to lack of trafficLights in map data. Junctions now spawn the trafficlights
        ImportProgress("Completed", totalTasks, tasksComplete++);
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
        roadGenerator.RenderRoads();
    }

    void GenerateVehiclePaths()
    {
        //Add Vehicle Paths
        pathGenerator.AddPathsToRoads(parentObjectsForWays); // Add vehicle paths
    }

    /// <summary>
    /// Merges connected roads with same name together, and links a Road mesh to its vehicle path
    /// </summary>
    void MergeRoadsAndPaths()
    {
        //Combine all connected roads with the same name
        UpdateRoadPathConnections();
    }

    /// <summary>
    /// Updates road, vehicle path and the dictionary "parentObjectsForWays" which links a parent object to a way.
    /// </summary>
    /// <remarks>
    /// Call after roads are merged.
    /// </remarks>
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

    void GenerateStreetFurniture()
    {
        streetFurnitureGenerator.GenerateStreetFurniture();
    }

    void GenerateTrafficLights(Dictionary<ulong, GameObject> roadNodesById)
    {
        // Add Trafficlights
        Dictionary<ulong, GameObject> trafficLights = trafficLightGenerator.AddTrafficLightsByWay(parentObjectsForWays);
        //Add stop nodes to trafficLights
        trafficLightGenerator.AddStopNodesToTrafficLights(roadNodesById);
    }

    /// <summary>
    /// For each road: Links the roads' "Mesh" to the roads' "Path", so if the nodes along the path are modified, then the mesh will auto update.
    /// </summary>
    void LinkRoadToVehiclePath()
    {
        foreach (KeyValuePair<MapXmlWay, GameObject> kv in parentObjectsForWays)
        {
            //Get the Parent_GameObject for the current way
            GameObject RoadParentObject = kv.Value;
            //RoadMesh = first child
            foreach (RoadWay roadWay in RoadParentObject.GetComponentsInChildren<RoadWay>())
            {
                GameObject pathGameObject = roadWay.gameObject;
                //Check if has component
                if (!pathGameObject.GetComponent<RoadMeshUpdater>())
                {
                    pathGameObject.AddComponent<RoadMeshUpdater>();
                    //Initializse values
                    pathGameObject.GetComponent<RoadMeshUpdater>().SetValues(pathGameObject, pathGameObject.GetComponent<RoadWay>(), defaultLaneWidth);
                }
                //Update road Mesh
                pathGameObject.GetComponent<RoadMeshUpdater>().UpdateRoadMesh();

            }
        }
    }

    /// <summary>
    /// Merges all elements from a secondary dictionary into the primary dictionary
    /// </summary>
    /// <param name="primaryDic">Dictionary receiving data</param>
    /// <param name="addedDic">Dictionary from which the data will be copied</param>
    void MergeDictionary(Dictionary<MapXmlWay, GameObject> primaryDic, Dictionary<MapXmlWay, GameObject> addedDic)
    {
        foreach (var item in addedDic)
        {
            primaryDic.Add(item.Key, item.Value);
        }
    }

    /// <summary>
    /// vehiclePaths - Gameobjects to remove
    /// </summary>
    /// <param name="deletedPaths">list of all deleted vehicle paths</param>
    void RemoveDeletedRoads(List<GameObject> deletedPaths)
    {
        //loop through deleted Vehicle Path GameObjects
        foreach (var deletedPath in deletedPaths)
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
    /// <returns>Total number of nodes read by the OpenStreetMapReader</returns>
    public int GetNodesInScene()
    {
        return osmMapReader.nodes.Count;
    }

    /// <summary>
    /// Floor for the scene
    /// </summary>
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

    VehicleFactory CreateVehicleFactory()
    {
        GameObject vehicleFactoryGameObject = new GameObject("VehicleFactory");
        VehicleFactory vehicleFactory = vehicleFactoryGameObject.AddComponent<VehicleFactory>();
        string[] assets = Directory.GetFiles("Assets/Vehicles", "*.prefab");
        List<Vehicle> vehicles = new List<Vehicle>();
        foreach (string path in assets)
        {
            GameObject vehicle = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (vehicle != null)
            {
                vehicles.Add(vehicle.GetComponent<Vehicle>());
            }
        }
        vehicleFactory.SetDefaultVehicleProbabilities(vehicles);
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

    void CreateEnvironmentSettings()
    {
        GameObject.Instantiate(Resources.Load<EnvironmentSettings>("Models/EnvironmentSettings"));
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

    /// <summary>
    /// Updates GUI progress bar.
    /// </summary>
    /// <remarks>
    /// If GUI is null, method does nothing
    /// </remarks>
    /// <param name="ProgressText"></param>
    /// <param name="totalTasks"></param>
    /// <param name="tasksComplete"></param>
    void ImportProgress(string ProgressText, int totalTasks, int tasksComplete)
    {
        if (osmGui != null)
        {
            float progressPercentage = 0f;

            if (tasksComplete > 0)
                progressPercentage = (float)tasksComplete / totalTasks;

            osmGui.UpdateProgressBar(progressPercentage, ProgressText);
        }
    }
}

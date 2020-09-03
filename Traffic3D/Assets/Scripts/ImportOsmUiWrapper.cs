using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    MapReader mapReader;

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
    /// Calls all method and classes responsible for reading a file and generating a Road Network
    /// </summary>
    /// <returns>True if successfully uploaded data</returns>
    public bool Import()
    {
        mapReader = new MapReader();

        //Check if importing invalid file or file with invalid data
        try {
            //Read File and create node & way objects
            mapReader.ImportFile(filePath);
        } catch
        {
            return false;
        }

        //Defines the parent game object connected to each each Way (Key: "Way", Value: "Parent_Object") 
        parentObjectsForWays = new Dictionary<MapXmlWay, GameObject>(mapReader.ways.Count);

        //Create scene objects 

        buildingGenerator = new BuildingGenerator(mapReader, building_material);
        roadGenerator = new RoadGenerator(mapReader, road_material, floor_material);
        pathGenerator = new PathGenerator(mapReader);
        trafficLightGenerator = new TrafficLightGenerator(mapReader);
        junctionGenerator = new JunctionGenerator();


        // - Spawn and connect each element in the Scene -

        GenerateBuildings();

        GenerateRoads();

        MergeRoadsAndPaths();

        GenerateJunctions();

        pathGenerator.RemovePathsWithTwoNodes();
        pathGenerator.PopulateVehicleFactory();


        //GenerateTrafficLights(pathGenerator.GetAllNodesInRoadNetwork()); //NOTE: No Longer supporting real-world trafficLights due to lack of trafficLights in map data

        //mapReader.DrawTheLines();

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

        //update dictionary to match changes to road
        //parentObjectsForWays = roadGenerator.GetWayObjects(); //Currently causes error, but seems to work fine without method call
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
                roadMeshHolder.GetComponent<RoadMeshUpdater>().setValues(kv.Key.Lanes, roadMeshHolder, path,roadGenerator.DefaultLaneWidth);
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
        return mapReader.nodes.Count;
    }
}

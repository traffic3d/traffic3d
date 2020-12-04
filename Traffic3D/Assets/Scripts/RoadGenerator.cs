using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generate Road Mesh for each way which is a road, and assign to a game Object.
/// GameObject position and name is based on the way
/// </summary>
public class RoadGenerator : BaseAssetGenerator
{
    public Material road_material;
    public float DefaultLaneWidth { get; } = 3.65f;

    /// <summary>
    /// Creates a floor gameobject on
    /// </summary>
    /// <param name="mapReader"></param>
    /// <param name="roadMaterial"></param>
    /// <param name="floor_material"></param>
    public RoadGenerator(OpenStreetMapReader mapReader, Material roadMaterial) : base(mapReader)
    {
        road_material = roadMaterial;

        //Initialize parent so all roads stored under same gameObject called 'Roads'
        InitializeRootParent("Roads");
    }

    /// <summary>
    /// Loops through all ways in map reader and generates: road gameObject, Mesh and a Label showing road name.
    /// Sets parent object for road.
    /// </summary>
    public void GenerateRoads()
    {
        // Iterate through ways...
        foreach (var way in osmMapReader.ways)
        {
            //if road...
            if (way.IsRoad)
            {
                // Create Road object and assign a Mesh
                GameObject road = GenerateObject(way, road_material, way.Name);

                //add text label above road
                AddRoadNames(way, road);

                //Link current object holding road_mesh to the parent_Object holding all objects assigned to current road
                SetParent(way, road);

                //make Parent_object child of Root_Parent, assigned to parent for "all Roads""
                AddToRootParent(GetParent(way));
            }

        }
    }

    /// <summary>
    /// Abstract method. Gets node positions as Vector3s and calls method to create mesh
    /// </summary>
    /// <param name="way">Road</param>
    /// <param name="origin">center position</param>
    /// <param name="vectors">node postions as Vector3s'</param>
    /// <param name="normals"></param>
    /// <param name="uvs"></param>
    /// <param name="indices"></param>
    /// <returns></returns>
    protected override Mesh InitializeMesh(MapXmlWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices)
    {
        List<Vector3> VectorNodesInRoad = new List<Vector3>(way.NodeIDs.Count);

        //for each node, get Vector3 position
        foreach (ulong id in way.NodeIDs)
        {
            MapXmlNode node = osmMapReader.nodes[id];
            VectorNodesInRoad.Add(node - origin);
        }

        RoadGenerationHandler rgh = new RoadGenerationHandler();

        return rgh.CreateRoadMesh(VectorNodesInRoad, way.Lanes, DefaultLaneWidth);
    }

    /// <summary>
    /// Create Label above roads center node
    /// </summary>
    /// <param name="way">Way for road</param>
    /// <param name="wayObject">Road GameObject</param>
    void AddRoadNames(MapXmlWay way, GameObject wayObject)
    {
        string name = way.Name;

        if (!string.IsNullOrEmpty(name))
        {
            name = way.Name;
            GameObject roadName = new GameObject("Road_Name");
            roadName.AddComponent<TextMesh>();
            TextMesh roadNameLabel = roadName.GetComponent<TextMesh>();
            roadNameLabel.anchor = TextAnchor.MiddleCenter;
            roadNameLabel.text = name;
            roadNameLabel.characterSize = 4;
            roadNameLabel.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            roadNameLabel.fontSize = 60;
            //Get middle Node index in path. 
            int middleIndex = middleIndex = (int)Math.Floor((decimal)((way.NodeIDs.Count - 1) / 2));
            //rotate towards previous node
            Vector3 midNodePos = osmMapReader.nodes[way.NodeIDs[middleIndex]] - osmMapReader.bounds.Centre;
            Vector3 nextNodePos = osmMapReader.nodes[way.NodeIDs[middleIndex + 1]] - osmMapReader.bounds.Centre;
            //Position label above center of road
            roadNameLabel.transform.position = midNodePos;
            //add label to road gameObject
            roadName.transform.parent = wayObject.transform;
            RoadGenerationHandler rgh = new RoadGenerationHandler();
            rgh.PositionRoadLabel(midNodePos, nextNodePos, way.NodeIDs.Count, roadNameLabel.gameObject, roadNameLabel.GetComponent<TextMesh>(), false);
        }
    }


    /// <summary>
    /// Make road a child of the ways' parent object
    /// </summary>
    /// <param name="way">Way linked to the current wayObject</param>
    /// <param name="road">Current road being handled</param>
    public void SetParent(MapXmlWay way, GameObject road)
    {
        //initialize object
        GameObject parent;

        //If current way doesn't already have a parent
        if (GetParent(way) == null)
        {
            //create new parent gameObject
            parent = new GameObject();

            //Give parent same name as object-(road)
            parent.name = road.name;

            //Add way and parent to Dictionary
            LinkWayToParent(way, parent);

            //make way a child of the 'parent Object'
            road.transform.parent = parent.transform;
        }
    }

    /// <summary>
    /// Delete road and remove from parentObject dictionary
    /// </summary>
    /// <param name="parent">parent of road</param>
    public void DeleteRoad(GameObject parent)
    {
        //destroy game object
        UnityEngine.Object.DestroyImmediate(parent);

        //remove from dictionary<Way, ParentObject>
        var parentObject = GetWayObjects().First(kvp => kvp.Value == parent);
        GetWayObjects().Remove(parentObject.Key);

    }
}
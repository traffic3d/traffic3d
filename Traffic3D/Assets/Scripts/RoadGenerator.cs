using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadGenerator : BaseAssetGenerator
{

    public Material road_material;
    public float DefaultLaneWidth { get; } = 3.65f;
    


    public RoadGenerator(MapReader mapReader, Material roadMaterial, Material floor_material) : base(mapReader)
    {
        road_material = roadMaterial;

        //Create floor object
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);

        //Add material to floor
        floor.AddComponent<MeshCollider>();
        Renderer rend = floor.GetComponent<Renderer>();
        rend.material = floor_material;

        //position floor at center, and just slightly below road height.
        floor.transform.position = new Vector3(0, -0.01f, 0);

        //scale to size of map bounds
        floor.transform.localScale = mapReader.bounds.Size;
        //Ensure Y-axis scale is set to 1
        floor.transform.localScale = new Vector3(floor.transform.localScale.x, 1f, floor.transform.localScale.z);

        //Create GameObject to store all roads under
        InitializeRootParent("Roads");
    }

    public void GenerateRoads()
    {
        // Iterate through ways...
        foreach (var way in map.ways)
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

    protected override Mesh InitializeMesh(MapXmlWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices)
    {
        List<Vector3> VectorNodesInRoad = new List<Vector3>(way.NodeIDs.Count);

        foreach (ulong id in way.NodeIDs)
        {
            MapXmlNode n = map.nodes[id];
            VectorNodesInRoad.Add(n-origin);
        }

        
        Mesh mesh = CreateMesh(VectorNodesInRoad, way.Lanes);
        return mesh;
    }

    public Mesh CreateMesh(List<Vector3> nodePositions, int numLanes)
    {
        RoadGenerationHandler rgh = new RoadGenerationHandler();
        return rgh.CreateRoadMesh(nodePositions, numLanes, DefaultLaneWidth);

    }


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
            int middleIndex = middleIndex = (int)Math.Floor((decimal)( (way.NodeIDs.Count-1) / 2));

            //rotate towards previous node
            Vector3 midNodePos = map.nodes[way.NodeIDs[middleIndex]] - map.bounds.Centre;
            Vector3 nextNodePos = map.nodes[way.NodeIDs[middleIndex + 1]] - map.bounds.Centre;

            //Position label above center of road
            roadNameLabel.transform.position = midNodePos;


            //add label to road gameObject
            roadName.transform.parent = wayObject.transform;

            RoadGenerationHandler rgh = new RoadGenerationHandler();
            rgh.PositionRoadLabel(midNodePos, nextNodePos, way.NodeIDs.Count, roadNameLabel.gameObject, roadNameLabel.GetComponent<TextMesh>(), false);


        }
    }



    // Ensure current object (e.g Road_Mesh/path_of_nodes/building_mesh/...) is a child of the Ways' Parent gameObject
    // Way - Way linked to the current wayObject
    // road - Current road being handled
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

    //Deletes road
    //Parent - parent gameObject
    public void DeleteRoad(GameObject parent)
    {
        //destroy game object
        UnityEngine.Object.DestroyImmediate(parent);

        //remove from dictionary<Way, ParentObject>
        var parentObject = GetWayObjects().First(kvp => kvp.Value == parent);
        GetWayObjects().Remove(parentObject.Key);

    }

    
}


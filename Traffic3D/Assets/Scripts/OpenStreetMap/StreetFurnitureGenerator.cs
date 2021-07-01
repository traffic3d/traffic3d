using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StreetFurnitureGenerator : BaseAssetGenerator
{
    private OpenStreetMapReader mapReader;
    private int streetLightCounter = 0;
    private const float streetLightDistanceApart = 50f;
    private const float streetLightAwayFromRoad = 5f;
    private const float streetLightMinDistanceFromOtherRoads = 10f;
    private bool isLeftHandDrive;
    public List<MapXmlNode> amenityAndEmergencyNodes;

    public StreetFurnitureGenerator(OpenStreetMapReader mapReader, bool isLeftHandDrive) : base(mapReader)
    {
        InitializeRootParent("StreetFurniture");
        this.mapReader = mapReader;
        this.isLeftHandDrive = isLeftHandDrive;
        this.amenityAndEmergencyNodes = new List<MapXmlNode>();
    }

    public void GenerateStreetFurniture()
    {
        RoadNetworkManager.GetInstance().Reload();
        GenerateAmenities();
        GenerateStreetLights();
    }

    public void GenerateAmenities()
    {
        List<GameObject> amenityPrefabs = Resources.LoadAll<GameObject>("Models/OpenStreetMap/Amenities").ToList();
        List<GameObject> emergencyPrefabs = Resources.LoadAll<GameObject>("Models/OpenStreetMap/Emergency").ToList();
        foreach (MapXmlNode node in mapReader.nodes.Values)
        {
            GameObject prefab = null;
            if (node.amenity != null)
            {
                prefab = amenityPrefabs.Find(g => g.name == node.amenity);
            }
            if (node.emergency != null)
            {
                prefab = emergencyPrefabs.Find(g => g.name == node.emergency);
            }
            if (prefab == null)
            {
                continue;
            }
            GameObject placedObject = GameObject.Instantiate(prefab, node - osmMapReader.bounds.Centre, Quaternion.Euler(new Vector3(0, 0, 0)));
            placedObject.name = prefab.name + "_" + node.ID;
            AddToRootParent(placedObject);
        }
    }

    public void GenerateStreetLights()
    {
        float streetLightAngle;
        if (isLeftHandDrive)
        {
            streetLightAngle = -90f;
        }
        else
        {
            streetLightAngle = 90f;
        }
        foreach (Road road in RoadNetworkManager.GetInstance().GetRoads())
        {
            List<RoadWay> roadWaysToUse = new List<RoadWay>();
            if (road.numberOfLanes == 0)
            {
                continue;
            }
            roadWaysToUse.Add(road.roadWays.First());
            // Add last roadway if more than 2 lanes
            if (road.numberOfLanes > 1)
            {
                roadWaysToUse.Add(road.roadWays.Last());
            }
            // The first and last roadways are the side of the main roads and can have streetlights placed along them.
            foreach (RoadWay roadWay in roadWaysToUse)
            {
                float nextStreetLightDistance = streetLightDistanceApart;
                for (int i = 0; i < roadWay.nodes.Count - 1; i++)
                {
                    RoadNode node = roadWay.nodes[i];
                    RoadNode nextNode = roadWay.nodes[i + 1];
                    float x1 = node.transform.position.x;
                    float y1 = node.transform.position.y;
                    float z1 = node.transform.position.z;
                    float x2 = nextNode.transform.position.x;
                    float y2 = nextNode.transform.position.y;
                    float z2 = nextNode.transform.position.z;
                    float totalNodeToNextNodeDistance = Vector3.Distance(node.transform.position, nextNode.transform.position);
                    float distanceAlongNode = nextStreetLightDistance;
                    while (distanceAlongNode < totalNodeToNextNodeDistance)
                    {
                        float angle = Mathf.Atan2(z2 - z1, x2 - x1) * 180 / Mathf.PI;
                        double ratio = distanceAlongNode / totalNodeToNextNodeDistance;
                        float xDest = (float)((1 - ratio) * x1 + ratio * x2);
                        float yDest = (float)((1 - ratio) * y1 + ratio * y2) - 1; // -1 because road mesh is created -1 down from nodes
                        float zDest = (float)((1 - ratio) * z1 + ratio * z2);
                        Vector3 streetLightPosition = new Vector3(xDest, yDest, zDest);
                        // Check distance to nodes and to other roads
                        if (!roadWay.nodes.Exists(n => Vector3.Distance(n.transform.position, streetLightPosition) < streetLightMinDistanceFromOtherRoads && RoadNetworkManager.GetInstance().GetRoadWaysFromNode(n).Count > 1))
                        {
                            GameObject streetLampPrefab = Resources.Load<GameObject>("Models/streetlight");
                            GameObject streetLamp = GameObject.Instantiate(streetLampPrefab, streetLightPosition, Quaternion.Euler(new Vector3(0, 0, 0)));
                            streetLamp.name = "StreetLight_" + streetLightCounter++ + "_" + roadWay.name;
                            AddToRootParent(streetLamp);
                            streetLamp.transform.RotateAround(streetLightPosition, Vector3.up, streetLightAngle);
                            streetLamp.transform.Rotate(Vector3.up, -angle);
                            streetLamp.transform.Translate(Vector3.right * streetLightAwayFromRoad, Space.Self); // Right moves the light backwards
                        }
                        distanceAlongNode = distanceAlongNode + streetLightDistanceApart;
                    }
                    nextStreetLightDistance = distanceAlongNode - totalNodeToNextNodeDistance;
                }
            }
        }
    }

    protected override Mesh InitializeMesh(MapXmlWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> tris)
    {
        throw new System.NotSupportedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StreetFurnitureGenerator
{
    private int streetLightCounter = 0;
    private const float streetLightDistanceApart = 50f;
    private const float streetLightAwayFromRoad = 5f;
    private List<RoadWay> roadWays;
    private Transform root;

    public void GenerateStreetFurniture()
    {
        root = new GameObject("StreetFurniture").transform;
        roadWays = GameObject.FindObjectsOfType<RoadWay>().ToList();
        GenerateStreetLights();
    }

    public void GenerateStreetLights()
    {
        foreach (RoadWay roadWay in roadWays)
        {
            float nextStreetLightDistance = streetLightDistanceApart;
            for (int i = 0; i < roadWay.nodes.Count - 1; i++)
            {
                Transform node = roadWay.nodes[i].transform;
                Transform nextNode = roadWay.nodes[i + 1].transform;
                float x1 = node.position.x;
                float y1 = node.position.y;
                float z1 = node.position.z;
                float x2 = nextNode.position.x;
                float y2 = nextNode.position.y;
                float z2 = nextNode.position.z;
                float totalNodeToNextNodeDistance = Vector3.Distance(node.position, nextNode.position);
                float distanceAlongNode = nextStreetLightDistance;
                while (distanceAlongNode < totalNodeToNextNodeDistance)
                {
                    float angle = Mathf.Atan2(z2 - z1, x2 - x1) * 180 / Mathf.PI;
                    double ratio = distanceAlongNode / totalNodeToNextNodeDistance;
                    float xDest = (float)((1 - ratio) * x1 + ratio * x2);
                    float yDest = (float)((1 - ratio) * y1 + ratio * y2) - 1; // -1 because road mesh is created -1 down from nodes
                    float zDest = (float)((1 - ratio) * z1 + ratio * z2);
                    GameObject streetLampPrefab = Resources.Load<GameObject>("Models/streetlight");
                    GameObject streetLamp = GameObject.Instantiate(streetLampPrefab, new Vector3(xDest, yDest, zDest), Quaternion.Euler(new Vector3(0, 0, 0)));
                    Debug.Log("Final Check: " + xDest + " " + yDest + " " + zDest);
                    Debug.Log("Final Check Obj: " + streetLamp.transform.position.x + " " + streetLamp.transform.position.y + " " + streetLamp.transform.position.z);
                    streetLamp.name = "StreetLight_" + streetLightCounter++;
                    streetLamp.transform.SetParent(root);
                    streetLamp.transform.RotateAround(new Vector3(xDest, yDest, zDest), Vector3.up, -90.0f);
                    streetLamp.transform.Rotate(Vector3.up, -angle);
                    // TODO MOVE STREET LAMP BACK - streetLamp.transform.Translate(Vector3.back * streetLightAwayFromRoad, Space.Self);
                    distanceAlongNode = distanceAlongNode + streetLightDistanceApart;
                }
                nextStreetLightDistance = distanceAlongNode - totalNodeToNextNodeDistance;
            }
        }
    }
}

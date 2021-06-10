using System.Collections.Generic;
using UnityEngine;

public class RoadWay : MonoBehaviour
{
    public int speedLimit = 50;
    public List<RoadNode> nodes = new List<RoadNode>();
    private const float debugSphereSize = 0.25f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentNode = nodes[i].transform.position;
            Vector3 previousNode = Vector3.zero;
            Vector3 lastNode = Vector3.zero;
            if (i > 0)
            {
                previousNode = nodes[i - 1].transform.position;
            }
            else if (i == 0 && nodes.Count > 1)
            {
                currentNode = lastNode;
            }
            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, debugSphereSize);
        }
    }

    public double GetDistanceUntilDensityMeasurePointInKM()
    {
        double distance = 0;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (i == 0)
            {
                if (nodes[i].GetComponent<DensityMeasurePoint>() != null)
                {
                    break;
                }
                continue;
            }
            distance += Vector3.Distance(nodes[i - 1].transform.position, nodes[i].transform.position);
            if (nodes[i].GetComponent<DensityMeasurePoint>() != null)
            {
                break;
            }
        }
        return distance / 1000.0;
    }

    public VehiclePath ToDirectVehiclePath()
    {
        List<Transform> transforms = new List<Transform>();
        foreach(RoadNode roadNode in nodes)
        {
            transforms.Add(roadNode.transform);
        }
        return new VehiclePath(transforms);
    }

    /// <summary>
    /// Get the path distance of the way.
    /// </summary>
    /// <returns>The distance in Unity units (meters).</returns>
    public float GetDistance()
    {
        float totalDistance = 0f;
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(nodes[i].transform.position, nodes[i + 1].transform.position);
        }
        return totalDistance;
    }
}

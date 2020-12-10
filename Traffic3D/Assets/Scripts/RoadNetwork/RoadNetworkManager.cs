using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadNetworkManager
{

    private static RoadNetworkManager instance;

    public static RoadNetworkManager GetInstance()
    {
        if (instance == null)
        {
            instance = new RoadNetworkManager();
        }
        return instance;
    }

    private List<RoadNode> nodes;
    private List<RoadWay> ways;
    private const int maxInvalidPaths = 1000;

    public RoadNetworkManager()
    {
        Reload();
    }

    public void Reload()
    {
        nodes = new List<RoadNode>(GameObject.FindObjectsOfType<RoadNode>());
        ways = new List<RoadWay>(GameObject.FindObjectsOfType<RoadWay>());
    }

    public List<RoadNode> GetNodes()
    {
        return nodes;
    }

    public List<RoadWay> GetWays()
    {
        return ways;
    }

    public RoadNode GetRandomStartNode()
    {
        List<RoadNode> startNodes = nodes.Where(node => node.startNode).ToList();
        if (startNodes.Count == 0)
        {
            throw new System.Exception("There are no start nodes to spawn vehicles.");
        }
        return startNodes[UnityEngine.Random.Range(0, startNodes.Count)];
    }

    public RoadNode GetRandomEndNode()
    {
        RoadWay roadWay = ways[UnityEngine.Random.Range(0, ways.Count)];
        return roadWay.nodes[roadWay.nodes.Count - 1];
    }

    public List<RoadWay> GetRoadWaysFromNode(RoadNode roadNode)
    {
        return ways.Where(way => way.nodes.Contains(roadNode)).ToList();
    }

    public List<RoadNode> GetRoadNodeNeighbours(RoadNode roadNode)
    {
        List<RoadWay> roadWaysWithNode = GetRoadWaysFromNode(roadNode);
        List<RoadNode> roadNodesNeighbours = new List<RoadNode>();
        foreach (RoadWay roadWay in roadWaysWithNode)
        {
            int indexOfRoadNode = roadWay.nodes.IndexOf(roadNode);
            if (indexOfRoadNode < roadWay.nodes.Count - 1)
            {
                roadNodesNeighbours.Add(roadWay.nodes[indexOfRoadNode + 1]);
            }
        }
        return roadNodesNeighbours;
    }

    public VehiclePath GetRandomVehiclePath(RoadNode startRoadNode)
    {
        return GetVehiclePath(startRoadNode, GetRandomEndNode());
    }

    public VehiclePath GetValidVehiclePath(RoadNode startNode)
    {
        VehiclePath vehiclePath = null;
        int errorCount = 0;
        while (vehiclePath == null)
        {
            errorCount++;
            vehiclePath = GetRandomVehiclePath(startNode);
            if (errorCount > maxInvalidPaths)
            {
                try
                {
                    throw new System.Exception("Unable to get Valid Vehicle Path after " + maxInvalidPaths + " attempts");
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        }
        return vehiclePath;
    }

    public VehiclePath GetVehiclePath(RoadNode startNode, RoadNode endNode)
    {
        List<RoadNodePathFindInfo> openNodes = new List<RoadNodePathFindInfo>();
        List<RoadNodePathFindInfo> closedNodes = new List<RoadNodePathFindInfo>();
        RoadNodePathFindInfo endNodeInfo = null;
        RoadNodePathFindInfo startNodeInfo = new RoadNodePathFindInfo(startNode);
        openNodes.Add(startNodeInfo);

        while (openNodes.Count > 0)
        {
            RoadNodePathFindInfo currentNode = openNodes.Aggregate((x, y) => x.hCost + x.gCost > y.hCost + y.gCost ? x : y);
            closedNodes.Add(currentNode);
            openNodes.Remove(currentNode);
            if (currentNode.roadNode.Equals(endNode))
            {
                endNodeInfo = currentNode;
                break;
            }
            foreach (RoadNode neighbourRoadNode in GetRoadNodeNeighbours(currentNode.roadNode))
            {
                if (closedNodes.Any(node => node.roadNode.Equals(neighbourRoadNode)))
                {
                    continue;
                }
                RoadNodePathFindInfo neighbourPathFindInfo = new RoadNodePathFindInfo(neighbourRoadNode);
                float costToNeighbour = currentNode.gCost + Vector3.Distance(currentNode.roadNode.transform.position, neighbourRoadNode.transform.position);
                if (costToNeighbour < neighbourPathFindInfo.gCost || !openNodes.Any(node => node.roadNode.Equals(neighbourRoadNode)))
                {
                    neighbourPathFindInfo.gCost = costToNeighbour;
                    neighbourPathFindInfo.hCost = Vector3.Distance(neighbourRoadNode.transform.position, endNode.transform.position);
                    neighbourPathFindInfo.parent = currentNode;
                    openNodes.Add(neighbourPathFindInfo);
                }
            }
        }

        if (endNodeInfo != null)
        {
            List<Transform> nodes = new List<Transform>();
            RoadNodePathFindInfo currentNode = endNodeInfo;
            while (currentNode != null)
            {
                nodes.Add(currentNode.roadNode.transform);
                currentNode = currentNode.parent;
            }
            nodes.Reverse();
            return new VehiclePath(nodes);
        }
        else
        {
            return null;
        }

    }

}

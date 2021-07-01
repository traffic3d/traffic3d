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
    private List<Road> roads;
    private Dictionary<RoadNode, List<RoadWay>> nodeRelatedRoadWays;
    private const int maxInvalidPaths = 1000;

    public RoadNetworkManager()
    {
        Reload();
    }

    public void Reload()
    {
        nodes = new List<RoadNode>(GameObject.FindObjectsOfType<RoadNode>());
        ways = new List<RoadWay>(GameObject.FindObjectsOfType<RoadWay>());
        roads = new List<Road>(GameObject.FindObjectsOfType<Road>());
        nodeRelatedRoadWays = new Dictionary<RoadNode, List<RoadWay>>();
        foreach (RoadWay way in ways)
        {
            foreach (RoadNode roadNode in way.nodes)
            {
                if (nodeRelatedRoadWays.ContainsKey(roadNode))
                {
                    nodeRelatedRoadWays[roadNode].Add(way);
                }
                else
                {
                    nodeRelatedRoadWays.Add(roadNode, new List<RoadWay> { way });
                }
            }
        }
    }

    public List<RoadNode> GetNodes()
    {
        return nodes;
    }

    public List<RoadWay> GetWays()
    {
        return ways;
    }

    public List<Road> GetRoads()
    {
        return roads;
    }

    public RoadNode GetRandomStartNode()
    {
        List<RoadNode> startNodes = nodes.Where(node => node.startNode).ToList();
        if (startNodes.Count == 0)
        {
            throw new System.Exception("There are no start nodes to spawn vehicles.");
        }
        return startNodes[RandomNumberGenerator.GetInstance().Range(0, startNodes.Count)];
    }

    public RoadNode GetRandomEndNode()
    {
        RoadWay roadWay = ways[RandomNumberGenerator.GetInstance().Range(0, ways.Count)];
        return roadWay.nodes[roadWay.nodes.Count - 1];
    }

    public List<RoadWay> GetRoadWaysFromNode(RoadNode roadNode)
    {
        return nodeRelatedRoadWays[roadNode];
    }

    public List<Road> GetRoadsFromNode(RoadNode roadNode)
    {
        return nodeRelatedRoadWays[roadNode].Select(w => GetRoadFromRoadWay(w)).Where(r => r != null).Distinct().ToList();
    }

    /// <summary>
    /// Get a list of ways (most likely with only one way) using two road nodes which are using the Road Way.
    /// </summary>
    /// <param name="roadNode">A road node attached to a way</param>
    /// <param name="nextRoadNode">The next road node along in a way</param>
    /// <returns>A list of ways but most likely will only be one way.</returns>
    public List<RoadWay> GetRoadWaysFromNodes(RoadNode roadNode, RoadNode nextRoadNodeAlong)
    {
        List<RoadWay> results = new List<RoadWay>();
        foreach (RoadWay roadWay in GetRoadWaysFromNode(roadNode))
        {
            int index = roadWay.nodes.IndexOf(roadNode);
            if (roadWay.nodes.Count <= index + 1)
            {
                continue;
            }
            if (roadWay.nodes[index + 1].Equals(nextRoadNodeAlong))
            {
                results.Add(roadWay);
            }
        }
        return results;
    }

    public List<RoadWay> GetRoadWaysFromStartOrEndNode(RoadNode roadNode)
    {
        return nodeRelatedRoadWays[roadNode].Where(way => roadNode.Equals(way.nodes.First()) || roadNode.Equals(way.nodes.Last())).ToList();
    }

    public Road GetRoadFromRoadWay(RoadWay roadWay)
    {
        return roads.Find(road => road.roadWays.Contains(roadWay));
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
                throw new System.Exception("Unable to get Valid Vehicle Path after " + maxInvalidPaths + " attempts");
            }
        }
        return vehiclePath;
    }

    public VehiclePath GetVehiclePath(RoadNode startNode, RoadNode endNode)
    {
        if (startNode == endNode)
        {
            return null;
        }
        List<RoadNodePathFindInfo> openNodes = new List<RoadNodePathFindInfo>();
        List<RoadNodePathFindInfo> closedNodes = new List<RoadNodePathFindInfo>();
        RoadNodePathFindInfo endNodeInfo = null;
        RoadNodePathFindInfo startNodeInfo = new RoadNodePathFindInfo(startNode);
        openNodes.Add(startNodeInfo);

        while (openNodes.Count > 0)
        {
            RoadNodePathFindInfo currentNode = openNodes.Aggregate((x, y) => x.hCost + x.gCost < y.hCost + y.gCost ? x : y);
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

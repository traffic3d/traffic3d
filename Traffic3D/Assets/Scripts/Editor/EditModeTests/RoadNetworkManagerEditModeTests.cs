using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class RoadNetworkManagerEditModeTests
{

    [SetUp]
    public void SetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = new Vector3(0, -1, 0);
    }

    [Test]
    public void RoadNetworkReloadTest()
    {
        RoadNetworkManager.GetInstance().Reload();
        Assert.AreEqual(0, RoadNetworkManager.GetInstance().GetNodes().Count);
        Assert.AreEqual(0, RoadNetworkManager.GetInstance().GetWays().Count);
        RoadNode node1 = new GameObject("Node1").AddComponent<RoadNode>();
        RoadNode node2 = new GameObject("Node2").AddComponent<RoadNode>();
        RoadWay roadWay1 = new GameObject("RoadWay1").AddComponent<RoadWay>();
        roadWay1.nodes.Add(node1);
        roadWay1.nodes.Add(node2);
        RoadNetworkManager.GetInstance().Reload();
        Assert.AreEqual(2, RoadNetworkManager.GetInstance().GetNodes().Count);
        Assert.AreEqual(1, RoadNetworkManager.GetInstance().GetWays().Count);
    }

    [Test]
    public void RoadNetworkSimplePathFindTest()
    {
        RoadNode node1 = new GameObject("Node1").AddComponent<RoadNode>();
        node1.transform.position = new Vector3(0, 1, 0);
        node1.startNode = true;
        RoadNode node2 = new GameObject("Node2").AddComponent<RoadNode>();
        node2.transform.position = new Vector3(1, 1, 0);
        RoadNode node3 = new GameObject("Node3").AddComponent<RoadNode>();
        node3.transform.position = new Vector3(2, 1, 0);
        RoadNode node4 = new GameObject("Node4").AddComponent<RoadNode>();
        node4.transform.position = new Vector3(1, 1, 1);
        RoadNode node5 = new GameObject("Node5").AddComponent<RoadNode>();
        node5.transform.position = new Vector3(1, 1, -1);
        RoadWay roadWay1 = new GameObject("RoadWay1").AddComponent<RoadWay>();
        roadWay1.nodes.Add(node1);
        roadWay1.nodes.Add(node2);
        roadWay1.nodes.Add(node3);
        RoadWay roadWay2 = new GameObject("RoadWay2").AddComponent<RoadWay>();
        roadWay2.nodes.Add(node1);
        roadWay2.nodes.Add(node4);
        roadWay2.nodes.Add(node3);
        RoadWay roadWay3 = new GameObject("RoadWay3").AddComponent<RoadWay>();
        roadWay3.nodes.Add(node1);
        roadWay3.nodes.Add(node5);
        roadWay3.nodes.Add(node3);
        RoadNetworkManager.GetInstance().Reload();
        VehiclePath vehiclePath = RoadNetworkManager.GetInstance().GetVehiclePath(node1, node3);
        Assert.NotNull(vehiclePath);
        // Path finding will always pick roadWay1 as its the shortest route
        Assert.AreEqual(roadWay1.nodes.Count, vehiclePath.nodes.Count);
        for (int i = 0; i < vehiclePath.nodes.Count; i++)
        {
            Assert.AreEqual(roadWay1.nodes[i].transform, vehiclePath.nodes[i]);
        }
    }
}

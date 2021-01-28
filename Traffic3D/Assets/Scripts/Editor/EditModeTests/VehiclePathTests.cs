using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

[Category("Tests")]
public class VehiclePathTests
{

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    //check if number of paths created and added to vehicle factory is correct (before any roads are merged)
    [Test]
    public void CornerAngleDifferenceTest()
    {
        RoadNode node1 = new GameObject("Node1").AddComponent<RoadNode>();
        node1.transform.position = new Vector3(0, 1, 0);
        node1.startNode = true;
        RoadNode node2 = new GameObject("Node2").AddComponent<RoadNode>();
        node2.transform.position = new Vector3(1, 1, 0);
        node1.transform.LookAt(node2.transform);
        RoadNode node3 = new GameObject("Node3").AddComponent<RoadNode>();
        node3.transform.position = new Vector3(1, 1, 1);
        RoadWay roadWay = new GameObject("RoadWay").AddComponent<RoadWay>();
        roadWay.nodes.Add(node1);
        roadWay.nodes.Add(node2);
        roadWay.nodes.Add(node3);
        VehiclePath vehiclePath = roadWay.ToDirectVehiclePath();
        // Roadway should be a length of 2 and have an angle difference of (around) 45 degrees.
        Assert.AreEqual(45, vehiclePath.GetDirectionDifferenceToRoadAheadByDistanceMeasured(node1.transform, node1.transform, 2, false));
    }

    [Test]
    public void StraightLineAngleDifferenceTest()
    {
        RoadNode node1 = new GameObject("Node1").AddComponent<RoadNode>();
        node1.transform.position = new Vector3(0, 1, 0);
        node1.startNode = true;
        RoadNode node2 = new GameObject("Node2").AddComponent<RoadNode>();
        node2.transform.position = new Vector3(1, 1, 0);
        node1.transform.LookAt(node2.transform);
        RoadNode node3 = new GameObject("Node3").AddComponent<RoadNode>();
        node3.transform.position = new Vector3(2, 1, 0);
        RoadWay roadWay = new GameObject("RoadWay").AddComponent<RoadWay>();
        roadWay.nodes.Add(node1);
        roadWay.nodes.Add(node2);
        roadWay.nodes.Add(node3);
        VehiclePath vehiclePath = roadWay.ToDirectVehiclePath();
        // Roadway should be a length of 2 and have an angle difference of (around) 0 degrees.
        Assert.AreEqual(0, vehiclePath.GetDirectionDifferenceToRoadAheadByDistanceMeasured(node1.transform, node1.transform, 2, false));
    }

}


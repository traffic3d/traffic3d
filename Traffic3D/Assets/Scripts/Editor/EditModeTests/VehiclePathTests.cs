using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

[Category("Tests")]
public class VehiclePathTests
{

    [SetUp]
    public void SetUp()
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

    [Test]
    public void FindIntersectionTest()
    {
        RoadNode node1 = new GameObject("Node1").AddComponent<RoadNode>();
        node1.transform.position = new Vector3(-1, 1, 0);
        node1.startNode = true;
        RoadNode node2 = new GameObject("Node2").AddComponent<RoadNode>();
        node2.transform.position = new Vector3(1, 1, 0);
        node1.transform.LookAt(node2.transform);
        RoadNode node3 = new GameObject("Node3").AddComponent<RoadNode>();
        node3.transform.position = new Vector3(0, 1, 1);
        RoadNode node4 = new GameObject("Node4").AddComponent<RoadNode>();
        node4.transform.position = new Vector3(0, 1, -1);
        node3.transform.LookAt(node4.transform);
        RoadWay roadWay1 = new GameObject("RoadWay1").AddComponent<RoadWay>();
        RoadWay roadWay2 = new GameObject("RoadWay2").AddComponent<RoadWay>();
        roadWay1.nodes.Add(node1);
        roadWay1.nodes.Add(node2);
        roadWay2.nodes.Add(node3);
        roadWay2.nodes.Add(node4);
        VehiclePath vehiclePath = roadWay1.ToDirectVehiclePath();
        VehiclePath otherVehiclePath = roadWay2.ToDirectVehiclePath();
        HashSet<PathIntersectionPoint> intersections = vehiclePath.GetIntersectionPoints(otherVehiclePath);
        Assert.AreEqual(1, intersections.Count);
        PathIntersectionPoint pathIntersectionPoint = intersections.First();
        Assert.AreEqual(new Vector3(0, 1, 0), pathIntersectionPoint.intersection);
        Assert.AreEqual(1, pathIntersectionPoint.line1.distanceFromFirstToIntersection);
        Assert.AreEqual(1, pathIntersectionPoint.line1.distanceFromIntersectionToLast);
    }

    [Test]
    public void FindNoIntersectionTest()
    {
        RoadNode node1 = new GameObject("Node1").AddComponent<RoadNode>();
        node1.transform.position = new Vector3(-1, 1, 0);
        node1.startNode = true;
        RoadNode node2 = new GameObject("Node2").AddComponent<RoadNode>();
        node2.transform.position = new Vector3(1, 1, 0);
        node1.transform.LookAt(node2.transform);
        RoadNode node3 = new GameObject("Node3").AddComponent<RoadNode>();
        node3.transform.position = new Vector3(-1, 1, 1);
        RoadNode node4 = new GameObject("Node4").AddComponent<RoadNode>();
        node4.transform.position = new Vector3(1, 1, 1);
        node3.transform.LookAt(node4.transform);
        RoadWay roadWay1 = new GameObject("RoadWay1").AddComponent<RoadWay>();
        RoadWay roadWay2 = new GameObject("RoadWay2").AddComponent<RoadWay>();
        roadWay1.nodes.Add(node1);
        roadWay1.nodes.Add(node2);
        roadWay2.nodes.Add(node3);
        roadWay2.nodes.Add(node4);
        VehiclePath vehiclePath = roadWay1.ToDirectVehiclePath();
        VehiclePath otherVehiclePath = roadWay2.ToDirectVehiclePath();
        HashSet<PathIntersectionPoint> intersections = vehiclePath.GetIntersectionPoints(otherVehiclePath);
        Assert.AreEqual(0, intersections.Count);
    }

    [Test]
    public void VehiclePathHasStopLineTest()
    {
        GameObject dummyVehicle = new GameObject("DummyVehicle");
        dummyVehicle.transform.position = new Vector3(0, 1, 0);
        RoadNode node1 = new GameObject("Node1").AddComponent<RoadNode>();
        node1.transform.position = new Vector3(1, 1, 0);
        node1.startNode = true;
        RoadNode node2 = new GameObject("Node2").AddComponent<RoadNode>();
        node2.gameObject.AddComponent<StopLine>();
        node2.transform.position = new Vector3(2, 1, 0);
        node1.transform.LookAt(node2.transform);
        RoadNode node3 = new GameObject("Node3").AddComponent<RoadNode>();
        node3.transform.position = new Vector3(3, 1, 0);
        RoadWay roadWay = new GameObject("RoadWay1").AddComponent<RoadWay>();
        roadWay.nodes.Add(node1);
        roadWay.nodes.Add(node2);
        roadWay.nodes.Add(node3);
        VehiclePath vehiclePath = roadWay.ToDirectVehiclePath();
        StopLine stopLine = vehiclePath.GetNextStopLine(node1.transform);
        Assert.AreEqual(node2.gameObject, stopLine.gameObject);
        float distance = vehiclePath.GetDistanceToNextStopLine(node1.transform, dummyVehicle.transform);
        Assert.AreEqual(2, distance);
    }

}


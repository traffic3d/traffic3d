using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Linq;

[Category("Tests")]
public class DeadlockTests : MonoBehaviour
{
    private const int TIME_OUT = 60;
    private const int MAX_STOP_LINE_CHECKS = 10000;
    private const int STOP_LINE_DISTANCE = 15;
    private List<string> deadlockableRoadWayStrings;

    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SocketManager.GetInstance().SetSocket(new MockSocket());
            SceneManager.LoadScene(2);
            deadlockableRoadWayStrings = new List<string>();
            deadlockableRoadWayStrings.Add("WayTrafficLight1-2");
            deadlockableRoadWayStrings.Add("WayTrafficLight2-2");
            deadlockableRoadWayStrings.Add("WayTrafficLight3-2");
            deadlockableRoadWayStrings.Add("WayTrafficLight4-2");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    [TearDown]
    public void TearDown()
    {
        EventManager.Destroy();
    }

    public void DisableLoops()
    {
        PythonManager.GetInstance().StopAllCoroutines();
        PedestrianFactory pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));
        GameObject.FindObjectOfType<VehicleFactory>().StopAllCoroutines();
        if (pedestrianFactory != null)
        {
            pedestrianFactory.StopAllCoroutines();
        }
        RoadNetworkManager.GetInstance().Reload();
    }

    [UnityTest]
    public IEnumerator NoDeadlockTest()
    {
        yield return null;
        DisableLoops();
        VehicleFactory vehicleFactory = GameObject.FindObjectOfType<VehicleFactory>();
        GameObject vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), RoadNetworkManager.GetInstance().GetRandomStartNode());
        VehicleEngine vehicleEngine = vehicle.GetComponent<VehicleEngine>();
        yield return new WaitForFixedUpdate();
        Assert.False(vehicleEngine.IsInDeadlock());
    }

    [UnityTest]
    [Timeout(TIME_OUT * 1000 * 5)]
    public IEnumerator DeadlockTest()
    {
        yield return null;
        DisableLoops();
        List<RoadWay> deadlockableRoadWays = FindObjectsOfType<RoadWay>().Where(r => deadlockableRoadWayStrings.Contains(r.name)).ToList();
        VehicleFactory vehicleFactory = GameObject.FindObjectOfType<VehicleFactory>();
        List<VehicleEngine> vehicles = new List<VehicleEngine>();
        Assert.True(deadlockableRoadWays.Count > 0);
        foreach(RoadWay roadWay in deadlockableRoadWays)
        {
            VehicleEngine vehicleEngine = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), roadWay.nodes[0]).GetComponent<VehicleEngine>();
            vehicleEngine.SetVehiclePath(roadWay.ToDirectVehiclePath());
            vehicles.Add(vehicleEngine);
        }
        // Vehicles must be close to the stop line.
        bool vehiclesReady = false;
        float previousMaxSpeed = 50;
        for (int i = 0; i <= MAX_STOP_LINE_CHECKS; i++)
        {
            yield return new WaitForFixedUpdate();
            if (vehicles.All(v => v.maxSpeed == 0))
            {
                vehiclesReady = true;
                break;
            }
            foreach (VehicleEngine vehicle in vehicles)
            {
                if (vehicle == null)
                {
                    break;
                }
                float distance = vehicle.path.GetDistanceToNextStopLine(vehicle.currentNode, vehicle.transform);
                // Just before the stop line evaluation point.
                vehicle.maxSpeed = distance - vehicle.stopLineEvaluationDistance;
                if (distance < STOP_LINE_DISTANCE)
                {
                    previousMaxSpeed = vehicle.maxSpeed;
                    vehicle.maxSpeed = 0;
                }
            }
        }
        if (!vehiclesReady)
        {
            Assert.Fail("Unable to align vehicles for deadlock.");
        }
        foreach (VehicleEngine vehicle in vehicles)
        {
            vehicle.maxSpeed = previousMaxSpeed;
        }
        // All vehicles are at the stop line
        bool allVehiclesDeadlocked = false;
        for (int i = 0; i <= MAX_STOP_LINE_CHECKS; i++)
        {
            yield return new WaitForFixedUpdate();
            if (vehicles.All(v => v.deadlock))
            {
                allVehiclesDeadlocked = true;
                break;
            }
        }
        if (!allVehiclesDeadlocked)
        {
            Assert.Fail("Vehicles are not deadlocked.");
        }
        bool allVehiclesCompleted = false;
        // Vehicles should work out how to stop the deadlock and complete their journey.
        for (int i = 0; i <= MAX_STOP_LINE_CHECKS; i++)
        {
            yield return new WaitForFixedUpdate();
            if (vehicles.All(v => v == null))
            {
                allVehiclesCompleted = true;
                break;
            }
        }
        Assert.True(allVehiclesCompleted);
    }

}

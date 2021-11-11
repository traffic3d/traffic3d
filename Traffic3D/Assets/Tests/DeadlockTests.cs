using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Category("Tests")]
public class DeadlockTests
{
    private const int TIME_OUT = 60;
    private const int MAX_CHECKS = 10000;
    private const int STOP_LINE_DISTANCE = 15;
    private List<string> deadlockableRoadWayStrings;

    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SocketManager.GetInstance().SetSocket(new MockSocket());
            SceneManager.LoadScene("DayDemoWithoutTrafficLights");
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
        Vehicle vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), RoadNetworkManager.GetInstance().GetRandomStartNode());
        yield return new WaitForFixedUpdate();
        Assert.False(vehicle.vehicleDriver.vehicleSensors.GetSensor<DeadlockSensor>().IsInDeadlock());
    }

    [UnityTest]
    [Timeout(TIME_OUT * 1000 * 5)]
    public IEnumerator DeadlockTest()
    {
        yield return null;
        DisableLoops();
        List<RoadWay> deadlockableRoadWays = GameObject.FindObjectsOfType<RoadWay>().Where(r => deadlockableRoadWayStrings.Contains(r.name)).ToList();
        VehicleFactory vehicleFactory = GameObject.FindObjectOfType<VehicleFactory>();
        List<Vehicle> vehicles = new List<Vehicle>();
        Assert.True(deadlockableRoadWays.Count > 0);
        foreach (RoadWay roadWay in deadlockableRoadWays)
        {
            Vehicle vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), roadWay.nodes[0]);
            vehicle.vehicleDriver.vehicleNavigation.SetVehiclePath(roadWay.ToDirectVehiclePath());
            vehicles.Add(vehicle);
        }
        // Vehicles must be close to the stop line.
        bool vehiclesReady = false;
        float previousMaxSpeed = 50;
        for (int i = 0; i <= MAX_CHECKS; i++)
        {
            yield return new WaitForFixedUpdate();
            if (vehicles.All(v => v.vehicleSettings.maxSpeed == 0))
            {
                vehiclesReady = true;
                break;
            }
            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle == null)
                {
                    break;
                }
                VehicleSettings vehicleSettings = vehicle.vehicleSettings;
                float distance = vehicle.vehicleDriver.vehicleNavigation.path.GetDistanceToNextStopLine(vehicle.vehicleDriver.vehicleNavigation.currentNode, vehicle.transform);
                // Just before the stop line evaluation point.
                vehicleSettings.maxSpeed = distance - vehicleSettings.stopLineEvaluationDistance;
                if (distance < STOP_LINE_DISTANCE)
                {
                    previousMaxSpeed = vehicleSettings.maxSpeed;
                    vehicleSettings.maxSpeed = 0;
                }
            }
        }
        if (!vehiclesReady)
        {
            Assert.Fail("Unable to align vehicles for deadlock.");
        }
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.vehicleSettings.maxSpeed = previousMaxSpeed;
        }
        // All vehicles are at the stop line
        bool allVehiclesDeadlocked = false;
        for (int i = 0; i <= MAX_CHECKS; i++)
        {
            yield return new WaitForFixedUpdate();
            if (vehicles.All(v => v.vehicleDriver.vehicleSensors.GetSensor<DeadlockSensor>().deadlock))
            {
                allVehiclesDeadlocked = true;
                break;
            }
        }
        if (!allVehiclesDeadlocked)
        {
            Assert.Fail("Vehicles are not deadlocked.");
        }
        // All vehicles should have the same deadlock release list
        List<Vehicle> comparisonList = vehicles.First().vehicleDriver.vehicleSensors.GetSensor<DeadlockSensor>().GenerateVehicleReleaseList();
        Assert.NotNull(comparisonList);
        Assert.IsNotEmpty(comparisonList);
        foreach (Vehicle vehicle in vehicles)
        {
            CollectionAssert.AreEqual(comparisonList, vehicle.vehicleDriver.vehicleSensors.GetSensor<DeadlockSensor>().GenerateVehicleReleaseList());
        }
        bool allVehiclesCompleted = false;
        // Vehicles should work out how to stop the deadlock and complete their journey.
        for (int i = 0; i <= MAX_CHECKS; i++)
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

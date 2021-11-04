using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class VehicleEngineTests : CommonSceneTest
{
    public const int STOP_LIGHT_TIME = 20;
    public const int TIME_OUT_DESTROY_TIME = 60;
    public const int TIME_OUT_STOP_TIME = 60;
    public const int CHECK_ANGLE_DIFFERENCE_SPEED_TEST = 2;
    public const int SPEED_LIMIT_TEST_VALUE = 20;
    public const int SPEED_LIMIT_TEST_ALLOWABLE_EXTRA = 1;

    [UnityTest]
    public IEnumerator VehicleEngineGoTest()
    {
        DisableLoops();
        TrafficLightManager.GetInstance().SetTrafficLightsToGreen(TrafficLightManager.GetInstance().trafficLights.Select(t => t.trafficLightId).ToList());
        Vehicle vehicle = SpawnVehicle();
        yield return null;
        Assert.True(vehicle.vehicleEngine.GetEngineStatus() == VehicleEngine.EngineStatus.ACCELERATE);
    }

    [UnityTest]
    [Timeout(STOP_LIGHT_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineStopTest()
    {
        DisableLoops();
        TrafficLightManager.GetInstance().SetAllToRed();
        Vehicle vehicle = SpawnVehicle();
        yield return new WaitUntil(() => TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(vehicle.vehicleDriver.vehicleNavigation.currentNode) != null);
        Assert.True(vehicle.vehicleEngine.GetEngineStatus() == VehicleEngine.EngineStatus.STOP || vehicle.vehicleEngine.GetEngineStatus() == VehicleEngine.EngineStatus.HARD_STOP);
    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineDestroyTest()
    {
        yield return null;
        DisableLoops();
        foreach (TrafficLight trafficLight in TrafficLightManager.GetInstance().trafficLights)
        {
            trafficLight.SetColour(TrafficLight.LightColour.GREEN);
        }
        GameObject vehicle = SpawnVehicle().gameObject;
        bool carIsDestroyed = false;
        for (int i = 0; i <= TIME_OUT_DESTROY_TIME; i = i + 5)
        {
            yield return new WaitForSeconds(5);

            if (vehicle == null)
            {
                carIsDestroyed = true;
                break;
            }
        }
        Assert.True(carIsDestroyed);
    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineSpeedTest()
    {
        yield return null;
        DisableLoops();
        foreach (TrafficLight trafficLight in TrafficLightManager.GetInstance().trafficLights)
        {
            trafficLight.SetColour(TrafficLight.LightColour.GREEN);
        }
        RoadWay roadWayWithTurning = null;
        foreach (RoadWay roadWay in RoadNetworkManager.GetInstance().GetWays())
        {
            float xRange = roadWay.nodes.Select(node => node.transform.position.x).Max() - roadWay.nodes.Select(node => node.transform.position.x).Min();
            float zRange = roadWay.nodes.Select(node => node.transform.position.z).Max() - roadWay.nodes.Select(node => node.transform.position.z).Min();
            // Path has turning
            if (xRange != 0 && zRange != 0)
            {
                roadWayWithTurning = roadWay;
                break;
            }
        }
        if (roadWayWithTurning == null)
        {
            Assert.Inconclusive("Unable to test. No paths with turnings.");
        }
        Vehicle vehicle = SpawnVehicle(roadWayWithTurning.nodes[0]);
        VehicleDriver vehicleDriver = vehicle.vehicleDriver;
        GameObject vehicleObject = vehicleDriver.gameObject;
        VehiclePath vehiclePath = roadWayWithTurning.ToDirectVehiclePath();
        vehicleDriver.vehicleNavigation.SetVehiclePath(vehiclePath);
        bool carIsDestroyed = false;
        for (int i = 0; i <= TIME_OUT_DESTROY_TIME; i = i + 1)
        {
            yield return new WaitForSeconds(1);

            if (vehicleObject == null)
            {
                carIsDestroyed = true;
                break;
            }
            // While turning, speeds should be reduced.
            if (vehiclePath.GetDirectionDifferenceToRoadAheadByDistanceMeasured(vehicleDriver.vehicleNavigation.currentNode, vehicleDriver.transform, vehicle.vehicleSettings.distanceForSpeedCheck.Min(), false) > CHECK_ANGLE_DIFFERENCE_SPEED_TEST)
            {
                Assert.Greater(vehicleDriver.vehicleSettings.maxSpeed, vehicle.vehicleEngine.targetSpeed, "Turning Speed");
            }

        }
        Assert.True(carIsDestroyed);
    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineSpeedLimitTest()
    {
        yield return null;
        DisableLoops();
        foreach (TrafficLight trafficLight in TrafficLightManager.GetInstance().trafficLights)
        {
            trafficLight.SetColour(TrafficLight.LightColour.GREEN);
        }
        RoadWay longestRoadWay = RoadNetworkManager.GetInstance().GetWays().Aggregate((w1, w2) => w1.GetDistance() > w2.GetDistance() ? w1 : w2);
        int originalSpeedLimit = longestRoadWay.speedLimit;
        longestRoadWay.speedLimit = SPEED_LIMIT_TEST_VALUE;
        Vehicle vehicle = SpawnVehicle(longestRoadWay.nodes[0]);
        VehicleDriver vehicleDriver = vehicle.vehicleDriver;
        GameObject vehicleObject = vehicle.gameObject;
        VehiclePath vehiclePath = longestRoadWay.ToDirectVehiclePath();
        vehicleDriver.vehicleNavigation.SetVehiclePath(vehiclePath);
        bool carIsDestroyed = false;
        for (int i = 0; i <= TIME_OUT_DESTROY_TIME; i++)
        {
            yield return new WaitForSeconds(1);

            if (vehicleObject == null)
            {
                carIsDestroyed = true;
                break;
            }
            Assert.AreEqual(SPEED_LIMIT_TEST_VALUE, vehicle.vehicleSettings.maxSpeed, "Vehicle max speed is not the same as speed limit.");
            Assert.GreaterOrEqual(SPEED_LIMIT_TEST_VALUE + SPEED_LIMIT_TEST_ALLOWABLE_EXTRA, vehicle.vehicleEngine.currentSpeed, "Vehicle current speed is over the speed limit.");
        }
        Assert.True(carIsDestroyed);
        longestRoadWay.speedLimit = originalSpeedLimit;
    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineSetStatusTest()
    {
        yield return null;
        DisableLoops();
        Vehicle vehicle = SpawnVehicle();
        VehicleSettings vehicleSettings = vehicle.vehicleSettings;

        vehicle.vehicleEngine.SetEngineStatus(VehicleEngine.EngineStatus.ACCELERATE);

        Assert.AreEqual(0, vehicleSettings.wheelColliderFrontLeft.brakeTorque);
        Assert.AreEqual(0, vehicleSettings.wheelColliderFrontRight.brakeTorque);
        Assert.AreEqual(vehicle.vehicleEngine.currentMotorTorque, vehicleSettings.wheelColliderFrontLeft.motorTorque);
        Assert.AreEqual(vehicle.vehicleEngine.currentMotorTorque, vehicleSettings.wheelColliderFrontRight.motorTorque);

        vehicle.vehicleEngine.SetEngineStatus(VehicleEngine.EngineStatus.STOP);

        Assert.AreEqual(vehicleSettings.normalBrakeTorque, vehicleSettings.wheelColliderFrontLeft.brakeTorque);
        Assert.AreEqual(vehicleSettings.normalBrakeTorque, vehicleSettings.wheelColliderFrontRight.brakeTorque);
        Assert.AreEqual(0, vehicleSettings.wheelColliderFrontLeft.motorTorque);
        Assert.AreEqual(0, vehicleSettings.wheelColliderFrontRight.motorTorque);

        vehicle.vehicleEngine.SetEngineStatus(VehicleEngine.EngineStatus.HARD_STOP);

        Assert.AreEqual(vehicleSettings.maxBrakeTorque, vehicleSettings.wheelColliderFrontLeft.brakeTorque);
        Assert.AreEqual(vehicleSettings.maxBrakeTorque, vehicleSettings.wheelColliderFrontRight.brakeTorque);
        Assert.AreEqual(0, vehicleSettings.wheelColliderFrontLeft.motorTorque);
        Assert.AreEqual(0, vehicleSettings.wheelColliderFrontRight.motorTorque);
    }

    private new void DisableLoops()
    {
        // Optimize time by removing unneeded particles
        foreach (ParticleSystem particleSystem in GameObject.FindObjectsOfType<ParticleSystem>())
        {
            particleSystem.Stop();
        }
        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));
        trafficLightManager.enabled = false;
        trafficLightManager.StopAllCoroutines();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        vehicleFactory.StopAllCoroutines();
        PythonManager.GetInstance().StopAllCoroutines();
        PedestrianFactory pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));
        if (pedestrianFactory != null)
        {
            pedestrianFactory.StopAllCoroutines();
        }
        RoadNetworkManager.GetInstance().Reload();
    }

    private Vehicle SpawnVehicle()
    {
        return SpawnVehicle(RoadNetworkManager.GetInstance().GetRandomStartNode());
    }

    private Vehicle SpawnVehicle(RoadNode startNode)
    {
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        Vehicle vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), startNode);
        Assert.NotNull(vehicle, "Failed to spawn vehicle.");
        Assert.NotNull(vehicle.vehicleDriver, "Failed to spawn vehicle driver.");
        Assert.NotNull(vehicle.vehicleEngine, "Failed to spawn vehicle engine.");
        return vehicle;
    }

}

using NUnit.Framework;
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
    public const int CHECK_DISTANCE_AHEAD_SPEED_TEST = 5;
    public const int CHECK_ANGLE_DIFFERENCE_SPEED_TEST = 2;

    [UnityTest]
    public IEnumerator VehicleEngineGoTest()
    {
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        TrafficLightManager.GetInstance().SetTrafficLightsToGreen(TrafficLightManager.GetInstance().trafficLights.Select(t => t.trafficLightId).ToList());
        GameObject vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), RoadNetworkManager.GetInstance().GetRandomStartNode());
        VehicleEngine vehicleEngine = vehicle.GetComponent<VehicleEngine>();
        yield return null;
        Assert.True(vehicleEngine.GetEngineStatus() == VehicleEngine.EngineStatus.ACCELERATE);
    }

    [UnityTest]
    [Timeout(STOP_LIGHT_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineStopTest()
    {
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        TrafficLightManager.GetInstance().SetAllToRed();
        GameObject vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), RoadNetworkManager.GetInstance().GetRandomStartNode());
        VehicleEngine vehicleEngine = vehicle.GetComponent<VehicleEngine>();
        yield return new WaitUntil(() => TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(vehicleEngine.currentNode) != null);
        Assert.True(vehicleEngine.GetEngineStatus() == VehicleEngine.EngineStatus.STOP || vehicleEngine.GetEngineStatus() == VehicleEngine.EngineStatus.HARD_STOP);
    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineDestroyTest()
    {
        yield return null;
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        foreach (TrafficLight trafficLight in TrafficLightManager.GetInstance().trafficLights)
        {
            trafficLight.SetColour(TrafficLight.LightColour.GREEN);
        }
        GameObject vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), RoadNetworkManager.GetInstance().GetRandomStartNode());
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
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
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
        GameObject vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), roadWayWithTurning.nodes[0]);
        VehicleEngine vehicleEngine = vehicle.GetComponent<VehicleEngine>();
        VehiclePath vehiclePath = roadWayWithTurning.ToDirectVehiclePath();
        vehicleEngine.SetVehiclePath(vehiclePath);
        bool carIsDestroyed = false;
        for (int i = 0; i <= TIME_OUT_DESTROY_TIME; i = i + 1)
        {
            yield return new WaitForSeconds(1);

            if (vehicle == null)
            {
                carIsDestroyed = true;
                break;
            }
            // While turning, speeds should be reduced.
            if (vehiclePath.GetDirectionDifferenceToRoadAheadByDistanceMeasured(vehicleEngine.currentNode, vehicleEngine.transform, CHECK_DISTANCE_AHEAD_SPEED_TEST, false) > CHECK_ANGLE_DIFFERENCE_SPEED_TEST)
            {
                Assert.Greater(vehicleEngine.maxSpeed, vehicleEngine.targetSpeed, "Turning Speed");
            }

        }
        Assert.True(carIsDestroyed);
    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineSetStatusTest()
    {
        yield return null;
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        GameObject vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), RoadNetworkManager.GetInstance().GetRandomStartNode());
        VehicleEngine vehicleEngine = vehicle.GetComponent<VehicleEngine>();

        vehicleEngine.SetEngineStatus(VehicleEngine.EngineStatus.ACCELERATE);

        Assert.AreEqual(0, vehicleEngine.wheelColliderFrontLeft.brakeTorque);
        Assert.AreEqual(0, vehicleEngine.wheelColliderFrontRight.brakeTorque);
        Assert.AreEqual(vehicleEngine.currentMotorTorque, vehicleEngine.wheelColliderFrontLeft.motorTorque);
        Assert.AreEqual(vehicleEngine.currentMotorTorque, vehicleEngine.wheelColliderFrontRight.motorTorque);

        vehicleEngine.SetEngineStatus(VehicleEngine.EngineStatus.STOP);

        Assert.AreEqual(vehicleEngine.normalBrakeTorque, vehicleEngine.wheelColliderFrontLeft.brakeTorque);
        Assert.AreEqual(vehicleEngine.normalBrakeTorque, vehicleEngine.wheelColliderFrontRight.brakeTorque);
        Assert.AreEqual(0, vehicleEngine.wheelColliderFrontLeft.motorTorque);
        Assert.AreEqual(0, vehicleEngine.wheelColliderFrontRight.motorTorque);

        vehicleEngine.SetEngineStatus(VehicleEngine.EngineStatus.HARD_STOP);

        Assert.AreEqual(vehicleEngine.maxBrakeTorque, vehicleEngine.wheelColliderFrontLeft.brakeTorque);
        Assert.AreEqual(vehicleEngine.maxBrakeTorque, vehicleEngine.wheelColliderFrontRight.brakeTorque);
        Assert.AreEqual(0, vehicleEngine.wheelColliderFrontLeft.motorTorque);
        Assert.AreEqual(0, vehicleEngine.wheelColliderFrontRight.motorTorque);
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

}

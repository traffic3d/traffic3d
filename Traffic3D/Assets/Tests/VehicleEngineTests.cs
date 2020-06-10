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

    [UnityTest]
    public IEnumerator VehicleEngineGoTest()
    {
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        Rigidbody vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), vehicleFactory.GetRandomUnusedPath());
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
        Rigidbody vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), vehicleFactory.GetRandomUnusedPath());
        VehicleEngine vehicleEngine = vehicle.GetComponent<VehicleEngine>();
        yield return new WaitForSeconds(STOP_LIGHT_TIME);
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
        Rigidbody vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), vehicleFactory.GetRandomUnusedPath());
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
        Path pathWithTurning = null;
        foreach (Path path in vehicleFactory.paths)
        {
            float xRange = path.nodes.Select(node => node.position.x).Max() - path.nodes.Select(node => node.position.x).Min();
            float zRange = path.nodes.Select(node => node.position.z).Max() - path.nodes.Select(node => node.position.z).Min();
            // Path has turning
            if (xRange != 0 && zRange != 0)
            {
                pathWithTurning = path;
                break;
            }
        }
        if (pathWithTurning == null)
        {
            Assert.Inconclusive("Unable to test. No paths with turnings.");
        }
        Rigidbody vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), pathWithTurning);
        VehicleEngine vehicleEngine = vehicle.GetComponent<VehicleEngine>();
        bool carIsDestroyed = false;
        bool passedTest = false;
        for (int i = 0; i <= TIME_OUT_DESTROY_TIME; i = i + 5)
        {
            yield return new WaitForSeconds(5);

            if (vehicle == null)
            {
                carIsDestroyed = true;
                break;
            }

            if (Math.Abs(vehicleEngine.wheelColliderFrontLeft.steerAngle) > 2)
            {
                passedTest = vehicleEngine.maxSpeedTurning == vehicleEngine.targetSpeed;
            }

            if (vehicleEngine.currentNodeNumber + 1 < vehicleEngine.path.nodes.Count)
            {
                // If next node is a traffic light
                if (TrafficLightManager.GetInstance().IsStopNode(vehicleEngine.path.nodes[vehicleEngine.currentNodeNumber + 1]))
                {
                    passedTest = vehicleEngine.maxSpeedAproachingLightsLastNode == vehicleEngine.targetSpeed;
                }
            }
            if (vehicleEngine.currentNodeNumber + 2 < vehicleEngine.path.nodes.Count)
            {
                // If 2nd to next node is a traffic light
                if (TrafficLightManager.GetInstance().IsStopNode(vehicleEngine.path.nodes[vehicleEngine.currentNodeNumber + 2]))
                {
                    passedTest = vehicleEngine.maxSpeedAproachingLightsSecondLastNode == vehicleEngine.targetSpeed;
                }
            }

            if (!passedTest)
            {
                Assert.Fail("Speed test failed, current target speed: " + vehicleEngine.targetSpeed);
            }
        }
        Assert.True(passedTest && carIsDestroyed);
    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 5)]
    public IEnumerator VehicleEngineSetStatusTest()
    {
        yield return null;
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        Rigidbody vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), vehicleFactory.GetRandomUnusedPath());
        VehicleEngine vehicleEngine = vehicle.GetComponent<VehicleEngine>();

        vehicleEngine.SetEngineStatus(VehicleEngine.EngineStatus.ACCELERATE);

        Assert.AreEqual(0, vehicleEngine.wheelColliderFrontLeft.brakeTorque);
        Assert.AreEqual(0, vehicleEngine.wheelColliderFrontRight.brakeTorque);
        Assert.AreEqual(vehicleEngine.maxMotorTorque, vehicleEngine.wheelColliderFrontLeft.motorTorque);
        Assert.AreEqual(vehicleEngine.maxMotorTorque, vehicleEngine.wheelColliderFrontRight.motorTorque);

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
    }

}

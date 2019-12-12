using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Category("Tests")]
public class VehicleEngineTests
{
    public const int STOP_LIGHT_TIME = 20;
    public const int TIME_OUT_DESTROY_TIME = 60;
    public const int TIME_OUT_STOP_TIME = 60;

    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SocketManager.GetInstance().SetSocket(new MockSocket());
            SceneManager.LoadScene(0);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

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

    private void DisableLoops()
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
    }

}

using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
            // Check if car is destroyed
            if (vehicle == null)
            {
                carIsDestroyed = true;
                break;
            }
        }
        Assert.True(carIsDestroyed);
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
    }

}

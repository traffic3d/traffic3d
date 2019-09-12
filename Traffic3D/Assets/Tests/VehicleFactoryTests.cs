using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class VehicleFactoryTests
{
    public const int AMOUNT_LEFT_FOR_PASS = 2;

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
    public IEnumerator VehicleFactoryPathTest()
    {
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        foreach (Path path in vehicleFactory.paths)
        {
            vehicleFactory.SpawnVehicle(vehicleFactory.GetRandomVehicle(), path);
        }
        Assert.AreEqual(null, vehicleFactory.GetRandomUnusedPath());
        yield return null;
    }

    [UnityTest]
    public IEnumerator VehicleFactoryVehicleTest()
    {
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        foreach (Rigidbody vehicle in vehicleFactory.vehicles)
        {
            vehicleFactory.SpawnVehicle(vehicle, vehicleFactory.GetRandomPath());
        }
        Assert.AreEqual(vehicleFactory.vehicles.Count, vehicleFactory.currentVehicles.Count);
        yield return null;
    }
}

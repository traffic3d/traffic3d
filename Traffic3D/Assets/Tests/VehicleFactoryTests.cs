using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class VehicleFactoryTests
{

    public const int TEST_TIME = 500;
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

        foreach(Path path in vehicleFactory.paths) {

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


    [UnityTest]
    [Timeout((TEST_TIME * 1000 * 2))]
    public IEnumerator VehicleFactoryGeneralSpawnTest()
    {

        // Optimize time by removing unneeded particles
        foreach (ParticleSystem particleSystem in GameObject.FindObjectsOfType<ParticleSystem>())
        {
            particleSystem.Stop();
        }

        List<Type> engineTypeList = new List<Type>();
        engineTypeList.Add(typeof(VehicleEngine8));
        engineTypeList.Add(typeof(VehicleEngine3));
        engineTypeList.Add(typeof(VehicleEngine4));
        engineTypeList.Add(typeof(VehicleEngine5));
        engineTypeList.Add(typeof(VehicleEngine6));
        engineTypeList.Add(typeof(VehicleEngine7));
        engineTypeList.Add(typeof(VehicleEngine2));
        engineTypeList.Add(typeof(VehicleEngine1));

        // Ensure there are no cars currently spawned in
        foreach (Type engineType in engineTypeList)
        {
            Assert.AreEqual(0, GameObject.FindObjectsOfType(engineType).Length);
        }

        bool mostCarsSpawned = false;
        for (int i = 0; i < TEST_TIME; i++)
        {

            yield return new WaitForSeconds(1);

            engineTypeList.RemoveAll(engineType => GameObject.FindObjectsOfType(engineType).Length != 0);

            if (engineTypeList.Count <= AMOUNT_LEFT_FOR_PASS)
            {
                mostCarsSpawned = true;
                break;
            }

        }

        Assert.True(mostCarsSpawned);

    }

}

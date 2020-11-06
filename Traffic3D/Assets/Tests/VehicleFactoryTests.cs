using NUnit.Framework;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using System.Collections.Generic;

[Category("Tests")]
public class VehicleFactoryTests : CommonSceneTest
{
    public const int NUMBER_TEST_VEHICLES = 7;

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
        foreach (VehicleFactory.VehicleProbability vehicleProbability in vehicleFactory.vehicleProbabilities)
        {
            vehicleFactory.SpawnVehicle(vehicleProbability.vehicle, vehicleFactory.GetRandomPath());
        }
        Assert.AreEqual(vehicleFactory.vehicleProbabilities.Count, vehicleFactory.currentVehicles.Count);
        yield return null;
    }

    [UnityTest]
    public IEnumerator VehicleFactoryDefaultVehicleProbabilityTest()
    {
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        vehicleFactory.StopAllCoroutines();
        List<Rigidbody> vehicleList = new List<Rigidbody>();
        for (int i = 0; i < NUMBER_TEST_VEHICLES; i++)
        {
            vehicleList.Add(new GameObject("PretendVehicle" + i).AddComponent<Rigidbody>());
        }
        vehicleFactory.SetDefaultVehicleProbabilities(vehicleList);
        int vehicleCount = 0;
        foreach (Rigidbody vehicle in vehicleList)
        {
            Assert.True(vehicleFactory.vehicleProbabilities.Any(o => o.vehicle.Equals(vehicle)));
            vehicleCount++;
        }
        Assert.AreEqual(vehicleCount, vehicleFactory.vehicleProbabilities.Count);
        float vehicleProbabilityValue = 1f / vehicleCount;
        Assert.True(vehicleFactory.vehicleProbabilities.All(o => o.probability == vehicleProbabilityValue));
        yield return null;
    }
}

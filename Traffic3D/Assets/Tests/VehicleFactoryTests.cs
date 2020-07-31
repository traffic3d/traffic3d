using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class VehicleFactoryTests : CommonSceneTest
{
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
}

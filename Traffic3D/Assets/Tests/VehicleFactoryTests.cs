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
    public IEnumerator VehicleFactoryVehicleTest()
    {
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        int availableVehiclesAndWays = Mathf.Min(RoadNetworkManager.GetInstance().GetWays().Count, vehicleFactory.vehicleProbabilities.Count);
        for (int i = 0; i < availableVehiclesAndWays; i++)
        {
            vehicleFactory.SpawnVehicle(vehicleFactory.vehicleProbabilities[i].vehicle, RoadNetworkManager.GetInstance().GetWays()[i].nodes[0]);
        }
        Assert.AreEqual(availableVehiclesAndWays, vehicleFactory.currentVehicles.Count);
        yield return null;
    }

    [UnityTest]
    public IEnumerator VehicleFactoryDefaultVehicleProbabilityTest()
    {
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        vehicleFactory.StopAllCoroutines();
        string[] assets = Directory.GetFiles("Assets/Vehicles", "*.prefab");
        GameObject templateVehicle = AssetDatabase.LoadAssetAtPath<GameObject>(assets[0]);
        List<Vehicle> vehicleList = new List<Vehicle>();
        for (int i = 0; i < NUMBER_TEST_VEHICLES; i++)
        {
            vehicleList.Add(templateVehicle.GetComponent<Vehicle>());
        }
        vehicleFactory.SetDefaultVehicleProbabilities(vehicleList);
        int vehicleCount = 0;
        foreach (Vehicle vehicle in vehicleList)
        {
            Assert.True(vehicleFactory.vehicleProbabilities.Any(o => o.vehicle.Equals(vehicle)));
            vehicleCount++;
        }
        Assert.AreEqual(vehicleCount, vehicleFactory.vehicleProbabilities.Count);
        float vehicleProbabilityValue = 1f / vehicleCount;
        Assert.True(vehicleFactory.vehicleProbabilities.All(o => o.probability == vehicleProbabilityValue));
        yield return null;
    }

    [UnityTest]
    public IEnumerator VehicleFactoryVehicleSpawnCheckTest()
    {
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        foreach(Vehicle currentVehicle in vehicleFactory.currentVehicles)
        {
            GameObject.DestroyImmediate(currentVehicle.gameObject);
        }
        vehicleFactory.CleanVehicles();
        RoadNode node = RoadNetworkManager.GetInstance().GetNodes().Find(n => n.startNode);
        Assert.IsTrue(node.CanSpawnVehicle(vehicleFactory.vehicleProbabilities[0].vehicle));
        Vehicle vehicle = vehicleFactory.SpawnVehicle(vehicleFactory.vehicleProbabilities[0].vehicle, node);
        Assert.NotNull(vehicle);
        yield return null;
        Assert.IsFalse(node.CanSpawnVehicle(vehicleFactory.vehicleProbabilities[0].vehicle));
        Vehicle vehicle2 = vehicleFactory.SpawnVehicle(vehicleFactory.vehicleProbabilities[0].vehicle, node);
        Assert.IsNull(vehicle2);
    }
}

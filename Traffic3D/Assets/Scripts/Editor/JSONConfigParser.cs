using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class JSONConfigParser
{
    private static JSONConfig config;

    public static void Parse(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Cannot find inputted JSON config file at: " + filePath);
        }
        string fileData = File.ReadAllText(filePath);
        config = JsonUtility.FromJson<JSONConfig>(fileData);
        EditorApplication.playModeStateChanged += UpdateObjects;
    }

    public static void UpdateObjects(PlayModeStateChange playModeStateChange)
    {
        Debug.Log("Updating Objects");
        SetUpVehicleFactory(GameObject.FindObjectOfType<VehicleFactory>());
    }

    public static void SetUpVehicleFactory(VehicleFactory vehicleFactory)
    {
        Debug.Log("Object is: ");
        Debug.Log(vehicleFactory);
        vehicleFactory.highRangeRespawnTime = config.vehicleFactoryConfig.highRangeRespawnTime;
        vehicleFactory.lowRangeRespawnTime = config.vehicleFactoryConfig.lowRangeRespawnTime;
        vehicleFactory.maximumVehicleCount = config.vehicleFactoryConfig.maximumVehicleCount;
        vehicleFactory.slowDownVehicleRateAt = config.vehicleFactoryConfig.slowDownVehicleRateAt;
        vehicleFactory.timeOfStartInvisibility = config.vehicleFactoryConfig.timeOfStartInvisibility;
    }

    public class JSONConfig
    {
        public VehicleFactoryConfig vehicleFactoryConfig;
    }

    public class VehicleFactoryConfig
    {
        public float highRangeRespawnTime;
        public float lowRangeRespawnTime;
        public float maximumVehicleCount;
        public float slowDownVehicleRateAt;
        public float timeOfStartInvisibility;
    }
}

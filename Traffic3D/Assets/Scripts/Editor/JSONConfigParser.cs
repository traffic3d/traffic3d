using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JSONConfigParser
{
    private static JSONConfig config;

    public static JSONConfig GetConfig()
    {
        return config;
    }

    public static void SetConfig(JSONConfig input)
    {
        config = input;
        if (config == null)
        {
            EditorSceneManager.sceneOpened -= UpdateObjects;
        }
    }

    public static void Parse(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Cannot find inputted JSON config file at: " + filePath);
        }
        string fileData = File.ReadAllText(filePath);
        config = JsonUtility.FromJson<JSONConfig>(fileData);
        EditorSceneManager.sceneOpened += UpdateObjects;
    }

    public static void UpdateObjects(Scene scene, OpenSceneMode mode)
    {
        SetUpVehicleFactory((VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory)));
    }

    public static void SetUpVehicleFactory(VehicleFactory vehicleFactory)
    {
        vehicleFactory.highRangeRespawnTime = config.vehicleFactoryConfig.highRangeRespawnTime;
        vehicleFactory.lowRangeRespawnTime = config.vehicleFactoryConfig.lowRangeRespawnTime;
        vehicleFactory.maximumVehicleCount = config.vehicleFactoryConfig.maximumVehicleCount;
        vehicleFactory.slowDownVehicleRateAt = config.vehicleFactoryConfig.slowDownVehicleRateAt;
        vehicleFactory.timeOfStartInvisibility = config.vehicleFactoryConfig.timeOfStartInvisibility;
    }

    [System.Serializable]
    public class JSONConfig
    {
        public VehicleFactoryConfig vehicleFactoryConfig;
    }

    [System.Serializable]
    public class VehicleFactoryConfig
    {
        public float highRangeRespawnTime;
        public float lowRangeRespawnTime;
        public float maximumVehicleCount;
        public float slowDownVehicleRateAt;
        public float timeOfStartInvisibility;
    }
}

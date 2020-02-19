using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JSONConfigParser
{
    private static JSONConfig config;
    private static int currentBuildScene = -1;

    public static JSONConfig GetConfig()
    {
        return config;
    }

    public static void SetConfig(JSONConfig input)
    {
        config = input;
        if (config == null)
        {
            EditorSceneManager.sceneOpened -= EditorSceneManagerUpdateObjects;
            SceneManager.sceneLoaded -= SceneManagerUpdateObjects;
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
        EditorSceneManager.sceneOpened += EditorSceneManagerUpdateObjects;
        SceneManager.sceneLoaded += SceneManagerUpdateObjects;
    }

    public static void SceneManagerUpdateObjects(Scene scene, LoadSceneMode mode)
    {
        UpdateObjects(scene.buildIndex);
    }

    public static void EditorSceneManagerUpdateObjects(Scene scene, OpenSceneMode mode)
    {
        UpdateObjects(scene.buildIndex);
    }

    public static void UpdateObjects(int scene)
    {
        if (currentBuildScene == scene)
        {
            return;
        }
        else
        {
            currentBuildScene = scene;
        }
        SetUpVehicleFactory((VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory)));
    }

    public static void SetUpVehicleFactory(VehicleFactory vehicleFactory)
    {
        vehicleFactory.highRangeRespawnTime = config.vehicleFactoryConfig.highRangeRespawnTime;
        vehicleFactory.lowRangeRespawnTime = config.vehicleFactoryConfig.lowRangeRespawnTime;
        vehicleFactory.maximumVehicleCount = config.vehicleFactoryConfig.maximumVehicleCount;
        vehicleFactory.slowDownVehicleRateAt = config.vehicleFactoryConfig.slowDownVehicleRateAt;
        vehicleFactory.timeOfStartInvisibility = config.vehicleFactoryConfig.timeOfStartInvisibility;
        vehicleFactory.vehicleProbabilities.Clear();

        foreach (VehicleProbabilityConfig vehicleProbabilityConfig in config.vehicleFactoryConfig.vehicleProbabilities)
        {
            VehicleFactory.VehicleProbability vehicleProbability = new VehicleFactory.VehicleProbability();
            vehicleProbability.vehicle = (Rigidbody)AssetDatabase.LoadAssetAtPath(vehicleProbabilityConfig.vehiclePath, typeof(Rigidbody));
            vehicleProbability.probability = vehicleProbabilityConfig.probability;
            vehicleFactory.vehicleProbabilities.Add(vehicleProbability);
        }
    }

    public static void SetUpSumo(SumoManager sumoManager)
    {
        if (sumoManager == null)
        {
            return;
        }
        sumoManager.sumoControlSettings = config.sumoConfig.sumoControlSettings;
    }

    [System.Serializable]
    public class JSONConfig
    {
        public VehicleFactoryConfig vehicleFactoryConfig;
        public SumoConfig sumoConfig;
    }

    [System.Serializable]
    public class SumoConfig
    {
        public List<SumoManager.SumoLinkControlPointObject> sumoControlSettings;
    }

    [System.Serializable]
    public class VehicleFactoryConfig
    {
        public float highRangeRespawnTime;
        public float lowRangeRespawnTime;
        public float maximumVehicleCount;
        public float slowDownVehicleRateAt;
        public float timeOfStartInvisibility;
        public List<VehicleProbabilityConfig> vehicleProbabilities;
    }

    [System.Serializable]
    public class VehicleProbabilityConfig
    {
        public string vehiclePath;
        public float probability;
    }
}

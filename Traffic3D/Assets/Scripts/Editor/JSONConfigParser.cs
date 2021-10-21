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
        SetUpPedestrianFactory((PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory)));
        SetUpSumo((SumoManager)GameObject.FindObjectOfType(typeof(SumoManager)));
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
            vehicleProbability.vehicle = ((GameObject)AssetDatabase.LoadAssetAtPath(vehicleProbabilityConfig.vehiclePath, typeof(GameObject))).GetComponent<Vehicle>();
            vehicleProbability.probability = vehicleProbabilityConfig.probability;
            vehicleFactory.vehicleProbabilities.Add(vehicleProbability);
        }
    }

    public static void SetUpPedestrianFactory(PedestrianFactory pedestrianFactory)
    {
        // Pedestrian Factory is not supported in SUMO
        if (pedestrianFactory == null)
        {
            return;
        }
        pedestrianFactory.highRangeRespawnTime = config.pedestrianFactoryConfig.highRangeRespawnTime;
        pedestrianFactory.lowRangeRespawnTime = config.pedestrianFactoryConfig.lowRangeRespawnTime;
        pedestrianFactory.maximumPedestrianCount = config.pedestrianFactoryConfig.maximumPedestrianCount;
        pedestrianFactory.pedestrianProbabilities.Clear();

        foreach (PedestrianProbabilityConfig pedestrianProbabilityConfig in config.pedestrianFactoryConfig.pedestrianProbabilities)
        {
            PedestrianFactory.PedestrianProbability pedestrianProbability = new PedestrianFactory.PedestrianProbability();
            pedestrianProbability.pedestrian = (Pedestrian)AssetDatabase.LoadAssetAtPath(pedestrianProbabilityConfig.pedestrianPath, typeof(Pedestrian));
            pedestrianProbability.probability = pedestrianProbabilityConfig.probability;
            pedestrianFactory.pedestrianProbabilities.Add(pedestrianProbability);
        }
    }

    public static void SetUpSumo(SumoManager sumoManager)
    {
        if (sumoManager == null)
        {
            return;
        }
        sumoManager.sumoControlSettings = config.sumoConfig.sumoControlSettings;
        sumoManager.ip = config.sumoConfig.ip;
        sumoManager.port = config.sumoConfig.port;
    }

    [System.Serializable]
    public class JSONConfig
    {
        public VehicleFactoryConfig vehicleFactoryConfig;
        public PedestrianFactoryConfig pedestrianFactoryConfig;
        public SumoConfig sumoConfig;
    }

    [System.Serializable]
    public class SumoConfig
    {
        public string ip;
        public int port;
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

    [System.Serializable]
    public class PedestrianFactoryConfig
    {
        public float highRangeRespawnTime;
        public float lowRangeRespawnTime;
        public int maximumPedestrianCount;
        public List<PedestrianProbabilityConfig> pedestrianProbabilities;
    }

    [System.Serializable]
    public class PedestrianProbabilityConfig
    {
        public string pedestrianPath;
        public float probability;
    }
}

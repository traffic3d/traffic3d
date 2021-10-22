using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleFactory : MonoBehaviour
{
    public float highRangeRespawnTime = 5;
    public float lowRangeRespawnTime = 3;
    public float maximumVehicleCount = 8;
    public float slowDownVehicleRateAt = 6;
    public float timeOfStartInvisibility = 1;
    public bool isLeftHandDrive = true;
    public List<VehicleProbability> vehicleProbabilities;
    public List<Vehicle> currentVehicles = new List<Vehicle>();

    void Start()
    {
        // This seed is needed for running benchmarks so if its removed
        // add an if statement to add the seed back with the following condition:
        // Settings.IsBenchmark()
        if (vehicleProbabilities.Count == 0)
        {
            throw new System.Exception("No vehicles to spawn.");
        }
        float probabilitySum = vehicleProbabilities.Select(p => p.probability).Sum();
        if (probabilitySum < 0.999999 || probabilitySum > 1.000001)
        {
            throw new System.Exception("Vehicle Probabilities do not sum to 100%");
        }
        if (SumoManager.GetInstance() == null)
        {
            StartCoroutine(GenerateVehicle());
        }
    }

    /// <summary>
    /// A loop that generates vehicles in a random way.
    /// </summary>
    IEnumerator GenerateVehicle()
    {
        while (true)
        {
            yield return new WaitForSeconds(RandomNumberGenerator.GetInstance().Range(lowRangeRespawnTime, highRangeRespawnTime));
            CleanVehicles();
            if (currentVehicles.Count < RandomNumberGenerator.GetInstance().Range(slowDownVehicleRateAt, maximumVehicleCount))
            {
                RoadNode roadNode = RoadNetworkManager.GetInstance().GetRandomStartNode();
                if (roadNode != null)
                {
                    SpawnVehicle(GetRandomVehicle(), roadNode);
                }
            }
        }
    }

    /// <summary>
    /// Remove vehicles that are no longer being used in the currentVehicles dictionary.
    /// </summary>
    public void CleanVehicles()
    {
        // Remove null vehicles from current list
        foreach (Vehicle key in currentVehicles.Where(vehicle => vehicle == null).ToList())
        {
            currentVehicles.Remove(key);
        }
        // Remove unknown vehicles from scene (Not currently in the list)
        foreach (Vehicle vehicle in GameObject.FindObjectsOfType<Vehicle>())
        {
            if (!currentVehicles.Contains(vehicle))
            {
                Destroy(vehicle.gameObject);
            }
        }
    }

    /// <summary>
    /// Create the vehicle with the path inputted.
    /// </summary>
    /// <param name="vehicle">The vehicle to create.</param>
    /// <param name="startRoadNode">The node the vehicle starts on.</param>
    /// <returns>The created vehicle.</returns>
    public Vehicle SpawnVehicle(Vehicle vehicle, RoadNode startRoadNode)
    {
        if (vehicle == null || !startRoadNode.CanSpawnVehicle(vehicle))
        {
            return null;
        }
        Vehicle spawnedVehicle = Instantiate(vehicle.gameObject, startRoadNode.transform.position, startRoadNode.transform.rotation).GetComponent<Vehicle>();
        try
        {
            TemporarilyHideVehicle(spawnedVehicle, timeOfStartInvisibility);
            spawnedVehicle.vehicleDriver.vehicleNavigation.GenerateVehiclePath(startRoadNode);
            currentVehicles.Add(spawnedVehicle);
            EventManager.GetInstance().CallVehicleSpawnEvent(this, new VehicleEventArgs(spawnedVehicle));
        }
        catch (Exception e)
        {
            Destroy(spawnedVehicle);
            Debug.LogError("Unable to spawn vehicle: " + e.Message);
        }
        return spawnedVehicle;
    }

    /// <summary>
    /// Gets a random vehicle from the vehicles list that is inputted by the user in Unity.
    /// </summary>
    /// <returns>The vehicle template.</returns>
    public Vehicle GetRandomVehicle()
    {
        float finalProbability = RandomNumberGenerator.GetInstance().NextFloat();
        float cumulativeProbability = 0.0F;
        foreach (VehicleProbability vehicleProbability in vehicleProbabilities)
        {
            cumulativeProbability += vehicleProbability.probability;
            if (finalProbability <= cumulativeProbability)
            {
                return vehicleProbability.vehicle;
            }
        }

        return vehicleProbabilities.Aggregate((highest, next) => highest.probability > next.probability ? highest : next).vehicle;
    }

    /// <summary>
    /// Sets a uniform probability distribution for a list of default vehicles within the vehicleProbabilities list.
    /// Please note that all vehicles already in the probability list will be removed.
    /// </summary>
    /// <param name="defaultVehicles">A list of default vehicles to set.</param>
    public void SetDefaultVehicleProbabilities(List<Vehicle> defaultVehicles)
    {
        vehicleProbabilities = new List<VehicleProbability>();
        if (defaultVehicles.Count == 0)
        {
            return;
        }
        float totalVehicles = defaultVehicles.Count;
        float vehicleProbabilityValue = 1f / totalVehicles;
        foreach (Vehicle vehicle in defaultVehicles)
        {
            VehicleProbability vehicleProbability = new VehicleProbability();
            vehicleProbability.vehicle = vehicle;
            vehicleProbability.probability = vehicleProbabilityValue;
            vehicleProbabilities.Add(vehicleProbability);
        }
    }

    /// <summary>
    /// Temporarily hide a selected vehicle. 
    /// Mainly used at the start of a vehicle's life so it looks like they spawn in smoother.
    /// </summary>
    /// <param name="vehicle">The vehicle to temporarily hide.</param>
    /// <param name="hideForSeconds">The amount of seconds as a float to hide the vehicle for.</param>
    public void TemporarilyHideVehicle(Vehicle vehicle, float hideForSeconds)
    {
        vehicle.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = false);
        StartCoroutine(WaitAndShowVehicle(vehicle, hideForSeconds));
    }

    IEnumerator WaitAndShowVehicle(Vehicle vehicle, float hideForSeconds)
    {
        yield return new WaitForSeconds(hideForSeconds);
        if (vehicle != null)
        {
            vehicle.gameObject.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = true);
        }
    }

    [System.Serializable]
    public class VehicleProbability
    {
        public Vehicle vehicle;
        public float probability;
    }

}

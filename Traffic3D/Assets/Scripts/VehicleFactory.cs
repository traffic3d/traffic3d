using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleFactory : MonoBehaviour
{
    public float highRangeRespawnTime = 5;
    public float lowRangeRespawnTime = 3;
    public float maximumVehicleCount = 8;
    public float slowDownVehicleRateAt = 6;
    public float timeOfStartInvisibility = 1;
    public List<VehicleProbability> vehicleProbabilities;
    public List<Path> paths;
    public Dictionary<Rigidbody, Path> currentVehicles = new Dictionary<Rigidbody, Path>();

    void Start()
    {
        // This seed is needed for running benchmarks so if its removed
        // add an if statement to add the seed back with the following condition:
        // Settings.IsBenchmark()
        Random.InitState(123);
        if (vehicleProbabilities.Count == 0)
        {
            throw new System.Exception("No vehicles to spawn.");
        }
        if (vehicleProbabilities.Select(p => p.probability).Sum() != 1)
        {
            throw new System.Exception("Vehicle Probabilities do not sum to 100%");
        }
        if (paths.Count == 0)
        {
            throw new System.Exception("No paths for vehicles to spawn on.");
        }
        StartCoroutine(GenerateVehicle());
    }

    /// <summary>
    /// A loop that generates vehicles in a random way.
    /// </summary>
    IEnumerator GenerateVehicle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(lowRangeRespawnTime, highRangeRespawnTime));
            CleanVehicles();
            if (currentVehicles.Count < Random.Range(slowDownVehicleRateAt, maximumVehicleCount))
            {
                Path path = GetRandomUnusedPath();
                if (path != null)
                {
                    SpawnVehicle(GetRandomVehicle(), path);
                }
            }
        }
    }

    /// <summary>
    /// Remove vehicles that are no longer being used in the currentVehicles dictionary.
    /// </summary>
    public void CleanVehicles()
    {
        foreach (Rigidbody key in currentVehicles.Keys.Where(vehicle => vehicle == null).ToList())
        {
            currentVehicles.Remove(key);
        }
    }

    /// <summary>
    /// Create the vehicle with the path inputted.
    /// </summary>
    /// <param name="vehicle">The vehicle to create.</param>
    /// <param name="path">The path to set the vehicle on.</param>
    /// <returns>The created vehicle.</returns>
    public Rigidbody SpawnVehicle(Rigidbody vehicle, Path path)
    {
        Rigidbody spawnedVehicle = Instantiate(vehicle, path.nodes[0].position, path.nodes[0].rotation);
        TemporarilyHideVehicle(spawnedVehicle, timeOfStartInvisibility);
        VehicleEngine vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine>();
        vehicleEngine.SetPath(path);
        currentVehicles.Add(spawnedVehicle, path);
        return spawnedVehicle;
    }

    /// <summary>
    /// Gets a random vehicle from the vehicles list that is inputted by the user in Unity.
    /// </summary>
    /// <returns>The vehicle template.</returns>
    public Rigidbody GetRandomVehicle()
    {
        float finalProbability = Random.value;
        float cumulativeProbability = 0.0F;
        foreach (VehicleProbability vehicleProbability in vehicleProbabilities)
        {
            cumulativeProbability += vehicleProbability.probability;
            if (finalProbability <= cumulativeProbability)
            {
                return vehicleProbability.vehicle;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a random path for the vehicles to use.
    /// </summary>
    /// <returns>The path chosen.</returns>
    public Path GetRandomPath()
    {
        return paths[Random.Range(0, paths.Count - 1)];
    }

    /// <summary>
    /// Gets a random path that is not currently used by any vehicles.
    /// </summary>
    /// <returns>The path chosen which is unused.</returns>
    public Path GetRandomUnusedPath()
    {
        List<Path> unusedPaths = paths.FindAll(path => !currentVehicles.ContainsValue(path));
        if (unusedPaths.Count == 0)
        {
            return null;
        }
        return unusedPaths[Random.Range(0, unusedPaths.Count - 1)];
    }

    /// <summary>
    /// Temporarily hide a selected vehicle. 
    /// Mainly used at the start of a vehicle's life so it looks like they spawn in smoother.
    /// </summary>
    /// <param name="vehicle">The vehicle to temporarily hide.</param>
    /// <param name="hideForSeconds">The amount of seconds as a float to hide the vehicle for.</param>
    public void TemporarilyHideVehicle(Rigidbody vehicle, float hideForSeconds)
    {
        vehicle.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = false);
        StartCoroutine(WaitAndShowVehicle(vehicle, hideForSeconds));
    }

    IEnumerator WaitAndShowVehicle(Rigidbody vehicle, float hideForSeconds)
    {
        yield return new WaitForSeconds(hideForSeconds);
        vehicle.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = true);
    }

    [System.Serializable]
    public class VehicleProbability
    {
        public Rigidbody vehicle;
        public float probability;
    }

}

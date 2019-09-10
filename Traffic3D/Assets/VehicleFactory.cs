using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleFactory : MonoBehaviour
{

    public List<Rigidbody> vehicles;
    public List<Path> paths;

    public Dictionary<Rigidbody, Path> currentVehicles = new Dictionary<Rigidbody, Path>();

    // Use this for initialization
    void Start()
    {
        Random.InitState(123);

        if (vehicles.Count == 0)
        {
            throw new System.Exception("No vehicles to spawn.");
        }

        if (paths.Count == 0)
        {
            throw new System.Exception("No paths for vehicles to spawn on.");
        }

        StartCoroutine(GenerateCars());
    }

    IEnumerator GenerateCars()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(19, 22));

            CleanVehicles();

            if (currentVehicles.Count < Random.Range(6, 10))
            {

                Path path = GetRandomUnusedPath();

                if (path != null)
                {
                    currentVehicles.Add(SpawnVehicle(GetRandomVehicle(), path), path);
                }

            }

        }

    }

    public void CleanVehicles()
    {
        List<Rigidbody> vehiclesToRemove = new List<Rigidbody>();
        foreach (KeyValuePair<Rigidbody, Path> entry in currentVehicles)
        {
            if (entry.Key == null)
            {
                vehiclesToRemove.Add(entry.Key);
            }
        }

        foreach (var key in vehiclesToRemove)
        {
            currentVehicles.Remove(key);
        }
    }

    public Rigidbody SpawnVehicle(Rigidbody vehicle, Path path)
    {
        Rigidbody spawnedVehicle = Instantiate(vehicle, path.nodes[0].position, path.nodes[0].rotation);
        SetPath(spawnedVehicle, path);
        return spawnedVehicle;

    }

    public Rigidbody SpawnVehicle(Type engineType, Path path)
    {
        Rigidbody spawnedVehicle = null;
        foreach (Rigidbody vehicle in vehicles)
        {
            if (vehicle.gameObject.GetComponent(engineType) != null)
            {
                spawnedVehicle = Instantiate(vehicle, path.nodes[0].position, path.nodes[0].rotation);
                SetPath(spawnedVehicle, path);
            }
        }
        return spawnedVehicle;
    }

    public void SetPath(Rigidbody spawnedVehicle, Path path)
    {
        if (spawnedVehicle.GetComponent<VehicleEngine1>() != null)
        {
            VehicleEngine1 vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine1>();
            vehicleEngine.SetPath(path);
        }
        else if (spawnedVehicle.GetComponent<VehicleEngine2>() != null)
        {
            VehicleEngine2 vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine2>();
            vehicleEngine.SetPath(path);
        }
        else if (spawnedVehicle.GetComponent<VehicleEngine3>() != null)
        {
            VehicleEngine3 vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine3>();
            vehicleEngine.SetPath(path);
        }
        else if (spawnedVehicle.GetComponent<VehicleEngine4>() != null)
        {
            VehicleEngine4 vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine4>();
            vehicleEngine.SetPath(path);
        }
        else if (spawnedVehicle.GetComponent<VehicleEngine5>() != null)
        {
            VehicleEngine5 vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine5>();
            vehicleEngine.SetPath(path);
        }
        else if (spawnedVehicle.GetComponent<VehicleEngine6>() != null)
        {
            VehicleEngine6 vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine6>();
            vehicleEngine.SetPath(path);
        }
        else if (spawnedVehicle.GetComponent<VehicleEngine7>() != null)
        {
            VehicleEngine7 vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine7>();
            vehicleEngine.SetPath(path);
        }
        else if (spawnedVehicle.GetComponent<VehicleEngine8>() != null)
        {
            VehicleEngine8 vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine8>();
            vehicleEngine.SetPath(path);
        }
    }

    public Rigidbody GetRandomVehicle()
    {
        return vehicles[Random.Range(0, vehicles.Count - 1)];
    }

    public Path GetRandomPath()
    {
        return paths[Random.Range(0, paths.Count - 1)];
    }

    public Path GetRandomUnusedPath()
    {
        List<Path> unusedPaths = new List<Path>();

        foreach (Path path in paths)
        {
            if (!currentVehicles.ContainsValue(path))
            {
                unusedPaths.Add(path);
            }
        }

        if (unusedPaths.Count == 0)
        {
            return null;
        }

        return unusedPaths[Random.Range(0, unusedPaths.Count - 1)];

    }

}

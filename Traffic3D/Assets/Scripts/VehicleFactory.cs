﻿using System.Collections;
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
    public List<Rigidbody> vehicles;
    public List<Path> paths;
    public Dictionary<Rigidbody, Path> currentVehicles = new Dictionary<Rigidbody, Path>();

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
        StartCoroutine(GenerateVehicle());
    }

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

    public void CleanVehicles()
    {
        foreach (Rigidbody key in currentVehicles.Keys.Where(vehicle => vehicle == null).ToList())
        {
            currentVehicles.Remove(key);
        }
    }

    public Rigidbody SpawnVehicle(Rigidbody vehicle, Path path)
    {
        Rigidbody spawnedVehicle = Instantiate(vehicle, path.nodes[0].position, path.nodes[0].rotation);
        TemporarilyHideVehicle(spawnedVehicle, timeOfStartInvisibility);
        VehicleEngine vehicleEngine = spawnedVehicle.GetComponent<VehicleEngine>();
        vehicleEngine.SetPath(path);
        currentVehicles.Add(spawnedVehicle, path);
        return spawnedVehicle;
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
        List<Path> unusedPaths = paths.FindAll(path => !currentVehicles.ContainsValue(path));
        if (unusedPaths.Count == 0)
        {
            return null;
        }
        return unusedPaths[Random.Range(0, unusedPaths.Count - 1)];
    }

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
}

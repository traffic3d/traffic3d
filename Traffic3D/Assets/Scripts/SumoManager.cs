using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Types;
using System.Collections.Generic;
using UnityEngine;

public class SumoManager : MonoBehaviour
{
    private TraCIClient client;
    private VehicleFactory vehicleFactory;
    private Dictionary<string, Rigidbody> renderedVehicles = new Dictionary<string, Rigidbody>();

    void Start()
    {
        vehicleFactory = FindObjectOfType<VehicleFactory>();
        string filePath = System.IO.Path.Combine(Application.dataPath, "Sumo");
        ImportAndGenerate.parseXMLfiles(filePath);
        ImportAndGenerate.CreateStreetNetwork();
        client = new TraCIClient();
        if (client.Connect("127.0.0.1", 4001))
        {
            Debug.Log("Connected to Sumo");
        }
        else
        {
            Debug.Log("Unable to connect to Sumo");
            this.enabled = false;
        }
    }

    void FixedUpdate()
    {
        client.Control.SimStep(0.0);
        TraCIResponse<List<string>> vehicleIDs = client.Vehicle.GetIdList();
        foreach (string vehicleId in vehicleIDs.Content)
        {
            if (!renderedVehicles.ContainsKey(vehicleId))
            {
                CreateRenderedVehicle(vehicleId);
            }
            Position3D position = client.Vehicle.GetPosition3D(vehicleId).Content;
            double yawAngle = client.Vehicle.GetAngle(vehicleId).Content;
            UpdateRenderedVehicle(vehicleId, position, (float)yawAngle);
        }
        List<string> vehiclesToDestroy = new List<string>();
        foreach (string id in renderedVehicles.Keys)
        {
            if (!vehicleIDs.Content.Contains(id))
            {
                vehiclesToDestroy.Add(id);
            }
        }
        foreach (string id in vehiclesToDestroy)
        {
            DestroyRenderedVehicle(id);
        }

    }

    public void DestroyRenderedVehicle(string id)
    {
        Destroy(renderedVehicles[id].gameObject);
        renderedVehicles.Remove(id);
    }

    public void CreateRenderedVehicle(string id)
    {
        Rigidbody vehicle = Instantiate(vehicleFactory.GetRandomVehicle());
        renderedVehicles.Add(id, vehicle);
    }

    public void UpdateRenderedVehicle(string id, Position3D position3D, float angle)
    {
        renderedVehicles[id].transform.position = new Vector3((float)position3D.X, (float)position3D.Z, (float)position3D.Y);
        renderedVehicles[id].transform.rotation = Quaternion.Euler(0, angle, 0);
    }

}

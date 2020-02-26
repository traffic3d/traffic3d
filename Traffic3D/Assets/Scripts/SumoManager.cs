using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SumoManager : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = 4001;
    public List<SumoLinkControlPointObject> sumoControlSettings = new List<SumoLinkControlPointObject>();

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
        if (client.Connect(ip, port))
        {
            Debug.Log("Connected to Sumo");
        }
        else
        {
            Debug.Log("Unable to connect to Sumo");
            this.enabled = false;
            return;
        }
        StartCoroutine(Run());
        vehicleFactory.StopAllCoroutines();
        if (!IsControlledBySumo(SumoLinkControlPoint.TRAFFIC_FLOW))
        {
            StartCoroutine(RunTraffic3DTrafficFlow());
        }
    }

    IEnumerator Run()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / 60F);
            client.Control.SimStep(0.0);
        }
    }

    IEnumerator RunTraffic3DTrafficFlow()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(vehicleFactory.lowRangeRespawnTime, vehicleFactory.highRangeRespawnTime));
            if (renderedVehicles.Count < UnityEngine.Random.Range(vehicleFactory.slowDownVehicleRateAt, vehicleFactory.maximumVehicleCount))
            {
                AddVehicle();
            }
        }
    }

    public void AddVehicle()
    {
        client.Vehicle.Add(Guid.NewGuid().ToString(), "DEFAULT_VEHTYPE", GetRandomSumoRoute(), 0, 0, 0, 0);
    }

    void Update()
    {
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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
        long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public void DestroyRenderedVehicle(string id)
    {
        Destroy(renderedVehicles[id].gameObject);
        renderedVehicles.Remove(id);
    }

    public void CreateRenderedVehicle(string id)
    {
        Rigidbody vehicle = Instantiate(vehicleFactory.GetRandomVehicle());
        vehicle.isKinematic = true;
        vehicle.GetComponent<VehicleEngine>().enabled = false;
        foreach (BoxCollider boxCollider in vehicle.GetComponentsInChildren<BoxCollider>())
        {
            boxCollider.enabled = false;
        }
        renderedVehicles.Add(id, vehicle);
    }

    public void UpdateRenderedVehicle(string id, Position3D position3D, float angle)
    {
        renderedVehicles[id].transform.position = new Vector3((float)position3D.X, (float)position3D.Z, (float)position3D.Y);
        renderedVehicles[id].transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public string GetRandomSumoRoute()
    {
        return ImportAndGenerate.routes.Keys.ToArray()[UnityEngine.Random.Range(0, ImportAndGenerate.routes.Count)];
    }

    public bool IsControlledBySumo(SumoLinkControlPoint sumoLinkControlPoint)
    {
        SumoLinkControlPointObject controlPoint = sumoControlSettings.Find(controlSetting => controlSetting.sumoLinkControlPoint == sumoLinkControlPoint);
        if (controlPoint == null || !controlPoint.controlledBySumo)
        {
            return false;
        }
        return true;
    }

    [System.Serializable]
    public class SumoLinkControlPointObject
    {
        public SumoLinkControlPoint sumoLinkControlPoint;
        public bool controlledBySumo;
    }

}

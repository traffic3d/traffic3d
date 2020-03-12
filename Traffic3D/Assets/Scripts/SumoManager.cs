﻿using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SumoManager : MonoBehaviour
{
    private static SumoManager instance;

    public static SumoManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    public string ip = "127.0.0.1";
    public int port = 4001;
    public List<SumoLinkControlPointObject> sumoControlSettings = new List<SumoLinkControlPointObject>();

    private bool connected = false;
    private TraCIClient client;
    private VehicleFactory vehicleFactory;
    private Dictionary<string, Rigidbody> renderedVehicles = new Dictionary<string, Rigidbody>();

    private List<SumoTrafficLight> sumoTrafficLights = new List<SumoTrafficLight>();

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
            connected = true;
        }
        else
        {
            Debug.Log("Unable to connect to Sumo");
            this.enabled = false;
            return;
        }
        FindObjectOfType<CameraManager>().frameRate = 60;
        StartCoroutine(Run());
        vehicleFactory.StopAllCoroutines();
        TrafficLightManager.GetInstance().RefreshTrafficLightsAndJunctions();

        // Traffic Flow
        if (!IsControlledBySumo(SumoLinkControlPoint.TRAFFIC_FLOW))
        {
            StartCoroutine(RunTraffic3DTrafficFlow());
        }

        // Traffic Lights
        List<string> junctionIds = client.TrafficLight.GetIdList().Content;
        foreach (string id in junctionIds)
        {
            List<string> controlledLanes = client.TrafficLight.GetControlledLanes(id).Content;
            string currentState = client.TrafficLight.GetState(id).Content;
            for (int i = 0; i < controlledLanes.Count; i++)
            {
                TrafficLight trafficLight = TrafficLightManager.GetInstance().GetTrafficLight(controlledLanes[i]);
                if (trafficLight != null)
                {
                    SumoTrafficLight sumoTrafficLight = sumoTrafficLights.Find(s => s.trafficLight.trafficLightId.Equals(trafficLight.trafficLightId));
                    if (sumoTrafficLight == null)
                    {
                        sumoTrafficLights.Add(new SumoTrafficLight(trafficLight, id, new HashSet<int>() { i }));
                    }
                    else
                    {
                        sumoTrafficLight.AddIndexState(i);
                    }
                }
            }
            // Remove all current traffic light programs in Sumo
            if (!IsControlledBySumo(SumoLinkControlPoint.TRAFFIC_LIGHTS))
            {
                client.TrafficLight.SetRedYellowGreenState(id, new string('r', currentState.Length));
                client.TrafficLight.SetPhaseDuration(id, Double.MaxValue);
            }
        }
        if (IsControlledBySumo(SumoLinkControlPoint.TRAFFIC_LIGHTS))
        {
            TrafficLightManager.GetInstance().StopAllCoroutines();
            StartCoroutine(RunTrafficLights());
        }
        else
        {
            TrafficLightManager.GetInstance().trafficLightChangeEvent += ChangeSumoTrafficLights;
            foreach (string junctionId in junctionIds)
            {
                List<SumoTrafficLight> sumoTrafficLightsForJunction = sumoTrafficLights.FindAll(sumoTrafficLight => sumoTrafficLight.junctionId.Equals(junctionId));
                Junction junction = FindObjectsOfType<Junction>().ToList().Find(j => j.junctionId.Equals(junctionId));
                if (junction == null)
                {
                    continue;
                }
                int stateCounter = 0;
                foreach (SumoTrafficLight sumoTrafficLightMain in sumoTrafficLightsForJunction)
                {
                    stateCounter++;
                    GameObject stateObject = new GameObject("State" + stateCounter);
                    stateObject.transform.SetParent(junction.gameObject.transform);
                    JunctionState junctionState = stateObject.AddComponent<JunctionState>();
                    junctionState.stateNumber = stateCounter;
                    int trafficLightStateCounter = 0;
                    junctionState.states = new JunctionState.TrafficLightState[sumoTrafficLightsForJunction.Count()];
                    foreach (SumoTrafficLight sumoTrafficLight in sumoTrafficLightsForJunction)
                    {
                        junctionState.states[trafficLightStateCounter] = new JunctionState.TrafficLightState(sumoTrafficLight.trafficLight.trafficLightId, (sumoTrafficLightMain == sumoTrafficLight ? TrafficLight.LightColour.GREEN : TrafficLight.LightColour.RED));
                        trafficLightStateCounter++;
                    }
                }
            }
            foreach (SumoTrafficLight sumoTrafficLight in sumoTrafficLights)
            {

            }
        }

    }

    IEnumerator Run()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / 60F);
            var task = Task.Run(() => client.Control.SimStep(0.0));
            if (!task.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new Exception("Sumo Timed out");
            }
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

    IEnumerator RunTrafficLights()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1F);
            foreach (string id in client.TrafficLight.GetIdList().Content)
            {
                sumoTrafficLights.FindAll(t => t.junctionId.Equals(id)).ForEach(t => t.UpdateTrafficLight(client.TrafficLight.GetState(id).Content));
            }
        }
    }

    public void ChangeSumoTrafficLights(object sender, TrafficLight.TrafficLightChangeEventArgs trafficLightChangeEvent)
    {
        SumoTrafficLight sumoTrafficLight = sumoTrafficLights.Find(s => s.trafficLight.trafficLightId.Equals(trafficLightChangeEvent.trafficLight.trafficLightId));
        if (sumoTrafficLight == null)
        {
            Debug.Log("Unable to find sumo traffic light: " + trafficLightChangeEvent.trafficLight.trafficLightId);
            return;
        }
        string currentState = client.TrafficLight.GetState(sumoTrafficLight.junctionId).Content;
        string newState = sumoTrafficLight.GetStateFromTrafficLightColour(currentState);
        client.TrafficLight.SetRedYellowGreenState(sumoTrafficLight.junctionId, newState);
    }

    public bool IsConnected()
    {
        return connected;
    }

    public void AddVehicle()
    {
        client.Vehicle.Add(Guid.NewGuid().ToString(), "DEFAULT_VEHTYPE", GetRandomSumoRoute(), 0, 0, 0, 0);
    }

    void Update()
    {
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

using CodingConnected.TraCI.NET;
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
    private const int SIMULATION_RUNS_PER_SECOND = 60;
    private const int SUMO_TIME_OUT = 5;

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
    private Dictionary<string, GameObject> renderedVehicles = new Dictionary<string, GameObject>();

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
        PythonManager pythonManager = FindObjectOfType<PythonManager>();
        if (pythonManager != null)
        {
            pythonManager.frameRate = 60;
        }
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

            foreach (tlLogicType tlLogicType in ImportAndGenerate.trafficLightPrograms.Values)
            {
                int stateCounter = 0;
                Junction junction = FindObjectsOfType<Junction>().ToList().Find(j => j.junctionId.Equals(tlLogicType.id));
                List<SumoTrafficLight> sumoTrafficLightsForJunction = sumoTrafficLights.FindAll(sumoTrafficLight => sumoTrafficLight.junctionId.Equals(tlLogicType.id));
                foreach (object obj in tlLogicType.Items)
                {
                    if (obj is phaseType)
                    {
                        stateCounter++;
                        GameObject stateObject = new GameObject("State" + stateCounter);
                        stateObject.transform.SetParent(junction.gameObject.transform);
                        JunctionState junctionState = stateObject.AddComponent<JunctionState>();
                        junctionState.stateNumber = stateCounter;
                        junctionState.trafficLightStates = new JunctionState.TrafficLightState[sumoTrafficLightsForJunction.Count()];
                        int trafficLightStateCounter = 0;
                        phaseType phase = (phaseType)obj;

                        foreach (SumoTrafficLight sumoTrafficLight in sumoTrafficLightsForJunction)
                        {
                            TrafficLight.LightColour lightColour = sumoTrafficLight.GetLightColourFromStateString(phase.state);
                            junctionState.trafficLightStates[trafficLightStateCounter] = new JunctionState.TrafficLightState(sumoTrafficLight.trafficLight.trafficLightId, lightColour);
                            trafficLightStateCounter++;
                        }
                    }
                }
            }
        }

    }

    IEnumerator Run()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / SIMULATION_RUNS_PER_SECOND);
            var task = Task.Run(() => client.Control.SimStep(0.0));
            if (!task.Wait(TimeSpan.FromSeconds(SUMO_TIME_OUT)))
            {
                throw new Exception("Sumo Timed out");
            }
        }
    }

    IEnumerator RunTraffic3DTrafficFlow()
    {
        while (true)
        {
            yield return new WaitForSeconds(RandomNumberGenerator.GetInstance().Range(vehicleFactory.lowRangeRespawnTime, vehicleFactory.highRangeRespawnTime));
            if (renderedVehicles.Count < RandomNumberGenerator.GetInstance().Range(vehicleFactory.slowDownVehicleRateAt, vehicleFactory.maximumVehicleCount))
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
        GameObject vehicle = Instantiate(vehicleFactory.GetRandomVehicle()).gameObject;
        vehicle.GetComponent<Rigidbody>().isKinematic = true;
        vehicle.GetComponent<Vehicle>().enabled = false;
        vehicle.GetComponent<VehicleDriver>().enabled = false;
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
        return ImportAndGenerate.routes.Keys.ToArray()[RandomNumberGenerator.GetInstance().Range(0, ImportAndGenerate.routes.Count)];
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

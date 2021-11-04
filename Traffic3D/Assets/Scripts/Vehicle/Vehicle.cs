using System;
using System.Linq;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [HideInInspector]
    public VehicleSettings vehicleSettings;
    [HideInInspector]
    public VehicleEngine vehicleEngine;
    [HideInInspector]
    public VehicleDriver vehicleDriver;

    private void Awake()
    {
        vehicleSettings = gameObject.GetComponent<VehicleSettings>();
        if(vehicleSettings == null)
        {
            Debug.LogError("Vehicle has no VehicleSettings Script: " + gameObject.name);
            return;
        }
        vehicleEngine = gameObject.AddComponent<VehicleEngine>();
        vehicleDriver = gameObject.AddComponent<VehicleDriver>();
        Mesh mainMesh = GetComponentsInChildren<MeshFilter>().Aggregate((m1, m2) => (m1.mesh.bounds.extents.x * m1.mesh.bounds.extents.y * m1.mesh.bounds.extents.z) > (m2.mesh.bounds.extents.x * m2.mesh.bounds.extents.y * m2.mesh.bounds.extents.z) ? m1 : m2).mesh;
        if (vehicleSettings.longestSideLength < 0)
        {
            vehicleSettings.longestSideLength = Math.Max(mainMesh.bounds.size.z * transform.lossyScale.z, mainMesh.bounds.size.x * transform.lossyScale.x);
        }
        if (vehicleSettings.shortestSideLength < 0)
        {
            vehicleSettings.shortestSideLength = Math.Min(mainMesh.bounds.size.z * transform.lossyScale.z, mainMesh.bounds.size.x * transform.lossyScale.x);
        }
    }

    private void Start()
    {
        EventManager.GetInstance().VehicleSpawnEvent += OnVehicleSpawnEvent;
        EventManager.GetInstance().VehicleDestroyEvent += OnVehicleDestroyEvent;
    }

    public void OnVehicleSpawnEvent(object sender, VehicleEventArgs args)
    {
        vehicleDriver.vehicleSensors.GetSensor<MergeSensor>().AddVehicleIntersectionPoint(args.vehicle);
    }

    public void OnVehicleDestroyEvent(object sender, VehicleEventArgs args)
    {
        vehicleDriver.vehicleSensors.GetSensor<MergeSensor>().vehicleIntersectionPoints.Remove(args.vehicle);
    }

    private void OnDestroy()
    {
        EventManager.GetInstance().VehicleSpawnEvent -= OnVehicleSpawnEvent;
        EventManager.GetInstance().VehicleDestroyEvent -= OnVehicleDestroyEvent;
        EventManager.GetInstance().CallVehicleDestroyEvent(this, new VehicleEventArgs(this));
        Destroy(vehicleEngine);
        Destroy(vehicleDriver);
    }

    /// <summary>
    /// Destroys the vehicle and passes information to the Python Manager.
    /// </summary>
    public void DestroyVehicle()
    {
        if (!vehicleDriver.densityCountTriggered)
        {
            vehicleDriver.densityCountTriggered = true;
            PythonManager.GetInstance().IncrementDensityCount();
        }
        Vector3 endPos = transform.position;
        double distance = Vector3.Distance(vehicleDriver.startPos, endPos);
        double time = (Time.time - vehicleDriver.startTime);
        double speed = (distance / time);
        PythonManager.GetInstance().speedList.Add(speed);
        Destroy(this.gameObject);
        PythonManager.GetInstance().IncrementRewardCount();
        Utils.AppendAllTextToResults(Utils.VEHICLE_TIMES_FILE_NAME, time.ToString() + ",");
    }
}

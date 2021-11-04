using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrontCollisionSensor : ISensor, IGizmos
{
    private Vehicle vehicle;
    private float distanceBetweenRays;

    private List<GizmoRay> gizmoRays;

    public FrontCollisionSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        this.gizmoRays = new List<GizmoRay>();
    }

    public void Start()
    {
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        distanceBetweenRays = (vehicleSettings.shortestSideLength / (vehicleSettings.numberOfSensorRays - 1));
    }

    public void Run(Dictionary<string, object> args)
    {
        VehicleDriver vehicleDriver = this.vehicle.vehicleDriver;
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        List<Ray> rays = new List<Ray>();
        RaycastHit hit;
        Vector3 rayPosition = vehicle.transform.position + vehicle.transform.TransformDirection(Vector3.up);
        rayPosition = rayPosition + vehicle.transform.TransformDirection(Vector3.forward) * ((vehicleSettings.longestSideLength / 2) - 1) + vehicle.transform.TransformDirection(Vector3.left) * (vehicleSettings.shortestSideLength / 2);
        float angle = (float)Math.PI * vehicleSettings.wheelColliderFrontLeft.steerAngle / 180f;
        Vector3 direction = vehicle.transform.TransformDirection(new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle)));
        for (float i = 0; i <= vehicleSettings.shortestSideLength; i = i + distanceBetweenRays)
        {
            Ray ray = new Ray(rayPosition + vehicle.transform.TransformDirection(Vector3.right) * i, direction);
            rays.Add(ray);
        }
        float speedToTarget = vehicle.vehicleEngine.targetSpeed;
        float distanceToMonitor = Math.Min(Math.Max(vehicleSettings.maxDistanceToMonitor - (Math.Abs(angle) * vehicleSettings.steerReduceRayConstant * vehicleSettings.maxDistanceToMonitor), vehicleSettings.stoppingDistance), vehicleSettings.maxDistanceToMonitor);
        vehicleDriver.isWaitingOnSensorRays = false;
        foreach (Ray ray in rays)
        {
            if (Physics.Raycast(ray, out hit, distanceToMonitor, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                if (vehicle.gameObject.GetComponentsInChildren<Collider>().Any(c => c.Equals(hit.collider)))
                {
                    continue;
                }
                if (vehicleDriver.debug)
                {
                    gizmoRays.Add(new GizmoRay(ray.origin, ray.direction * hit.distance, new Color(1, (hit.distance / distanceToMonitor), 0)));
                }
                speedToTarget = Math.Min(speedToTarget, (hit.distance - vehicleSettings.stoppingDistance) / 2);
                if ((hit.distance - vehicleSettings.stoppingDistance) / 2 < vehicleSettings.isWaitingOnSensorRaysDistance)
                {
                    vehicleDriver.isWaitingOnSensorRays = true;
                }
            }
            else
            {
                if (vehicleDriver.debug)
                {
                    gizmoRays.Add(new GizmoRay(ray.origin, ray.direction * distanceToMonitor, Color.green));
                }
            }
        }
        vehicle.vehicleEngine.SetTargetSpeed(speedToTarget);
    }

    public void OnDrawGizmos()
    {
        if (this.gizmoRays.Count == 0)
        {
            return;
        }
        foreach (GizmoRay gizmoRay in this.gizmoRays)
        {
            gizmoRay.Draw();
        }
        gizmoRays.Clear();
    }
}

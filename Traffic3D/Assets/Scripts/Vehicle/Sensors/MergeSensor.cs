using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MergeSensor : ISensor, IGizmos
{
    private Vehicle vehicle;
    private const float mergeAlgorithmPadding = 3f;
    private bool isLeftHandDrive;
    private List<GizmoRay> gizmoRays;
    public UnityEvent mergeSensorRunCompleteEvent;
    public Dictionary<Vehicle, HashSet<PathIntersectionPoint>> vehicleIntersectionPoints = new Dictionary<Vehicle, HashSet<PathIntersectionPoint>>();

    public MergeSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        this.mergeSensorRunCompleteEvent = new UnityEvent();
        this.gizmoRays = new List<GizmoRay>();
    }

    public void Start()
    {
        // Find all vehicles and intersection paths
        foreach (Vehicle vehicle in GameObject.FindObjectsOfType<Vehicle>())
        {
            AddVehicleIntersectionPoint(vehicle);
        }
        isLeftHandDrive = GameObject.FindObjectOfType<VehicleFactory>().isLeftHandDrive;
        this.vehicle.vehicleDriver.vehicleSensors.GetSensor<StopLineSensor>().vehicleApproachingStopLineEvent.AddListener(stopLineType => this.Run(stopLineType));
    }

    public void Run(Dictionary<string, object> args)
    {
        this.Run((StopLine.Type)args["stopLineType"]);
    }

    public void Run(StopLine.Type stopLineType)
    {
        VehicleDriver vehicleDriver = this.vehicle.vehicleDriver;
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        VehicleNavigation vehicleNavigation = vehicleDriver.vehicleNavigation;
        float previousTargetSpeed = vehicle.vehicleEngine.targetSpeed;
        vehicleDriver.waitingForVehicleDriverAhead = null;
        vehicleDriver.waitingForVehicleDriverBehind = null;
        UpdateVehicleIntersectionPointCache();
        Dictionary<PathIntersectionPoint, HashSet<Vehicle>> vehiclesAtIntersectionPoint = GetVehiclesAtIntersectionPointsUsingCache();
        if (vehiclesAtIntersectionPoint.Count == 0)
        {
            return;
        }
        // Find intersections within a certain distance of the vehicle
        HashSet<PathIntersectionPoint> pathIntersectionPoints = new HashSet<PathIntersectionPoint>(vehiclesAtIntersectionPoint.Keys.Where(i =>
        {
            if (vehicleNavigation.path.GetDistanceFromVehicleToIntersectionPoint(vehicleNavigation.path, vehicleNavigation.currentNodeNumber, vehicle.transform, i, out float distanceResult))
            {
                return distanceResult <= vehicleSettings.mergeRadiusCheck;
            }
            return false;
        }
        ));
        foreach (PathIntersectionPoint pathIntersectionPoint in pathIntersectionPoints)
        {
            if (stopLineType != StopLine.Type.MERGE)
            {
                if (IsOtherPathIncomingFromLookingDirection(pathIntersectionPoint))
                {
                    continue;
                }
            }
            if (vehicleDriver.debug)
            {
                gizmoRays.Add(new GizmoRay(pathIntersectionPoint.intersection, Vector3.up * 5, Color.red));
            }
            HashSet<Vehicle> vehiclesWithSameIntersection = vehiclesAtIntersectionPoint[pathIntersectionPoint];
            float currentDistance;
            if (!vehicleNavigation.path.GetDistanceFromVehicleToIntersectionPoint(vehicleNavigation.path, vehicleNavigation.currentNodeNumber, vehicle.transform, pathIntersectionPoint, out currentDistance))
            {
                // Unable to get path distance to intersection
                continue;
            }
            float distanceAhead = float.MaxValue;
            float distanceBehind = float.MaxValue;
            VehicleDriver vehicleAhead = null;
            VehicleDriver vehicleBehind = null;
            foreach (Vehicle otherVehicle in vehiclesWithSameIntersection)
            {
                float otherVehicleDistance;
                if (!otherVehicle.vehicleDriver.vehicleNavigation.path.GetDistanceFromVehicleToIntersectionPoint(otherVehicle.vehicleDriver.vehicleNavigation.path, otherVehicle.vehicleDriver.vehicleNavigation.currentNodeNumber, otherVehicle.transform, pathIntersectionPoint, out otherVehicleDistance))
                {
                    // Vehicle is now past intersection point
                    continue;
                }
                float relativeDistance = currentDistance - otherVehicleDistance;
                if (relativeDistance >= 0 && distanceAhead > relativeDistance)
                {
                    distanceAhead = relativeDistance;
                    vehicleAhead = otherVehicle.vehicleDriver;
                }
                else if (relativeDistance < 0 && distanceBehind > -relativeDistance)
                {
                    distanceBehind = -relativeDistance;
                    vehicleBehind = otherVehicle.vehicleDriver;
                }
            }
            if (distanceAhead - vehicleSettings.longestSideLength - mergeAlgorithmPadding <= vehicle.vehicleEngine.currentSpeed)
            {
                vehicleDriver.waitingForVehicleDriverAhead = vehicleAhead;
            }
            if (distanceBehind <= vehicleSettings.maxSpeed)
            {
                vehicleDriver.waitingForVehicleDriverBehind = vehicleBehind;
            }
            ApplyMergeAlgorithm(distanceAhead, distanceBehind);
        }
        this.mergeSensorRunCompleteEvent.Invoke();
    }

    private void ApplyMergeAlgorithm(float distanceAhead, float distanceBehind)
    {
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        if (distanceAhead - vehicleSettings.longestSideLength - mergeAlgorithmPadding > vehicle.vehicleEngine.currentSpeed && distanceBehind > vehicleSettings.maxSpeed)
        {
            vehicle.vehicleEngine.SetTargetSpeed(Math.Min(vehicle.vehicleEngine.targetSpeed, distanceAhead));
        }
        else
        {
            vehicle.vehicleEngine.SetTargetSpeed(0);
        }
    }

    /// <summary>
    /// Checks incoming paths from the right when driving on the left and the left when driving on the right
    /// </summary>
    /// <param name="pathIntersectionPoint"></param>
    /// <returns>True if incoming path is not in looking direction</returns>
    /// The diagram below shows which section is classed as the incoming path
    /*
                      |                                                         
                      |                                                         
                      -                                                         
                     /|\                                                        
                    / | \        Incoming Path                                  
                      |     +--------------------+                              
                      |     |                    |                              
                      |     |                    |                              
           /          |     |        /           |                              
          /           |     |       /            |                              
    ------------------------------------------------------                          
          \           |     |       \            |     Other Path               
           \          |     |        \           |                              
                      -     |                    |                              
                     /|\    |                    |                              
                    / | \   +--------------------+                              
                      |                                                         
        Current Path  |                                                         
                      |                                                         
                      |                                                         
    */
    private bool IsOtherPathIncomingFromLookingDirection(PathIntersectionPoint pathIntersectionPoint)
    {
        VehicleDriver vehicleDriver = this.vehicle.vehicleDriver;
        PathIntersectionLine currentIntersectionLine = pathIntersectionPoint.GetLineFromPath(vehicleDriver.vehicleNavigation.path);
        return (isLeftHandDrive && !pathIntersectionPoint.IsIncomingPathFromDirection(PathIntersectionPoint.Direction.RIGHT, currentIntersectionLine)) ||
            (!isLeftHandDrive && !pathIntersectionPoint.IsIncomingPathFromDirection(PathIntersectionPoint.Direction.LEFT, currentIntersectionLine));
    }

    /// <summary>
    /// Add all intersection points with this vehicle and another to a list for use in MergeCheck()
    /// </summary>
    /// <param name="otherVehicle">The other vehicle to check</param>
    public void AddVehicleIntersectionPoint(Vehicle otherVehicle)
    {
        if (otherVehicle != null && !otherVehicle.vehicleDriver.Equals(this.vehicle.vehicleDriver))
        {
            vehicleIntersectionPoints.Add(otherVehicle, vehicle.vehicleDriver.vehicleNavigation.path.GetIntersectionPoints(otherVehicle.vehicleDriver.vehicleNavigation.path));
        }
    }

    /// <summary>
    /// Ensures that there isn't a vehicle that has already been destroyed and is still in the cache
    /// </summary>
    private void UpdateVehicleIntersectionPointCache()
    {
        foreach (Vehicle vehicle in vehicleIntersectionPoints.Keys.Where(v => v == null).ToList())
        {
            vehicleIntersectionPoints.Remove(vehicle);
        }
    }

    /// <summary>
    /// Gets a Dictionary of intersection points with the vehicles that will be passing through that intersection point
    /// </summary>
    /// <returns>A dictionary of interesection points with vehicles</returns>
    private Dictionary<PathIntersectionPoint, HashSet<Vehicle>> GetVehiclesAtIntersectionPointsUsingCache()
    {
        Dictionary<PathIntersectionPoint, HashSet<Vehicle>> vehiclesAtIntersectionPoint = new Dictionary<PathIntersectionPoint, HashSet<Vehicle>>();
        foreach (KeyValuePair<Vehicle, HashSet<PathIntersectionPoint>> entry in vehicleIntersectionPoints)
        {
            foreach (PathIntersectionPoint intersectionPoint in entry.Value)
            {
                if (vehiclesAtIntersectionPoint.ContainsKey(intersectionPoint))
                {
                    vehiclesAtIntersectionPoint[intersectionPoint].Add(entry.Key);
                }
                else
                {
                    vehiclesAtIntersectionPoint.Add(intersectionPoint, new HashSet<Vehicle>() { entry.Key });
                }
            }
        }
        return vehiclesAtIntersectionPoint;
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

using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class StopLineSensor : ISensor
{
    private Vehicle vehicle;
    public StopLineTypeUnityEvent vehicleApproachingStopLineEvent;

    public StopLineSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        this.vehicleApproachingStopLineEvent = new StopLineTypeUnityEvent();
    }

    public void Start()
    {

    }

    public void Run(Dictionary<string, object> args)
    {
        VehicleDriver vehicleDriver = this.vehicle.vehicleDriver;
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        VehicleNavigation vehicleNavigation = vehicleDriver.vehicleNavigation;
        StopLine nextStopLine = vehicleNavigation.path.GetNextStopLine(vehicleNavigation.currentNode);
        if (nextStopLine != null)
        {
            float distance = vehicleNavigation.path.GetDistanceToNextStopLine(vehicleNavigation.currentNode, vehicle.transform);
            if (!float.IsNaN(distance) && distance < vehicleSettings.maxDistanceToMonitor)
            {
                if (distance <= vehicleSettings.stopLineEvaluationDistance)
                {
                    vehicleApproachingStopLineEvent.Invoke(nextStopLine.type);
                }
                else
                {
                    float speed = distance * vehicleSettings.stopDistanceToSpeed;
                    vehicle.vehicleEngine.SetTargetSpeed(Math.Min(Math.Max(0, speed), vehicle.vehicleEngine.targetSpeed));
                }
            }
        }
    }

    [System.Serializable]
    public class StopLineTypeUnityEvent : UnityEvent<StopLine.Type>
    {
    }
}

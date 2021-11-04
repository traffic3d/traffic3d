using System.Collections.Generic;
using UnityEngine;

public class StopNodeSensor : ISensor
{
    private Vehicle vehicle;

    public StopNodeSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }

    public void Start()
    {

    }

    public void Run(Dictionary<string, object> args)
    {
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        VehicleNavigation vehicleNavigation = this.vehicle.vehicleDriver.vehicleNavigation;
        Transform nextStopNode = vehicleNavigation.path.GetNextStopNode(vehicleNavigation.currentNode);
        if (nextStopNode != null)
        {
            float distance = vehicleNavigation.path.GetDistanceToNextStopNode(vehicleNavigation.currentNode, vehicle.transform);
            if (!float.IsNaN(distance) && distance < vehicleSettings.maxDistanceToMonitor)
            {
                float speed = distance * vehicleSettings.stopDistanceToSpeed;
                if (TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(nextStopNode).IsCurrentLightColour(TrafficLight.LightColour.RED))
                {
                    vehicle.vehicleEngine.SetTargetSpeed(speed);
                }
                else
                {
                    vehicle.vehicleEngine.SetTargetSpeed((speed < vehicleSettings.trafficLightMaxSpeedOnGreen ? vehicleSettings.trafficLightMaxSpeedOnGreen : speed));
                }
            }
        }
    }
}

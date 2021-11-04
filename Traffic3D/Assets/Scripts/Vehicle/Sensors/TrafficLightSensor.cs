using System.Collections.Generic;
using UnityEngine;

public class TrafficLightSensor : ISensor
{
    private Vehicle vehicle;
    public float startDelayTime = -1;

    public TrafficLightSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }

    public void Start()
    {

    }

    public void Run(Dictionary<string, object> args)
    {
        TrafficLight trafficLight = TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(vehicle.vehicleDriver.vehicleNavigation.currentNode);
        if ((trafficLight != null && trafficLight.IsCurrentLightColour(TrafficLight.LightColour.RED)) || vehicle.gameObject.tag == "collided")
        {
            vehicle.vehicleEngine.SetTargetSpeed(0);
        }
        if (startDelayTime == -1 && trafficLight != null)
        {
            startDelayTime = Time.time;
        }
    }
}

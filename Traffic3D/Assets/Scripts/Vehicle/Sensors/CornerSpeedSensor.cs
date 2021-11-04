using System;
using System.Collections.Generic;

public class CornerSpeedSensor : ISensor
{
    private Vehicle vehicle;

    public CornerSpeedSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }

    public void Start()
    {

    }

    public void Run(Dictionary<string, object> args)
    {
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        VehicleDriver vehicleDriver = this.vehicle.vehicleDriver;
        VehicleNavigation vehicleNavigation = vehicleDriver.vehicleNavigation;
        // Store the distances with their angles
        Dictionary<float, float> distanceAndAngles = new Dictionary<float, float>();
        foreach (float distanceToCheck in vehicleSettings.distanceForSpeedCheck)
        {
            float angle = vehicleNavigation.path.GetDirectionDifferenceToRoadAheadByDistanceMeasured(vehicleNavigation.currentNode, vehicle.transform, distanceToCheck, vehicleDriver.debug);
            if (float.IsNaN(angle))
            {
                continue;
            }
            distanceAndAngles.Add(distanceToCheck, angle);
        }
        // Ignore if there are no distances and angles
        if (distanceAndAngles.Count > 0)
        {
            float averageAngleDifference = 0;
            foreach (KeyValuePair<float, float> entry in distanceAndAngles)
            {
                averageAngleDifference = averageAngleDifference + entry.Value;
            }
            // Find the average angle using the mean of all angles found.
            averageAngleDifference = averageAngleDifference / distanceAndAngles.Count;
            // Put the averaged angle into a exponential decay curve formula where x is the angle and y is the speed output
            double finalSpeed = Math.Pow(vehicleSettings.cornerSensitivityModifier, -averageAngleDifference / vehicleSettings.cornerThresholdDegrees) * vehicleSettings.maxSpeed;
            // Keep final speed between the minimum corner speed and the max speed.
            finalSpeed = Math.Max(vehicleSettings.cornerMinSpeed, Math.Min(finalSpeed, vehicleSettings.maxSpeed));
            vehicle.vehicleEngine.SetTargetSpeed((float)finalSpeed);
        }
    }
}

using UnityEngine;

public class DensityMeasurePoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Vehicle vehicle = other.GetComponentInParent<Vehicle>();
        if (vehicle != null && !vehicle.vehicleDriver.densityCountTriggered)
        {
            TrafficLightSensor trafficLightSensor = vehicle.vehicleDriver.vehicleSensors.GetSensor<TrafficLightSensor>();
            vehicle.vehicleDriver.densityCountTriggered = true;
            PythonManager.GetInstance().IncrementDensityCount();
            // Calculate Delay
            if (trafficLightSensor.startDelayTime != -1)
            {
                float delay = Time.time - trafficLightSensor.startDelayTime;
                Utils.AppendAllTextToResults(Utils.VEHICLE_DELAY_TIMES_FILE_NAME, delay.ToString() + ",");
            }
        }
    }
}

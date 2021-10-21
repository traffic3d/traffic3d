using UnityEngine;

public class DensityMeasurePoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Vehicle vehicle = other.GetComponentInParent<Vehicle>();
        if (vehicle != null && !vehicle.vehicleDriver.densityCountTriggered)
        {
            vehicle.vehicleDriver.densityCountTriggered = true;
            PythonManager.GetInstance().IncrementDensityCount();
            // Calculate Delay
            if (vehicle.vehicleDriver.startDelayTime != -1)
            {
                float delay = Time.time - vehicle.vehicleDriver.startDelayTime;
                Utils.AppendAllTextToResults(Utils.VEHICLE_DELAY_TIMES_FILE_NAME, delay.ToString() + ",");
            }
        }
    }
}

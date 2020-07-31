using UnityEngine;

public class DensityMeasurePoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        VehicleEngine vehicleEngine = other.GetComponentInParent<VehicleEngine>();
        if (vehicleEngine != null && !vehicleEngine.densityCountTriggered)
        {
            vehicleEngine.densityCountTriggered = true;
            PythonManager.GetInstance().IncrementDensityCount();
            // Calculate Delay
            if (vehicleEngine.startDelayTime != -1)
            {
                float delay = Time.time - vehicleEngine.startDelayTime;
                Utils.AppendAllTextToResults(Utils.VEHICLE_DELAY_TIMES_FILE_NAME, delay.ToString() + ",");
            }
        }
    }
}

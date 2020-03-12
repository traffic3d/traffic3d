using System.Collections;
using System.Collections.Generic;
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
        }
    }
}

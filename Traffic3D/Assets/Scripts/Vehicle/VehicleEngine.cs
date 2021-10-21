using UnityEngine;

public class VehicleEngine : MonoBehaviour
{
    public VehicleSettings vehicleSettings;
    public EngineStatus engineStatus;
    public float targetSpeed;
    public float currentMotorTorque;
    public float currentSpeed;

    private const float metresPerSecondToKilometresPerHourConversion = 3.6f;

    private void Awake()
    {
        vehicleSettings = gameObject.GetComponent<VehicleSettings>();
    }

    void Start()
    {
        engineStatus = EngineStatus.STOP;
        targetSpeed = vehicleSettings.maxSpeed;
        currentMotorTorque = vehicleSettings.maxMotorTorque;
    }

    void FixedUpdate()
    {
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * metresPerSecondToKilometresPerHourConversion;
        if (currentSpeed < targetSpeed && currentSpeed < vehicleSettings.maxSpeed)
        {
            SetEngineStatus(VehicleEngine.EngineStatus.ACCELERATE);
        }
        else
        {
            SetEngineStatus(VehicleEngine.EngineStatus.STOP);
        }

    }

    /// <summary>
    /// Set the target speed of the vehicle
    /// </summary>
    /// <param name="targetSpeed">Speed in kilometers per hour</param>
    public void SetTargetSpeed(float targetSpeed)
    {
        this.targetSpeed = targetSpeed;
    }

    /// <summary>
    /// Gets the current engine status of the vehicle.
    /// </summary>
    /// <returns>The current engine status enum of the vehicle.</returns>
    public EngineStatus GetEngineStatus()
    {
        return engineStatus;
    }

    /// <summary>
    /// Set the current engine status of the vehicle.
    /// </summary>
    /// <param name="engineStatus">The status that the vehicle engine should be in.</param>
    public void SetEngineStatus(EngineStatus engineStatus)
    {
        this.engineStatus = engineStatus;
        vehicleSettings.wheelColliderFrontLeft.brakeTorque = 0;
        vehicleSettings.wheelColliderFrontRight.brakeTorque = 0;
        vehicleSettings.wheelColliderFrontLeft.motorTorque = 0;
        vehicleSettings.wheelColliderFrontRight.motorTorque = 0;
        if (engineStatus == EngineStatus.ACCELERATE)
        {
            vehicleSettings.wheelColliderFrontLeft.motorTorque = currentMotorTorque;
            vehicleSettings.wheelColliderFrontRight.motorTorque = currentMotorTorque;
        }
        else if (engineStatus == EngineStatus.STOP)
        {
            vehicleSettings.wheelColliderFrontLeft.brakeTorque = vehicleSettings.normalBrakeTorque;
            vehicleSettings.wheelColliderFrontRight.brakeTorque = vehicleSettings.normalBrakeTorque;
        }
        else if (engineStatus == EngineStatus.HARD_STOP)
        {
            vehicleSettings.wheelColliderFrontLeft.brakeTorque = vehicleSettings.maxBrakeTorque;
            vehicleSettings.wheelColliderFrontRight.brakeTorque = vehicleSettings.maxBrakeTorque;
        }
    }

    public enum EngineStatus
    {
        ACCELERATE,
        STOP,
        HARD_STOP
    }
}

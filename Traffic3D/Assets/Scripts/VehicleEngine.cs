using System;
using UnityEngine;

public class VehicleEngine : MonoBehaviour
{
    public Path path;
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider wheelColliderFrontLeft;
    public WheelCollider wheelColliderFrontRight;
    public float maxMotorTorque = 200f;
    public float normalBrakeTorque = 200f;
    public float maxBrakeTorque = 400f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public float maxSpeedTurning = 20f;
    public float maxSpeedAproachingLightsLastNode = 7f;
    public float maxSpeedAproachingLightsSecondLastNode = 30f;
    public Vector3 centerOfMass;
    public Transform currentNode;
    public int currentNodeNumber;
    private float targetSteerAngle = 0;
    public float targetSpeed;
    public float startTime;
    public Vector3 startPos;
    public float nodeReadingOffset;
    public EngineStatus engineStatus;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        startTime = Time.time;
        startPos = transform.position;
        engineStatus = EngineStatus.STOP;
        targetSpeed = maxSpeed;
    }

    /// <summary>
    /// Sets the path for the vehicle to use.
    /// </summary>
    /// <param name="path">The path for the vehicle to use.</param>
    public void SetPath(Path path)
    {
        this.path = path;
        currentNodeNumber = 1;
        currentNode = path.nodes[currentNodeNumber];
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "car")
        {
            other.gameObject.tag = "hap";
        }
    }

    /// <summary>
    /// The update method to check and update values for the vehicle.
    /// </summary>
    private void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        if (Vector3.Distance(transform.TransformPoint(0, 0, nodeReadingOffset), currentNode.position) < 3f)
        {
            NextNode();
        }
        if (currentNodeNumber == path.nodes.Count - 1)
        {
            Destroy();
            return;
        }
        ApplySteer();
        currentSpeed = 2 * Mathf.PI * wheelColliderFrontLeft.radius * wheelColliderFrontLeft.rpm * 60 / 1000;
        SpeedCheck();
        TrafficLight trafficLight = TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(currentNode);
        if ((trafficLight != null && trafficLight.IsCurrentLightColour(TrafficLight.LightColour.RED)) || this.gameObject.tag == "hap")
        {
            SetEngineStatus(EngineStatus.HARD_STOP);
        }
        else if (currentSpeed < targetSpeed && currentSpeed < maxSpeed)
        {
            SetEngineStatus(EngineStatus.ACCELERATE);
        }
        else
        {
            SetEngineStatus(EngineStatus.STOP);
        }
    }

    /// <summary>
    /// Applies the steer of the vehicle for this current period of time.
    /// </summary>
    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(currentNode.position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelColliderFrontLeft.steerAngle = Mathf.Lerp(newSteer, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelColliderFrontRight.steerAngle = Mathf.Lerp(newSteer, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    /// <summary>
    /// Checks and adjusts the speed accordingly.
    /// Speed in kilometers per hour
    /// </summary>
    private void SpeedCheck()
    {
        // If starting to turn
        if (Math.Abs(wheelColliderFrontLeft.steerAngle) > 2)
        {
            SetTargetSpeed(maxSpeedTurning);
        }
        else
        {
            SetTargetSpeed(maxSpeed);
        }
        if (currentNodeNumber + 1 < path.nodes.Count)
        {
            // If next node is a traffic light
            if (TrafficLightManager.GetInstance().IsStopNode(path.nodes[currentNodeNumber + 1]))
            {
                SetTargetSpeed(maxSpeedAproachingLightsLastNode);
            }
        }
        if (currentNodeNumber + 2 < path.nodes.Count)
        {
            // If 2nd to next node is a traffic light
            if (TrafficLightManager.GetInstance().IsStopNode(path.nodes[currentNodeNumber + 2]))
            {
                SetTargetSpeed(maxSpeedAproachingLightsSecondLastNode);
            }
        }
    }

    /// <summary>
    /// Destroys the vehicle and passes information to the Python Manager.
    /// </summary>
    private void Destroy()
    {
        PythonManager.GetInstance().IncrementDensityCount();
        Vector3 endPos = transform.position;
        double distance = Vector3.Distance(startPos, endPos);
        double time = (Time.time - startTime);
        double speed = (distance / time);
        PythonManager.GetInstance().speedList.Add(speed);
        Destroy(this.gameObject);
        PythonManager.GetInstance().IncrementRewardCount();
        System.IO.File.AppendAllText(System.IO.Path.Combine("Assets", "Results", "VehicleTimes.csv"), time.ToString() + ",");
    }

    /// <summary>
    /// Sets the vehicle to the next node in the path.
    /// </summary>
    private void NextNode()
    {
        if (currentNodeNumber == path.nodes.Count - 1)
        {
            currentNodeNumber = 0;
        }
        else
        {
            currentNodeNumber++;
        }
        currentNode = path.nodes[currentNodeNumber];
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
        wheelColliderFrontLeft.brakeTorque = 0;
        wheelColliderFrontRight.brakeTorque = 0;
        wheelColliderFrontLeft.motorTorque = 0;
        wheelColliderFrontRight.motorTorque = 0;
        if (engineStatus == EngineStatus.ACCELERATE)
        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
        }
        else if (engineStatus == EngineStatus.STOP)
        {
            wheelColliderFrontLeft.brakeTorque = normalBrakeTorque;
            wheelColliderFrontRight.brakeTorque = normalBrakeTorque;
        }
        else if (engineStatus == EngineStatus.HARD_STOP)
        {
            wheelColliderFrontLeft.brakeTorque = maxBrakeTorque;
            wheelColliderFrontRight.brakeTorque = maxBrakeTorque;
        }
    }

    public enum EngineStatus
    {
        ACCELERATE,
        STOP,
        HARD_STOP
    }
}
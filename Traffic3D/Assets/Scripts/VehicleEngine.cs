using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleEngine : MonoBehaviour
{
    public Path path;
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider wheelColliderFrontLeft;
    public WheelCollider wheelColliderFrontRight;
    public float maxMotorTorque = 200f;
    public float currentMotorTorque;
    public float normalBrakeTorque = 200f;
    public float maxBrakeTorque = 400f;
    public float currentSpeed;
    public float stoppingDistance = 7f;
    public float maxSpeed = 100f;
    public float maxSpeedTurning = 20f;
    public float maxSpeedApproachingLightsLastNode = 7f;
    public float maxSpeedApproachingLightsSecondLastNode = 30f;
    public Vector3 centerOfMass;
    public Transform currentNode;
    public int currentNodeNumber;
    private float targetSteerAngle = 0;
    private Renderer renderer;
    public float numberOfSensorRays = 5;
    public float steerReduceRayConstant = 5;
    public float maxDistanceToMonitor = 80;
    private float distanceBetweenRays;
    private float shortestSide;
    private float longestSide;
    public float targetSpeed;
    public float startTime;
    public Vector3 startPos;
    public float nodeReadingOffset;
    public EngineStatus engineStatus;
    public bool densityCountTriggered = false;
    public bool debug = false;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        renderer = GetComponentsInChildren<Renderer>().Aggregate((r1, r2) => (r1.bounds.extents.x * r1.bounds.extents.y * r1.bounds.extents.z) > (r2.bounds.extents.x * r2.bounds.extents.y * r2.bounds.extents.z) ? r1 : r2);
        longestSide = Math.Max(renderer.bounds.size.z, renderer.bounds.size.x);
        shortestSide = Math.Min(renderer.bounds.size.z, renderer.bounds.size.x);
        distanceBetweenRays = (shortestSide / (numberOfSensorRays - 1));
        startTime = Time.time;
        startPos = transform.position;
        engineStatus = EngineStatus.STOP;
        targetSpeed = maxSpeed;
        currentMotorTorque = maxMotorTorque;
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

    private void Update()
    {
        SurfaceCheck();
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
        WheelCollider nonePoweredWheel = GetComponentsInChildren<WheelCollider>().First(wheel => wheel != wheelColliderFrontLeft && wheel != wheelColliderFrontRight);
        currentSpeed = 2 * Mathf.PI * nonePoweredWheel.radius * nonePoweredWheel.rpm * 60 / 1000;
        SpeedCheck();
        SensorCheck();
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
                SetTargetSpeed(maxSpeedApproachingLightsLastNode);
            }
        }
        if (currentNodeNumber + 2 < path.nodes.Count)
        {
            // If 2nd to next node is a traffic light
            if (TrafficLightManager.GetInstance().IsStopNode(path.nodes[currentNodeNumber + 2]))
            {
                SetTargetSpeed(maxSpeedApproachingLightsSecondLastNode);
            }
        }
    }

    private void SensorCheck()
    {
        List<Ray> rays = new List<Ray>();
        RaycastHit hit;
        Vector3 rayPosition = transform.position + transform.TransformDirection(Vector3.up);
        rayPosition = rayPosition + transform.TransformDirection(Vector3.forward) * ((longestSide / 2) - 1) + transform.TransformDirection(Vector3.left) * (shortestSide / 2);
        float angle = (float)Math.PI * wheelColliderFrontLeft.steerAngle / 180f;
        Vector3 direction = transform.TransformDirection(new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle)));
        for (float i = 0; i <= shortestSide; i = i + distanceBetweenRays)
        {
            Ray ray = new Ray(rayPosition + transform.TransformDirection(Vector3.right) * i, direction);
            rays.Add(ray);
        }
        float speedToTarget = targetSpeed;
        float distanceToMonitor = Math.Min(Math.Max(maxDistanceToMonitor - (Math.Abs(angle) * steerReduceRayConstant * maxDistanceToMonitor), stoppingDistance), maxDistanceToMonitor);
        foreach (Ray ray in rays)
        {
            if (Physics.Raycast(ray, out hit, distanceToMonitor, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                if (GetComponentsInChildren<Collider>().Any(c => c.Equals(hit.collider)))
                {
                    continue;
                }
                if (debug)
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, new Color(1, (hit.distance / distanceToMonitor), 0), 0.01f);
                }
                speedToTarget = Math.Min(speedToTarget, (hit.distance - stoppingDistance) / 2);
            }
            else
            {
                if (debug)
                {
                    Debug.DrawRay(ray.origin, ray.direction * distanceToMonitor, Color.green, 0.01f);
                }
            }
        }
        SetTargetSpeed(speedToTarget);
    }

    private void SurfaceCheck()
    {
        WheelHit hit;
        if (wheelColliderFrontLeft.GetGroundHit(out hit))
        {
            float staticFriction = hit.collider.material.staticFriction;
            float dyanmicFriction = hit.collider.material.dynamicFriction;
            foreach (WheelCollider wheelCollider in GetComponentsInChildren<WheelCollider>())
            {
                WheelFrictionCurve wheelFrictionCurveForward = new WheelFrictionCurve();
                wheelFrictionCurveForward.extremumValue = staticFriction;
                wheelFrictionCurveForward.extremumSlip = wheelCollider.forwardFriction.extremumSlip;
                wheelFrictionCurveForward.asymptoteValue = dyanmicFriction;
                wheelFrictionCurveForward.asymptoteSlip = wheelCollider.forwardFriction.asymptoteSlip;
                wheelFrictionCurveForward.stiffness = wheelCollider.forwardFriction.stiffness;
                WheelFrictionCurve wheelFrictionCurveSideways = new WheelFrictionCurve();
                wheelFrictionCurveSideways.extremumValue = staticFriction;
                wheelFrictionCurveSideways.extremumSlip = wheelCollider.sidewaysFriction.extremumSlip;
                wheelFrictionCurveSideways.asymptoteValue = dyanmicFriction;
                wheelFrictionCurveSideways.asymptoteSlip = wheelCollider.sidewaysFriction.asymptoteSlip;
                wheelFrictionCurveSideways.stiffness = wheelCollider.sidewaysFriction.stiffness;
                wheelCollider.forwardFriction = wheelFrictionCurveForward;
                wheelCollider.sidewaysFriction = wheelFrictionCurveSideways;
            }
            currentMotorTorque = Math.Min(maxMotorTorque, maxMotorTorque * dyanmicFriction);
        }
    }

    /// <summary>
    /// Destroys the vehicle and passes information to the Python Manager.
    /// </summary>
    private void Destroy()
    {
        if (!densityCountTriggered)
        {
            densityCountTriggered = true;
            PythonManager.GetInstance().IncrementDensityCount();
        }
        Vector3 endPos = transform.position;
        double distance = Vector3.Distance(startPos, endPos);
        double time = (Time.time - startTime);
        double speed = (distance / time);
        PythonManager.GetInstance().speedList.Add(speed);
        Destroy(this.gameObject);
        PythonManager.GetInstance().IncrementRewardCount();
        Utils.AppendAllTextToResults("VehicleTimes.csv", time.ToString() + ",");
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
            wheelColliderFrontLeft.motorTorque = currentMotorTorque;
            wheelColliderFrontRight.motorTorque = currentMotorTorque;
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
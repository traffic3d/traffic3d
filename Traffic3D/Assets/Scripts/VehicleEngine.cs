using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleEngine : MonoBehaviour
{
    public VehiclePath path;
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
    public List<float> distanceForSpeedCheck = new List<float> { 10, 30, 60, 150 };
    public float cornerThresholdDegrees = 90f;
    public float cornerSensitivityModifier = 50f;
    public float cornerMinSpeed = 10f;
    public float trafficLightDistanceToSpeed = 0.4f;
    public float trafficLightMaxSpeedOnGreen = 20f;
    public float mergeRadiusCheck = 100f;
    public Vector3 centerOfMass;
    public Transform currentNode;
    public int currentNodeNumber;
    private float targetSteerAngle = 0;
    private Renderer renderer;
    public float numberOfSensorRays = 5;
    public float steerReduceRayConstant = 5;
    public float maxDistanceToMonitor = 100;
    private float distanceBetweenRays;
    private float shortestSide;
    private float longestSide;
    public float targetSpeed;
    public float startTime;
    public float startDelayTime = -1;
    public Vector3 startPos;
    public float nodeReadingOffset;
    public EngineStatus engineStatus;
    public bool densityCountTriggered = false;
    public bool debug = false;
    public VehicleEngine waitingForVehicleAhead = null;
    public VehicleEngine waitingForVehicleBehind = null;
    private bool isLeftHandDrive;
    private const float debugSphereSize = 0.25f;
    private const float metresPerSecondToKilometresPerHourConversion = 3.6f;
    private Dictionary<VehicleEngine, HashSet<PathIntersectionPoint>> vehicleIntersectionPoints = new Dictionary<VehicleEngine, HashSet<PathIntersectionPoint>>();

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
        isLeftHandDrive = FindObjectOfType<VehicleFactory>().isLeftHandDrive;
        EventManager.GetInstance().VehicleSpawnEvent += OnVehicleSpawnEvent;
        EventManager.GetInstance().VehicleDestroyEvent += OnVehicleDestroyEvent;
        // Find all vehicles and intersection paths
        foreach (VehicleEngine vehicleEngine in FindObjectsOfType<VehicleEngine>())
        {
            AddVehicleIntersectionPoint(vehicleEngine);
        }
    }

    /// <summary>
    /// Sets the path for the vehicle to use.
    /// </summary>
    /// <param name="path">The path for the vehicle to use.</param>
    public void GenerateVehiclePath(RoadNode startRoadNode)
    {
        SetVehiclePath(RoadNetworkManager.GetInstance().GetValidVehiclePath(startRoadNode));
    }

    public void SetVehiclePath(VehiclePath vehiclePath)
    {
        this.path = vehiclePath;
        currentNodeNumber = 1;
        currentNode = path.nodes[currentNodeNumber];
    }

    public void OnVehicleSpawnEvent(object sender, VehicleEventArgs args)
    {
        AddVehicleIntersectionPoint(args.vehicleEngine);
    }

    public void OnVehicleDestroyEvent(object sender, VehicleEventArgs args)
    {
        vehicleIntersectionPoints.Remove(args.vehicleEngine);
    }

    private void OnDestroy()
    {
        EventManager.GetInstance().VehicleSpawnEvent -= OnVehicleSpawnEvent;
        EventManager.GetInstance().VehicleDestroyEvent -= OnVehicleDestroyEvent;
        EventManager.GetInstance().CallVehicleDestroyEvent(this, new VehicleEventArgs(this));
    }

    public void AddVehicleIntersectionPoint(VehicleEngine otherVehicle)
    {
        if (otherVehicle != null && !otherVehicle.Equals(this))
        {
            vehicleIntersectionPoints.Add(otherVehicle, path.GetIntersectionPoints(otherVehicle.path));
        }
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
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * metresPerSecondToKilometresPerHourConversion;
        SpeedCheck();
        MergeCheck();
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
        if (startDelayTime == -1 && trafficLight != null)
        {
            startDelayTime = Time.time;
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
        // Reset Speed
        // Corner Speed Check
        SetTargetSpeed(maxSpeed);
        // Store the distances with their angles
        Dictionary<float, float> distanceAndAngles = new Dictionary<float, float>();
        foreach (float distanceToCheck in distanceForSpeedCheck)
        {
            float angle = path.GetDirectionDifferenceToRoadAheadByDistanceMeasured(currentNode, transform, distanceToCheck, debug);
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
            double finalSpeed = Math.Pow(cornerSensitivityModifier, -averageAngleDifference / cornerThresholdDegrees) * maxSpeed;
            // Keep final speed between the minimum corner speed and the max speed.
            finalSpeed = Math.Max(cornerMinSpeed, Math.Min(finalSpeed, maxSpeed));
            SetTargetSpeed((float)finalSpeed);
        }
        // Stop Node Check
        Transform nextStopNode = path.GetNextStopNode(currentNode);
        if (nextStopNode != null)
        {
            float distance = path.GetDistanceToNextStopNode(currentNode, transform);
            if (!float.IsNaN(distance) && distance < maxDistanceToMonitor)
            {
                float speed = distance * trafficLightDistanceToSpeed;
                if (TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(nextStopNode).IsCurrentLightColour(TrafficLight.LightColour.RED))
                {
                    SetTargetSpeed(speed);
                }
                else
                {
                    SetTargetSpeed((speed < trafficLightMaxSpeedOnGreen ? trafficLightMaxSpeedOnGreen : speed));
                }
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

    private void MergeCheck()
    {
        foreach (VehicleEngine vehicleEngine in vehicleIntersectionPoints.Keys.Where(v => v == null).ToList())
        {
            vehicleIntersectionPoints.Remove(vehicleEngine);
        }
        Dictionary<PathIntersectionPoint, HashSet<VehicleEngine>> vehiclesAtIntersectionPoint = new Dictionary<PathIntersectionPoint, HashSet<VehicleEngine>>();
        foreach (KeyValuePair<VehicleEngine, HashSet<PathIntersectionPoint>> entry in vehicleIntersectionPoints)
        {
            foreach (PathIntersectionPoint intersectionPoint in entry.Value)
            {
                if (vehiclesAtIntersectionPoint.ContainsKey(intersectionPoint))
                {
                    vehiclesAtIntersectionPoint[intersectionPoint].Add(entry.Key);
                }
                else
                {
                    vehiclesAtIntersectionPoint.Add(intersectionPoint, new HashSet<VehicleEngine>() { entry.Key });
                }
            }
        }
        if (vehiclesAtIntersectionPoint.Count == 0)
        {
            return;
        }
        HashSet<PathIntersectionPoint> pathIntersectionPoints = new HashSet<PathIntersectionPoint>(vehiclesAtIntersectionPoint.Keys.Where(i => path.GetDistanceFromVehicleToIntersectionPoint(path, currentNodeNumber, transform, i) <= mergeRadiusCheck)); //.OrderByDescending(intersectionPoint => path.GetDistanceFromVehicleToIntersectionPoint(currentNode, transform, intersectionPoint)).ToList();
        foreach (PathIntersectionPoint pathIntersectionPoint in pathIntersectionPoints)
        {
            PathIntersectionLine currentIntersectionLine = pathIntersectionPoint.GetLineFromPath(path);
            if ((isLeftHandDrive && !pathIntersectionPoint.IsOtherPathComingFromTheRight(currentIntersectionLine)) ||
                (!isLeftHandDrive && !pathIntersectionPoint.IsOtherPathComingFromTheLeft(currentIntersectionLine)))
            {
                continue;
            }
            if (debug)
            {
                Debug.DrawLine(pathIntersectionPoint.intersection, pathIntersectionPoint.intersection + Vector3.up * 5, Color.red);
            }
            HashSet<VehicleEngine> vehiclesWithSameIntersection = vehiclesAtIntersectionPoint[pathIntersectionPoint];
            float currentDistance = path.GetDistanceFromVehicleToIntersectionPoint(path, currentNodeNumber, transform, pathIntersectionPoint);
            float distanceAhead = float.MaxValue;
            float distanceBehind = float.MaxValue;
            VehicleEngine vehicleAhead = null;
            VehicleEngine vehicleBehind = null;
            foreach (VehicleEngine otherVehicle in vehiclesWithSameIntersection)
            {
                float otherVehicleDistance = otherVehicle.path.GetDistanceFromVehicleToIntersectionPoint(otherVehicle.path, otherVehicle.currentNodeNumber, otherVehicle.transform, pathIntersectionPoint);
                if (float.IsNaN(otherVehicleDistance))
                {
                    // Vehicle is now past intersection point
                    continue;
                }
                float relativeDistance = currentDistance - otherVehicleDistance;
                if (relativeDistance >= 0 && distanceAhead > relativeDistance)
                {
                    distanceAhead = relativeDistance;
                    vehicleAhead = otherVehicle;
                }
                else if (relativeDistance < 0 && distanceBehind > -relativeDistance)
                {
                    distanceBehind = -relativeDistance;
                    vehicleBehind = otherVehicle;
                }
            }
            if (distanceAhead - longestSide <= currentSpeed)
            {
                waitingForVehicleAhead = vehicleAhead;
            }
            else
            {
                waitingForVehicleAhead = null;
            }
            if (distanceBehind <= maxSpeed)
            {
                waitingForVehicleBehind = vehicleBehind;
            }
            else
            {
                waitingForVehicleBehind = null;
            }
            if (distanceAhead - longestSide > currentSpeed && distanceBehind > maxSpeed)
            {
                SetTargetSpeed(Math.Min(targetSpeed, distanceAhead));
            }
            else
            {
                SetTargetSpeed(0);
            }
        }
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
                WheelFrictionCurve wheelFrictionCurveForward = CloneWheelFrictionCurve(wheelCollider.forwardFriction);
                wheelFrictionCurveForward.extremumValue = staticFriction;
                wheelFrictionCurveForward.asymptoteValue = dyanmicFriction;
                WheelFrictionCurve wheelFrictionCurveSideways = CloneWheelFrictionCurve(wheelCollider.sidewaysFriction);
                wheelFrictionCurveSideways.extremumValue = staticFriction;
                wheelFrictionCurveSideways.asymptoteValue = dyanmicFriction;
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
        Utils.AppendAllTextToResults(Utils.VEHICLE_TIMES_FILE_NAME, time.ToString() + ",");
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

    private WheelFrictionCurve CloneWheelFrictionCurve(WheelFrictionCurve wheelFrictionCurveToClone)
    {
        WheelFrictionCurve newWheelFrictionCurve = new WheelFrictionCurve();
        newWheelFrictionCurve.extremumValue = wheelFrictionCurveToClone.extremumValue;
        newWheelFrictionCurve.extremumSlip = wheelFrictionCurveToClone.extremumSlip;
        newWheelFrictionCurve.asymptoteValue = wheelFrictionCurveToClone.asymptoteValue;
        newWheelFrictionCurve.asymptoteSlip = wheelFrictionCurveToClone.asymptoteSlip;
        newWheelFrictionCurve.stiffness = wheelFrictionCurveToClone.stiffness;
        return newWheelFrictionCurve;
    }

    void OnDrawGizmosSelected()
    {
        if (path == null)
        {
            return;
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < path.nodes.Count; i++)
        {
            Vector3 currentNode = path.nodes[i].transform.position;
            Vector3 previousNode = Vector3.zero;
            Vector3 lastNode = Vector3.zero;
            if (i > 0)
            {
                previousNode = path.nodes[i - 1].transform.position;
            }
            else if (i == 0 && path.nodes.Count > 1)
            {
                currentNode = lastNode;
            }
            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, debugSphereSize);
        }
    }
}
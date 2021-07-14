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
    public float stopDistanceToSpeed = 0.4f;
    public float stopLineEvaluationDistance = 10f;
    public float trafficLightMaxSpeedOnGreen = 20f;
    public float mergeRadiusCheck = 100f;
    public Vector3 centerOfMass;
    public Transform currentNode;
    public int currentNodeNumber;
    private float targetSteerAngle = 0;
    private Mesh mainMesh;
    public float numberOfSensorRays = 5;
    public float isWaitingOnSensorRaysDistance = 5;
    public bool isWaitingOnSensorRays = false;
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
    public float deadlockReleaseProceedSpeed = 10f;
    public float releaseDeadlockAfterSeconds = 3;
    public float currentDeadlockSeconds = 0;
    public float deadlockPriorityModifier;
    public bool deadlock;
    public float deadlockReleaseAtTime = 0;
    private int deadlockSearchMaxAttempts = 100;
    private float attemptAnotherDeadlockReleaseAfter = 5f;
    private bool isLeftHandDrive;
    private const float debugSphereSize = 0.25f;
    private const float metresPerSecondToKilometresPerHourConversion = 3.6f;
    private Dictionary<VehicleEngine, HashSet<PathIntersectionPoint>> vehicleIntersectionPoints = new Dictionary<VehicleEngine, HashSet<PathIntersectionPoint>>();

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        mainMesh = GetComponentsInChildren<MeshFilter>().Aggregate((m1, m2) => (m1.mesh.bounds.extents.x * m1.mesh.bounds.extents.y * m1.mesh.bounds.extents.z) > (m2.mesh.bounds.extents.x * m2.mesh.bounds.extents.y * m2.mesh.bounds.extents.z) ? m1 : m2).mesh;
        longestSide = Math.Max(mainMesh.bounds.size.z * transform.lossyScale.z, mainMesh.bounds.size.x * transform.lossyScale.x);
        shortestSide = Math.Min(mainMesh.bounds.size.z * transform.lossyScale.z, mainMesh.bounds.size.x * transform.lossyScale.x);
        distanceBetweenRays = (shortestSide / (numberOfSensorRays - 1));
        startTime = Time.time;
        startPos = transform.position;
        engineStatus = EngineStatus.STOP;
        targetSpeed = maxSpeed;
        currentMotorTorque = maxMotorTorque;
        deadlockPriorityModifier = RandomNumberGenerator.GetInstance().NextFloat();
        isLeftHandDrive = FindObjectOfType<VehicleFactory>().isLeftHandDrive;
        EventManager.GetInstance().VehicleSpawnEvent += OnVehicleSpawnEvent;
        EventManager.GetInstance().VehicleDestroyEvent += OnVehicleDestroyEvent;
        // Find all vehicles and intersection paths
        foreach (VehicleEngine vehicleEngine in FindObjectsOfType<VehicleEngine>())
        {
            AddVehicleIntersectionPoint(vehicleEngine);
        }
        SpeedLimitCheck();
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
    /// The main fixed loop of the vehicle object.
    /// Used for methods that need fixed timings for accurate physics when speeding up the simulation.
    /// The main loop updates multiple values of the vehicle such as:
    /// * The target speed of the vehicle by checking different sensors and conditions.
    /// * The direction of the vehicle by using the path the vehicle is following.
    /// * The current speed by calculating it from the velocity of the Rigidbody.
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
            SpeedLimitCheck();
        }
        if (currentNodeNumber == path.nodes.Count - 1)
        {
            Destroy();
            return;
        }
        ApplySteer();
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * metresPerSecondToKilometresPerHourConversion;
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
        SetTargetSpeed(maxSpeed);
        // Corner Speed Check
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
                float speed = distance * stopDistanceToSpeed;
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
        // Stop Line Check
        StopLine nextStopLine = path.GetNextStopLine(currentNode);
        if (nextStopLine != null)
        {
            float distance = path.GetDistanceToNextStopLine(currentNode, transform);
            if (!float.IsNaN(distance) && distance < maxDistanceToMonitor)
            {
                if (distance <= stopLineEvaluationDistance)
                {
                    MergeCheck(nextStopLine.type);
                }
                else
                {
                    float speed = distance * stopDistanceToSpeed;
                    SetTargetSpeed(Math.Min(Math.Max(0, speed), targetSpeed));
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
        isWaitingOnSensorRays = false;
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
                if ((hit.distance - stoppingDistance) / 2 < isWaitingOnSensorRaysDistance)
                {
                    isWaitingOnSensorRays = true;
                }
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

    private void MergeCheck(StopLine.Type stopLineType)
    {
        float previousTargetSpeed = targetSpeed;
        waitingForVehicleAhead = null;
        waitingForVehicleBehind = null;
        UpdateVehicleIntersectionPointCache();
        Dictionary<PathIntersectionPoint, HashSet<VehicleEngine>> vehiclesAtIntersectionPoint = GetVehiclesAtIntersectionPointsUsingCache();
        if (vehiclesAtIntersectionPoint.Count == 0)
        {
            return;
        }
        // Find intersections within a certain distance of the vehicle
        HashSet<PathIntersectionPoint> pathIntersectionPoints = new HashSet<PathIntersectionPoint>(vehiclesAtIntersectionPoint.Keys.Where(i =>
        {
            if (path.GetDistanceFromVehicleToIntersectionPoint(path, currentNodeNumber, transform, i, out float distanceResult))
            {
                return distanceResult <= mergeRadiusCheck;
            }
            return false;
        }
        ));
        foreach (PathIntersectionPoint pathIntersectionPoint in pathIntersectionPoints)
        {
            if (stopLineType != StopLine.Type.MERGE)
            {
                if (IsOtherPathIncomingFromLookingDirection(pathIntersectionPoint))
                {
                    continue;
                }
            }
            if (debug)
            {
                Debug.DrawLine(pathIntersectionPoint.intersection, pathIntersectionPoint.intersection + Vector3.up * 5, Color.red);
            }
            HashSet<VehicleEngine> vehiclesWithSameIntersection = vehiclesAtIntersectionPoint[pathIntersectionPoint];
            float currentDistance;
            if (!path.GetDistanceFromVehicleToIntersectionPoint(path, currentNodeNumber, transform, pathIntersectionPoint, out currentDistance))
            {
                // Unable to get path distance to intersection
                continue;
            }
            float distanceAhead = float.MaxValue;
            float distanceBehind = float.MaxValue;
            VehicleEngine vehicleAhead = null;
            VehicleEngine vehicleBehind = null;
            foreach (VehicleEngine otherVehicle in vehiclesWithSameIntersection)
            {
                float otherVehicleDistance;
                if (!otherVehicle.path.GetDistanceFromVehicleToIntersectionPoint(otherVehicle.path, otherVehicle.currentNodeNumber, otherVehicle.transform, pathIntersectionPoint, out otherVehicleDistance))
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
            if (distanceBehind <= maxSpeed)
            {
                waitingForVehicleBehind = vehicleBehind;
            }
            ApplyMergeAlgorithm(distanceAhead, distanceBehind);
        }
        // Deadlock check
        deadlock = IsInDeadlock();
        UpdateDeadlockTime(deadlock);
        bool isReleasingDeadlock = IsReleasingDeadlock();
        if (deadlock && !isReleasingDeadlock && ShouldReleaseDeadlock())
        {
            currentDeadlockSeconds = 0;
            deadlockReleaseAtTime = Time.time;
            isReleasingDeadlock = true;
        }
        if (isReleasingDeadlock)
        {
            waitingForVehicleAhead = null;
            waitingForVehicleBehind = null;
            // Bypass merge speed and proceed with caution
            SetTargetSpeed(deadlockReleaseProceedSpeed);
            return;
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

    private void SpeedLimitCheck()
    {
        RoadNode roadNode = path.nodes[currentNodeNumber].GetComponent<RoadNode>();
        List<RoadWay> roadWays = new List<RoadWay>();
        if (currentNodeNumber + 1 < path.nodes.Count)
        {
            RoadNode nextRoadNode = path.nodes[currentNodeNumber + 1].GetComponent<RoadNode>();
            roadWays = RoadNetworkManager.GetInstance().GetRoadWaysFromNodes(roadNode, nextRoadNode);
        }
        else if (currentNodeNumber > 0)
        {
            RoadNode previousRoadNode = path.nodes[currentNodeNumber - 1].GetComponent<RoadNode>();
            roadWays = RoadNetworkManager.GetInstance().GetRoadWaysFromNodes(previousRoadNode, roadNode);
        }
        if (roadWays.Count == 0)
        {
            return;
        }
        // Unlikely to get multiple road ways so just pick the first one.
        maxSpeed = roadWays.First().speedLimit;
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

    private void ApplyMergeAlgorithm(float distanceAhead, float distanceBehind)
    {
        if (distanceAhead - longestSide > currentSpeed && distanceBehind > maxSpeed)
        {
            SetTargetSpeed(Math.Min(targetSpeed, distanceAhead));
        }
        else
        {
            SetTargetSpeed(0);
        }
    }

    /// <summary>
    /// Checks incoming paths from the right when driving on the left and the left when driving on the right
    /// </summary>
    /// <param name="pathIntersectionPoint"></param>
    /// <returns>True if incoming path is not in looking direction</returns>
    /// The diagram below shows which section is classed as the incoming path
    /*
                      |                                                         
                      |                                                         
                      -                                                         
                     /|\                                                        
                    / | \        Incoming Path                                  
                      |     +--------------------+                              
                      |     |                    |                              
                      |     |                    |                              
           /          |     |        /           |                              
          /           |     |       /            |                              
    ------------------------------------------------------                          
          \           |     |       \            |     Other Path               
           \          |     |        \           |                              
                      -     |                    |                              
                     /|\    |                    |                              
                    / | \   +--------------------+                              
                      |                                                         
        Current Path  |                                                         
                      |                                                         
                      |                                                         
    */
    private bool IsOtherPathIncomingFromLookingDirection(PathIntersectionPoint pathIntersectionPoint)
    {
        PathIntersectionLine currentIntersectionLine = pathIntersectionPoint.GetLineFromPath(path);
        return (isLeftHandDrive && !pathIntersectionPoint.IsIncomingPathFromDirection(PathIntersectionPoint.Direction.RIGHT, currentIntersectionLine)) ||
            (!isLeftHandDrive && !pathIntersectionPoint.IsIncomingPathFromDirection(PathIntersectionPoint.Direction.LEFT, currentIntersectionLine));
    }

    /// <summary>
    /// Add all intersection points with this vehicle and another to a list for use in MergeCheck()
    /// </summary>
    /// <param name="otherVehicle">The other vehicle to check</param>
    private void AddVehicleIntersectionPoint(VehicleEngine otherVehicle)
    {
        if (otherVehicle != null && !otherVehicle.Equals(this))
        {
            vehicleIntersectionPoints.Add(otherVehicle, path.GetIntersectionPoints(otherVehicle.path));
        }
    }

    /// <summary>
    /// Ensures that there isn't a vehicle that has already been destroyed and is still in the cache
    /// </summary>
    private void UpdateVehicleIntersectionPointCache()
    {
        foreach (VehicleEngine vehicleEngine in vehicleIntersectionPoints.Keys.Where(v => v == null).ToList())
        {
            vehicleIntersectionPoints.Remove(vehicleEngine);
        }
    }

    /// <summary>
    /// Get the deadlock priority of the current vehicle
    /// </summary>
    /// <returns>Returns priority of vehicle for deadlock releases</returns>
    public float GetDeadlockPriority()
    {
        return deadlockPriorityModifier;
    }

    /// <summary>
    /// Checks if vehicle is currently in a merging deadlock
    /// Find out by checking through the vehicles that are being waited on to see if there is a loop back to the current vehicle.
    /// </summary>
    /// <returns>True if in a merging deadlock</returns>
    public bool IsInDeadlock()
    {
        if (waitingForVehicleBehind == null && waitingForVehicleAhead == null)
        {
            return false;
        }
        List<VehicleEngine> vehiclesToSearch = new List<VehicleEngine>();
        if (waitingForVehicleAhead != null)
        {
            vehiclesToSearch.Add(waitingForVehicleAhead);
        }
        if (waitingForVehicleBehind != null)
        {
            vehiclesToSearch.Add(waitingForVehicleBehind);
        }
        bool isInDeadlock = false;
        for (int i = 0; i < deadlockSearchMaxAttempts; i++)
        {
            if (vehiclesToSearch.Count == 0)
            {
                break;
            }
            VehicleEngine vehicleToSearch = vehiclesToSearch.First();
            // Vehicle has been found in a loop of waiting vehicles.
            if (this.Equals(vehicleToSearch.waitingForVehicleAhead) || this.Equals(vehicleToSearch.waitingForVehicleBehind))
            {
                isInDeadlock = true;
                break;
            }
            if (vehicleToSearch.waitingForVehicleAhead != null)
            {
                if (vehicleToSearch.waitingForVehicleAhead.deadlock)
                {
                    isInDeadlock = true;
                    break;
                }
                vehiclesToSearch.Add(vehicleToSearch.waitingForVehicleAhead);
            }
            if (vehicleToSearch.waitingForVehicleBehind != null)
            {
                if (vehicleToSearch.waitingForVehicleBehind.deadlock)
                {
                    isInDeadlock = true;
                    break;
                }
                vehiclesToSearch.Add(vehicleToSearch.waitingForVehicleBehind);
            }
            if (vehiclesToSearch.Count == 1 && vehicleToSearch.waitingForVehicleAhead == null && vehicleToSearch.waitingForVehicleBehind == null && vehicleToSearch.isWaitingOnSensorRays)
            {
                isInDeadlock = true;
                break;
            }
            vehiclesToSearch.Remove(vehicleToSearch);
        }
        return isInDeadlock;
    }

    /// <summary>
    /// Updates the deadlock value which is used to find out when to release the current deadlock.
    /// </summary>
    /// <param name="isInDeadlock">True if the vehicle is in deadlock</param>
    private void UpdateDeadlockTime(bool isInDeadlock)
    {
        if (isInDeadlock)
        {
            currentDeadlockSeconds = currentDeadlockSeconds + Time.fixedDeltaTime;
        }
        else
        {
            currentDeadlockSeconds = currentDeadlockSeconds - Time.fixedDeltaTime;
        }
        if (currentDeadlockSeconds < 0)
        {
            currentDeadlockSeconds = 0;
        }
    }

    /// <summary>
    /// Checks if the vehicle should release the deadlock
    /// </summary>
    /// <returns>True if the deadlock should be released</returns>
    private bool ShouldReleaseDeadlock()
    {
        if (currentDeadlockSeconds < releaseDeadlockAfterSeconds)
        {
            return false;
        }
        List<VehicleEngine> vehiclesInArea = FindObjectsOfType<VehicleEngine>().Where(v => Vector3.Distance(v.transform.position, transform.position) <= mergeRadiusCheck).OrderByDescending(v => v.GetDeadlockPriority()).ToList();
        List<VehicleEngine> vehiclesInAreaOriginal = new List<VehicleEngine>(vehiclesInArea);
        while (vehiclesInArea.Count > 0)
        {
            VehicleEngine vehicleToCheck = vehiclesInArea.First();
            if (this.Equals(vehicleToCheck))
            {
                return true;
            }
            if (vehicleToCheck.IsReleasingDeadlock() || !vehicleToCheck.isWaitingOnSensorRays)
            {
                return false;
            }
            vehiclesInArea.Remove(vehicleToCheck);
        }
        // This is the last vehicle so should release
        return true;
    }

    public bool IsReleasingDeadlock()
    {
        return deadlockReleaseAtTime + attemptAnotherDeadlockReleaseAfter > Time.time;
    }

    /// <summary>
    /// Gets a Dictionary of intersection points with the vehicles that will be passing through that intersection point
    /// </summary>
    /// <returns>A dictionary of interesection points with vehicles</returns>
    private Dictionary<PathIntersectionPoint, HashSet<VehicleEngine>> GetVehiclesAtIntersectionPointsUsingCache()
    {
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
        return vehiclesAtIntersectionPoint;
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


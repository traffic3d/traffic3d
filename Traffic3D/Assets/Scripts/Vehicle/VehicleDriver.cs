using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleDriver : MonoBehaviour
{
    public Vehicle vehicle;
    public VehicleSettings vehicleSettings;
    public VehiclePath path;
    public Transform currentNode;
    public int currentNodeNumber;
    private float targetSteerAngle = 0;
    public bool isWaitingOnSensorRays = false;
    private float distanceBetweenRays;
    public float startTime;
    public float startDelayTime = -1;
    public Vector3 startPos;
    public bool densityCountTriggered = false;
    public bool debug = false;
    public VehicleDriver waitingForVehicleDriverAhead = null;
    public VehicleDriver waitingForVehicleDriverBehind = null;
    public float currentDeadlockSeconds = 0;
    public float deadlockPriorityModifier;
    public bool deadlock;
    public float deadlockReleaseAtTime = 0;
    private float shortestSide;
    private float longestSide;
    private const int deadlockSearchMaxAttempts = 100;
    private const float attemptAnotherDeadlockReleaseAfter = 5f;
    private bool isLeftHandDrive;
    private const float debugSphereSize = 0.25f;
    public Dictionary<Vehicle, HashSet<PathIntersectionPoint>> vehicleIntersectionPoints = new Dictionary<Vehicle, HashSet<PathIntersectionPoint>>();

    private void Awake()
    {
        vehicle = gameObject.GetComponent<Vehicle>();
        vehicleSettings = gameObject.GetComponent<VehicleSettings>();
    }

    void Start()
    {
        Mesh mainMesh = GetComponentsInChildren<MeshFilter>().Aggregate((m1, m2) => (m1.mesh.bounds.extents.x * m1.mesh.bounds.extents.y * m1.mesh.bounds.extents.z) > (m2.mesh.bounds.extents.x * m2.mesh.bounds.extents.y * m2.mesh.bounds.extents.z) ? m1 : m2).mesh;
        longestSide = Math.Max(mainMesh.bounds.size.z * transform.lossyScale.z, mainMesh.bounds.size.x * transform.lossyScale.x);
        shortestSide = Math.Min(mainMesh.bounds.size.z * transform.lossyScale.z, mainMesh.bounds.size.x * transform.lossyScale.x);
        GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        distanceBetweenRays = (shortestSide / (vehicleSettings.numberOfSensorRays - 1));
        startTime = Time.time;
        startPos = transform.position;
        deadlockPriorityModifier = RandomNumberGenerator.GetInstance().NextFloat();
        isLeftHandDrive = FindObjectOfType<VehicleFactory>().isLeftHandDrive;
        // Find all vehicles and intersection paths
        foreach (Vehicle vehicle in FindObjectsOfType<Vehicle>())
        {
            AddVehicleIntersectionPoint(vehicle);
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
        if (Vector3.Distance(transform.TransformPoint(0, 0, vehicleSettings.nodeReadingOffset), currentNode.position) < 3f)
        {
            NextNode();
            SpeedLimitCheck();
        }
        if (currentNodeNumber == path.nodes.Count - 1)
        {
            vehicle.DestroyVehicle();
            return;
        }
        ApplySteer();
        SpeedCheck();
        SensorCheck();
        TrafficLight trafficLight = TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(currentNode);
        if ((trafficLight != null && trafficLight.IsCurrentLightColour(TrafficLight.LightColour.RED)) || this.gameObject.tag == "hap")
        {
            vehicle.vehicleEngine.SetTargetSpeed(0);
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
        float newSteer = (relativeVector.x / relativeVector.magnitude) * vehicleSettings.maxSteerAngle;
        vehicleSettings.wheelColliderFrontLeft.steerAngle = Mathf.Lerp(newSteer, targetSteerAngle, Time.deltaTime * vehicleSettings.turnSpeed);
        vehicleSettings.wheelColliderFrontRight.steerAngle = Mathf.Lerp(newSteer, targetSteerAngle, Time.deltaTime * vehicleSettings.turnSpeed);
    }

    /// <summary>
    /// Checks and adjusts the speed accordingly.
    /// Speed in kilometers per hour
    /// </summary>
    private void SpeedCheck()
    {
        // Reset Speed
        vehicle.vehicleEngine.SetTargetSpeed(vehicleSettings.maxSpeed);
        // Corner Speed Check
        // Store the distances with their angles
        Dictionary<float, float> distanceAndAngles = new Dictionary<float, float>();
        foreach (float distanceToCheck in vehicleSettings.distanceForSpeedCheck)
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
            double finalSpeed = Math.Pow(vehicleSettings.cornerSensitivityModifier, -averageAngleDifference / vehicleSettings.cornerThresholdDegrees) * vehicleSettings.maxSpeed;
            // Keep final speed between the minimum corner speed and the max speed.
            finalSpeed = Math.Max(vehicleSettings.cornerMinSpeed, Math.Min(finalSpeed, vehicleSettings.maxSpeed));
            vehicle.vehicleEngine.SetTargetSpeed((float)finalSpeed);
        }
        // Stop Node Check
        Transform nextStopNode = path.GetNextStopNode(currentNode);
        if (nextStopNode != null)
        {
            float distance = path.GetDistanceToNextStopNode(currentNode, transform);
            if (!float.IsNaN(distance) && distance < vehicleSettings.maxDistanceToMonitor)
            {
                float speed = distance * vehicleSettings.stopDistanceToSpeed;
                if (TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(nextStopNode).IsCurrentLightColour(TrafficLight.LightColour.RED))
                {
                    vehicle.vehicleEngine.SetTargetSpeed(speed);
                }
                else
                {
                    vehicle.vehicleEngine.SetTargetSpeed((speed < vehicleSettings.trafficLightMaxSpeedOnGreen ? vehicleSettings.trafficLightMaxSpeedOnGreen : speed));
                }
            }
        }
        // Stop Line Check
        StopLine nextStopLine = path.GetNextStopLine(currentNode);
        if (nextStopLine != null)
        {
            float distance = path.GetDistanceToNextStopLine(currentNode, transform);
            if (!float.IsNaN(distance) && distance < vehicleSettings.maxDistanceToMonitor)
            {
                if (distance <= vehicleSettings.stopLineEvaluationDistance)
                {
                    MergeCheck(nextStopLine.type);
                }
                else
                {
                    float speed = distance * vehicleSettings.stopDistanceToSpeed;
                    vehicle.vehicleEngine.SetTargetSpeed(Math.Min(Math.Max(0, speed), vehicle.vehicleEngine.targetSpeed));
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
        float angle = (float)Math.PI * vehicleSettings.wheelColliderFrontLeft.steerAngle / 180f;
        Vector3 direction = transform.TransformDirection(new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle)));
        for (float i = 0; i <= shortestSide; i = i + distanceBetweenRays)
        {
            Ray ray = new Ray(rayPosition + transform.TransformDirection(Vector3.right) * i, direction);
            rays.Add(ray);
        }
        float speedToTarget = vehicle.vehicleEngine.targetSpeed;
        float distanceToMonitor = Math.Min(Math.Max(vehicleSettings.maxDistanceToMonitor - (Math.Abs(angle) * vehicleSettings.steerReduceRayConstant * vehicleSettings.maxDistanceToMonitor), vehicleSettings.stoppingDistance), vehicleSettings.maxDistanceToMonitor);
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
                speedToTarget = Math.Min(speedToTarget, (hit.distance - vehicleSettings.stoppingDistance) / 2);
                if ((hit.distance - vehicleSettings.stoppingDistance) / 2 < vehicleSettings.isWaitingOnSensorRaysDistance)
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
        vehicle.vehicleEngine.SetTargetSpeed(speedToTarget);
    }

    private void MergeCheck(StopLine.Type stopLineType)
    {
        float previousTargetSpeed = vehicle.vehicleEngine.targetSpeed;
        waitingForVehicleDriverAhead = null;
        waitingForVehicleDriverBehind = null;
        UpdateVehicleIntersectionPointCache();
        Dictionary<PathIntersectionPoint, HashSet<Vehicle>> vehiclesAtIntersectionPoint = GetVehiclesAtIntersectionPointsUsingCache();
        if (vehiclesAtIntersectionPoint.Count == 0)
        {
            return;
        }
        // Find intersections within a certain distance of the vehicle
        HashSet<PathIntersectionPoint> pathIntersectionPoints = new HashSet<PathIntersectionPoint>(vehiclesAtIntersectionPoint.Keys.Where(i =>
        {
            if (path.GetDistanceFromVehicleToIntersectionPoint(path, currentNodeNumber, transform, i, out float distanceResult))
            {
                return distanceResult <= vehicleSettings.mergeRadiusCheck;
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
            HashSet<Vehicle> vehiclesWithSameIntersection = vehiclesAtIntersectionPoint[pathIntersectionPoint];
            float currentDistance;
            if (!path.GetDistanceFromVehicleToIntersectionPoint(path, currentNodeNumber, transform, pathIntersectionPoint, out currentDistance))
            {
                // Unable to get path distance to intersection
                continue;
            }
            float distanceAhead = float.MaxValue;
            float distanceBehind = float.MaxValue;
            VehicleDriver vehicleAhead = null;
            VehicleDriver vehicleBehind = null;
            foreach (Vehicle otherVehicle in vehiclesWithSameIntersection)
            {
                float otherVehicleDistance;
                if (!otherVehicle.vehicleDriver.path.GetDistanceFromVehicleToIntersectionPoint(otherVehicle.vehicleDriver.path, otherVehicle.vehicleDriver.currentNodeNumber, otherVehicle.transform, pathIntersectionPoint, out otherVehicleDistance))
                {
                    // Vehicle is now past intersection point
                    continue;
                }
                float relativeDistance = currentDistance - otherVehicleDistance;
                if (relativeDistance >= 0 && distanceAhead > relativeDistance)
                {
                    distanceAhead = relativeDistance;
                    vehicleAhead = otherVehicle.vehicleDriver;
                }
                else if (relativeDistance < 0 && distanceBehind > -relativeDistance)
                {
                    distanceBehind = -relativeDistance;
                    vehicleBehind = otherVehicle.vehicleDriver;
                }
            }
            if (distanceAhead - longestSide <= vehicle.vehicleEngine.currentSpeed)
            {
                waitingForVehicleDriverAhead = vehicleAhead;
            }
            if (distanceBehind <= vehicleSettings.maxSpeed)
            {
                waitingForVehicleDriverBehind = vehicleBehind;
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
            waitingForVehicleDriverAhead = null;
            waitingForVehicleDriverBehind = null;
            // Bypass merge speed and proceed with caution
            vehicle.vehicleEngine.SetTargetSpeed(vehicleSettings.deadlockReleaseProceedSpeed);
            return;
        }
    }

    private void SurfaceCheck()
    {
        WheelHit hit;
        if (vehicleSettings.wheelColliderFrontLeft.GetGroundHit(out hit))
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
            vehicle.vehicleEngine.currentMotorTorque = Math.Min(vehicleSettings.maxMotorTorque, vehicleSettings.maxMotorTorque * dyanmicFriction);
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
        vehicleSettings.maxSpeed = roadWays.First().speedLimit;
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

    private void ApplyMergeAlgorithm(float distanceAhead, float distanceBehind)
    {
        if (distanceAhead - longestSide > vehicle.vehicleEngine.currentSpeed && distanceBehind > vehicleSettings.maxSpeed)
        {
            vehicle.vehicleEngine.SetTargetSpeed(Math.Min(vehicle.vehicleEngine.targetSpeed, distanceAhead));
        }
        else
        {
            vehicle.vehicleEngine.SetTargetSpeed(0);
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
    public void AddVehicleIntersectionPoint(Vehicle otherVehicle)
    {
        if (otherVehicle != null && !otherVehicle.vehicleDriver.Equals(this))
        {
            vehicleIntersectionPoints.Add(otherVehicle, path.GetIntersectionPoints(otherVehicle.vehicleDriver.path));
        }
    }

    /// <summary>
    /// Ensures that there isn't a vehicle that has already been destroyed and is still in the cache
    /// </summary>
    private void UpdateVehicleIntersectionPointCache()
    {
        foreach (Vehicle vehicle in vehicleIntersectionPoints.Keys.Where(v => v == null).ToList())
        {
            vehicleIntersectionPoints.Remove(vehicle);
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
        if (waitingForVehicleDriverBehind == null && waitingForVehicleDriverAhead == null)
        {
            return false;
        }
        List<VehicleDriver> vehiclesToSearch = new List<VehicleDriver>();
        if (waitingForVehicleDriverAhead != null)
        {
            vehiclesToSearch.Add(waitingForVehicleDriverAhead);
        }
        if (waitingForVehicleDriverBehind != null)
        {
            vehiclesToSearch.Add(waitingForVehicleDriverBehind);
        }
        bool isInDeadlock = false;
        for (int i = 0; i < deadlockSearchMaxAttempts; i++)
        {
            if (vehiclesToSearch.Count == 0)
            {
                break;
            }
            VehicleDriver vehicleDriverToSearch = vehiclesToSearch.First();
            // Vehicle has been found in a loop of waiting vehicles.
            if (this.Equals(vehicleDriverToSearch.waitingForVehicleDriverAhead) || this.Equals(vehicleDriverToSearch.waitingForVehicleDriverBehind))
            {
                isInDeadlock = true;
                break;
            }
            if (vehicleDriverToSearch.waitingForVehicleDriverAhead != null)
            {
                if (vehicleDriverToSearch.waitingForVehicleDriverAhead.deadlock)
                {
                    isInDeadlock = true;
                    break;
                }
                vehiclesToSearch.Add(vehicleDriverToSearch.waitingForVehicleDriverAhead);
            }
            if (vehicleDriverToSearch.waitingForVehicleDriverBehind != null)
            {
                if (vehicleDriverToSearch.waitingForVehicleDriverBehind.deadlock)
                {
                    isInDeadlock = true;
                    break;
                }
                vehiclesToSearch.Add(vehicleDriverToSearch.waitingForVehicleDriverBehind);
            }
            if (vehiclesToSearch.Count == 1 && vehicleDriverToSearch.waitingForVehicleDriverAhead == null && vehicleDriverToSearch.waitingForVehicleDriverBehind == null && vehicleDriverToSearch.isWaitingOnSensorRays)
            {
                isInDeadlock = true;
                break;
            }
            vehiclesToSearch.Remove(vehicleDriverToSearch);
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
        if (currentDeadlockSeconds < vehicleSettings.releaseDeadlockAfterSeconds)
        {
            return false;
        }
        List<Vehicle> vehiclesInArea = FindObjectsOfType<Vehicle>().Where(v => Vector3.Distance(v.transform.position, transform.position) <= vehicleSettings.mergeRadiusCheck).OrderByDescending(v => v.vehicleDriver.GetDeadlockPriority()).ToList();
        List<Vehicle> vehiclesInAreaOriginal = new List<Vehicle>(vehiclesInArea);
        while (vehiclesInArea.Count > 0)
        {
            Vehicle vehicleToCheck = vehiclesInArea.First();
            if (this.Equals(vehicleToCheck.vehicleDriver))
            {
                return true;
            }
            if (vehicleToCheck.vehicleDriver.IsReleasingDeadlock() || !vehicleToCheck.vehicleDriver.isWaitingOnSensorRays)
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
    private Dictionary<PathIntersectionPoint, HashSet<Vehicle>> GetVehiclesAtIntersectionPointsUsingCache()
    {
        Dictionary<PathIntersectionPoint, HashSet<Vehicle>> vehiclesAtIntersectionPoint = new Dictionary<PathIntersectionPoint, HashSet<Vehicle>>();
        foreach (KeyValuePair<Vehicle, HashSet<PathIntersectionPoint>> entry in vehicleIntersectionPoints)
        {
            foreach (PathIntersectionPoint intersectionPoint in entry.Value)
            {
                if (vehiclesAtIntersectionPoint.ContainsKey(intersectionPoint))
                {
                    vehiclesAtIntersectionPoint[intersectionPoint].Add(entry.Key);
                }
                else
                {
                    vehiclesAtIntersectionPoint.Add(intersectionPoint, new HashSet<Vehicle>() { entry.Key });
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


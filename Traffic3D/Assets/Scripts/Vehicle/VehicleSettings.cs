using System.Collections.Generic;
using UnityEngine;

public class VehicleSettings : MonoBehaviour
{
    // Vehicle Driver Properties
    public float stoppingDistance = 7f;
    public List<float> distanceForSpeedCheck = new List<float> { 10, 30, 60, 150 };
    public float cornerThresholdDegrees = 90f;
    public float cornerSensitivityModifier = 50f;
    public float cornerMinSpeed = 10f;
    public float stopDistanceToSpeed = 0.4f;
    public float stopLineEvaluationDistance = 10f;
    public float trafficLightMaxSpeedOnGreen = 20f;
    public float mergeRadiusCheck = 100f;
    public float numberOfSensorRays = 5;
    public float isWaitingOnSensorRaysDistance = 5;
    public float steerReduceRayConstant = 5;
    public float maxDistanceToMonitor = 100;
    public float deadlockReleaseProceedSpeed = 10f;
    public float releaseDeadlockAfterSeconds = 3;
    public float longestSideLength = -1f;
    public float shortestSideLength = -1f;
    // Vehicle Navigation Properties
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public float nodeReadingOffset = 0f;
    // Vehicle Engine Properties
    public float normalBrakeTorque = 200f;
    public float maxBrakeTorque = 400f;
    public float maxSpeed = 100f;
    public float maxMotorTorque = 200f;
    public WheelCollider wheelColliderFrontLeft;
    public WheelCollider wheelColliderFrontRight;
}

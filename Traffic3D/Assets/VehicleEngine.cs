using System.Collections.Generic;
using UnityEngine;

public class VehicleEngine : MonoBehaviour
{
    public Path path;
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider wheelColliderFrontLeft;
    public WheelCollider wheelColliderFrontRight;
    public float maxMotorTorque = 80f;
    public float normalBrakeTorque = 100f;
    public float maxBrakeTorque = 100f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public Transform currentNode;
    public int currentNodeNumber;
    private float targetSteerAngle = 0;
    public float startTime;
    public Vector3 startPos;
    public EngineStatus engineStatus;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        startTime = Time.time;
        startPos = transform.position;
        engineStatus = EngineStatus.STOP;
    }

    public void SetPath(Path path)
    {
        this.path = path;
        currentNodeNumber = 0;
        currentNode = path.nodes[currentNodeNumber];
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "car")
        {
            other.gameObject.tag = "hap";
        }
    }

    private void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, currentNode.position) < 3f)
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
        TrafficLight trafficLight = TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(currentNode);
        if ((trafficLight != null && trafficLight.IsCurrentLightColour(TrafficLight.LightColour.RED)) || this.gameObject.tag == "hap")
        {
            SetEngineStatus(EngineStatus.HARD_STOP);
        }
        else if (currentSpeed < maxSpeed)
        {
            SetEngineStatus(EngineStatus.ACCELERATE);
        }
        else
        {
            SetEngineStatus(EngineStatus.STOP);
        }
    }

    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(currentNode.position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelColliderFrontLeft.steerAngle = Mathf.Lerp(newSteer, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelColliderFrontRight.steerAngle = Mathf.Lerp(newSteer, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void Destroy()
    {
        PythonManager.IncrementDensityCount1();
        Vector3 endPos = transform.position;
        double distance = Vector3.Distance(startPos, endPos);
        double time = (Time.time - startTime);
        double speed = (distance / time);
        PythonManager.speedlist.Add(speed);
        Destroy(this.gameObject);
        PythonManager.IncrementRewardCount();
        System.IO.File.AppendAllText("VehicleTimes.csv", time.ToString() + ",");
    }

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

    public EngineStatus GetEngineStatus()
    {
        return engineStatus;
    }

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
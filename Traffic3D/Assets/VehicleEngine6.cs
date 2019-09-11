using System.Collections.Generic;
using UnityEngine;

public class VehicleEngine6 : MonoBehaviour
{

    public Path path;

    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider wheelColliderFrontLeft;
    public WheelCollider wheelColliderFrontRight;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 100f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;

    public Material redMaterial;
    public Transform currentNode;
    public int currentNodeNumber;
    private int lapCounter = 0;
    private float targetSteerAngle = 0;

    public float k;
    public float startTime;
    public bool frus = false;
    public bool des = false;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        startTime = Time.time;
    }

    public void SetPath(Path path)
    {
        this.path = path;
        currentNodeNumber = 0;
        currentNode = path.nodes[currentNodeNumber];
    }

    void OnCollisionEnter(Collision other)
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
        ApplySteer();
        Drive(1);
        CheckWaypointDistance();
        Destroy();
        LerpToSteerAngle();
        StopAtLineIfRedElseGo();
        TurnOff();
        GoIfNotRed();
        GoIfTagRid();
    }

    private void GoIfTagRid()
    {
        if (this.gameObject.tag == "rid")
        {

            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
            wheelColliderFrontLeft.brakeTorque = 0;
            wheelColliderFrontRight.brakeTorque = 0;
        }
    }

    private void GoIfNotRed()
    {
        TrafficLight trafficLight = TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(currentNode);

        if (trafficLight == null || !trafficLight.IsCurrentLightColour(TrafficLight.LightColour.RED))
        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
            wheelColliderFrontLeft.brakeTorque = 0;
            wheelColliderFrontRight.brakeTorque = 0;
        }
    }

    private void TurnOff()
    {
        if (this.gameObject.tag == "hap")
        {

            wheelColliderFrontLeft.motorTorque = 0;
            wheelColliderFrontRight.motorTorque = 0;
            wheelColliderFrontLeft.brakeTorque = maxBrakeTorque;
            wheelColliderFrontRight.brakeTorque = maxBrakeTorque;
        }
    }

    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(currentNode.position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelColliderFrontLeft.steerAngle = newSteer;
        wheelColliderFrontRight.steerAngle = newSteer;
    }

    private void Drive(int numlaps)
    {
        currentSpeed = 2 * Mathf.PI * wheelColliderFrontLeft.radius * wheelColliderFrontLeft.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && lapCounter < numlaps)
        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
        }
        else
        {
            wheelColliderFrontLeft.motorTorque = 0;
            wheelColliderFrontRight.motorTorque = 0;
            wheelColliderFrontLeft.brakeTorque = maxBrakeTorque;
            wheelColliderFrontRight.brakeTorque = maxBrakeTorque;
        }
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, currentNode.position) < 1.5f)
        {
            NextNode();
        }
    }


    private void Destroy()
    {
        if (currentNodeNumber == path.nodes.Count - 1)
        {

            Destroy(this.gameObject);

            PythonManager.IncrementRewardCount();

            k = (Time.time - startTime);

            System.IO.File.AppendAllText("negjourneyTimeLatest1.csv", k.ToString() + ",");

        }

    }

    private void LerpToSteerAngle()
    {
        wheelColliderFrontLeft.steerAngle = Mathf.Lerp(wheelColliderFrontLeft.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelColliderFrontRight.steerAngle = Mathf.Lerp(wheelColliderFrontRight.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void StopAtLineIfRedElseGo()
    {

        TrafficLight trafficLight = TrafficLightManager.GetInstance().GetTrafficLightFromStopNode(currentNode);

        if (trafficLight != null && trafficLight.IsCurrentLightColour(TrafficLight.LightColour.RED))
        {
            wheelColliderFrontLeft.motorTorque = 0;
            wheelColliderFrontRight.motorTorque = 0;
            wheelColliderFrontLeft.brakeTorque = maxBrakeTorque;
            wheelColliderFrontRight.brakeTorque = maxBrakeTorque;

        }
        else
        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
            wheelColliderFrontLeft.brakeTorque = 0;
            wheelColliderFrontRight.brakeTorque = 0;
        }
    }

    private void NextNode()
    {
        if (currentNodeNumber == path.nodes.Count - 1)
        {
            currentNodeNumber = 0;
            lapCounter++;
        }
        else
        {
            currentNodeNumber++;
            currentNode = path.nodes[currentNodeNumber];
        }
    }

}



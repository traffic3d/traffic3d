using System.Collections.Generic;
using UnityEngine;

public class VehicleEngine2 : MonoBehaviour
{

    public Transform path;
    public Transform path1;
    public Transform path2;

    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider wheelColliderFrontLeft;
    public WheelCollider wheelColliderFrontRight;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 100f;
    public float maxBrakeTorque2 = 1000f;

    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;

    public Material redMaterial;
    public TrafficLight trafficLight = null;

    public List<Transform> nodes;

    public Transform currentNode;
    public int currentNodeNumber;
    private int lapCounter = 0;
    private float targetSteerAngle = 0;

    public static float k;
    public float startTime;
    public bool des = false;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        path1 = GameObject.Find("mypathy").GetComponent<Transform>();
        path2 = GameObject.Find("mypathy1").GetComponent<Transform>();
        trafficLight = TrafficLightManager.GetInstance().GetTrafficLight(3);

        path = path1;

        startTime = Time.time;

        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        currentNodeNumber = 0;
        currentNode = nodes[currentNodeNumber];

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
        ApplySteer();
        Drive(1);
        CheckWaypointDistance();
        Destroy();
        LerpToSteerAngle();
        StopAtLineIfRedElseGo();
        GoIfNotRed();
        GoIfSecondToLastNode();
    }

    private void GoIfNotRed()
    {
        if (!trafficLight.IsCurrentLightColour(TrafficLight.LightColour.RED))
        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
            wheelColliderFrontLeft.brakeTorque = 0;
            wheelColliderFrontRight.brakeTorque = 0;
        }

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


    private void GoIfSecondToLastNode()
    {
        if (currentNodeNumber == nodes.Count - 2)
        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
            wheelColliderFrontLeft.brakeTorque = 0;
            wheelColliderFrontRight.brakeTorque = 0;
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
        if (Vector3.Distance(transform.position, currentNode.position) < 3f)
        {
            NextNode();
        }
    }

    private void Destroy()
    {
        if (currentNodeNumber == nodes.Count - 1)
        {
            Destroy(this.gameObject);
            CarFactoryCounter3.DecrementCarCount();

            //to get the generated car count
            OverallCarCounter.IncrementOverallCarCount();

            k = Time.time - startTime;
            System.IO.File.AppendAllText("xFourjourneyTimeLatest.csv", k.ToString() + ",");
        }
    }

    private void LerpToSteerAngle()
    {
        wheelColliderFrontLeft.steerAngle = Mathf.Lerp(wheelColliderFrontLeft.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelColliderFrontRight.steerAngle = Mathf.Lerp(wheelColliderFrontRight.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void NextNode()
    {
        if (currentNodeNumber == nodes.Count - 1)
        {
            currentNodeNumber = 0;
            lapCounter++;
        }
        else
        {
            currentNodeNumber++;
            currentNode = nodes[currentNodeNumber];
        }
    }

}
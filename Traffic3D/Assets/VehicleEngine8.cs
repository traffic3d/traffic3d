using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEngine8 : MonoBehaviour
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
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;

    public Material redMaterial;
    public TrafficLightRed2 trafficLightRed2 = null;

    public List<Transform> nodes;
    public int currentNode = 0;
    private int lapCounter = 0;
    private float targetSteerAngle = 0;

    public float k;
    public float startTime;

    public bool frus = false;
    public bool des = false;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        path1 = GameObject.Find("mypath1a").GetComponent<Transform>();
        path2 = GameObject.Find("mypath21").GetComponent<Transform>();

        trafficLightRed2 = GameObject.Find("SphereTL2").GetComponent<TrafficLightRed2>();

        startTime = Time.time;

        path = path1;

        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {

                nodes.Add(pathTransforms[i]);
            }
        }

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
        {
            if (this.gameObject.tag == "rid")
            {

                wheelColliderFrontLeft.motorTorque = maxMotorTorque;
                wheelColliderFrontRight.motorTorque = maxMotorTorque;
                wheelColliderFrontLeft.brakeTorque = 0;
                wheelColliderFrontRight.brakeTorque = 0;
            }
        }

    }

    private void GoIfNotRed()
    {
        if (!(trafficLightRed2.currentMaterial.color.Equals(redMaterial.color)))
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
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
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
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 1.5f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
                lapCounter++;
            }
            else
            {
                currentNode++;
            }
        }
    }


    private void Destroy()
    {
        if (currentNode == nodes.Count - 1)
        {


            Destroy(this.gameObject);
            CarFactoryCounter2.DecrementCarCount();
            TrafficLightManagerWithAI.IncrementRewardCount();

            //to get the generated car count
            OverallCarCounter.IncrementOverallCarCount();

            JourneyCarCounter.IncrementJourneyCarCount();
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
        if ((trafficLightRed2.currentMaterial.color.Equals(redMaterial.color)) && (currentNode == nodes.Count - 4))
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

}

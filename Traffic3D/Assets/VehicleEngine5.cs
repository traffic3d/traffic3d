using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEngine5 : MonoBehaviour
{


    public Transform path;
    public Transform path1;
    public Transform path2;

    public GameObject trafficlight;

    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider wheelColliderFrontLeft;
    public WheelCollider wheelColliderFrontRight;
    public float maxMotorTorque = 70f;
    public float maxBrakeTorque = 250f;

    public float maxBrakeTorque2 = 1000f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public Rigidbody vehicle;
    public float range1 = 2f;
    public float range2 = 12f;

    public Material redMaterial;

    public TrafficLightRed4 trafficLightRed4 = null;

    public List<Transform> nodes;
    public List<Transform> materialChange;
    public int currentNode = 0;
    private int lapCounter = 0;
    private float targetSteerAngle = 0;

    public float startTime;
    public static float k;
    public bool des = false;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        path1 = GameObject.Find("mypath4").GetComponent<Transform>();
        path2 = GameObject.Find("mypath41").GetComponent<Transform>();

        trafficlight = GameObject.Find("SphereTL4");
        trafficLightRed4 = trafficlight.GetComponent<TrafficLightRed4>();

        startTime = Time.time;

        if (Random.value > 0.5)
        {

            path = path1;
        }
        else
        {

            path = path1;
        }

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

    public void SetUpPath(Transform[] pathTransforms)
    {

        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
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

        GoIfSecondToLastNode();
        TurnOff();

    }

    private void GoIfTagDrive()
    {
        if (this.gameObject.tag == "drive")
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


    private void GoIfNotRed()
    {
        if (!(trafficLightRed4.currrentMaterial.color.Equals(redMaterial.color)))

        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
            wheelColliderFrontLeft.brakeTorque = 0;
            wheelColliderFrontRight.brakeTorque = 0;
        }

    }

    private void GoIfTagUnhap()
    {
        if (this.gameObject.tag == "unhap")
        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
            wheelColliderFrontLeft.brakeTorque = 0;
            wheelColliderFrontRight.brakeTorque = 0;

        }
    }




    private void GoIfDesFalseAndTagDrive()
    {
        if (des == false)
        {
            if (this.gameObject.tag == "drive")
            {
                TrafficLightManagerWithAI.IncrementRewardCount();
                des = true;
            }

        }
    }


    private void StopAtLineIfRedElseGo()
    {

        if (currentNode == nodes.Count - 3 && trafficLightRed4.currrentMaterial.color.Equals(redMaterial.color))

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

        if (currentNode == nodes.Count - 2)


        {
            wheelColliderFrontLeft.motorTorque = maxMotorTorque;
            wheelColliderFrontRight.motorTorque = maxMotorTorque;
            wheelColliderFrontLeft.brakeTorque = 0;
            wheelColliderFrontRight.brakeTorque = 0;
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

        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 3f)
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
            CarFactoryCounter4.DecrementCarCount();

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

}


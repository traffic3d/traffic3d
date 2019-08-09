using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEngine1 : MonoBehaviour
{



    public Transform path;
    public Transform path1;
    public Transform path2;

    public GameObject trafficLight;

    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 100f;
    public float maxBrakeTorque2 = 1000f;

    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public Rigidbody VEHICLE;
    public Vector3 VehicleCurrentPosition;
    public Vector3 TrafficLightPosition;

    public COUNTER n = null;

    public Material material2;
    public Material material4;
    public TrafficLightRed3 u = null;


    public List<Transform> nodes;


    public int currentNode = 0;
    private int lapCounter = 0;
    public int count = 0;
    private float targetSteerAngle = 0;

    public static float k;
    public float startTime;
    public bool des = false;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        path1 = GameObject.Find("mypathye").GetComponent<Transform>();
        path2 = GameObject.Find("mypathy1").GetComponent<Transform>();
        trafficLight = GameObject.Find("SphereTL3");
        u = trafficLight.GetComponent<TrafficLightRed3>();


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

    public void setUpPath(Transform[] pathTransforms)
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
        Instantiate();
        LerpToSteerAngle();
        juststop();
        keepgoing();
        goo();
    }


    private void redtest()
    {
        if (u.currentMaterial.color.Equals(material2.color))
        {
            print("red1");
            WheelFL.motorTorque = 0;
            WheelFR.motorTorque = 0;
            WheelFL.brakeTorque = maxBrakeTorque;
            WheelFR.brakeTorque = maxBrakeTorque;
        }
    }

    private void engineoff()
    {
        if (this.gameObject.tag == "hap")
        {
            WheelFL.motorTorque = 0;
            WheelFR.motorTorque = 0;
            WheelFL.brakeTorque = maxBrakeTorque;
            WheelFR.brakeTorque = maxBrakeTorque;
        }
    }


    private void keepgoing()
    {
        if (!(u.currentMaterial.color.Equals(material2.color)))

        {
            WheelFL.motorTorque = maxMotorTorque;
            WheelFR.motorTorque = maxMotorTorque;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
        }

    }

    private void going()
    {
        if (this.gameObject.tag == "unhap")
        {
            WheelFL.motorTorque = maxMotorTorque;
            WheelFR.motorTorque = maxMotorTorque;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;

        }
    }

    private void got()
    {
        if (this.gameObject.tag == "drive")
        {
            WheelFL.motorTorque = maxMotorTorque;
            WheelFR.motorTorque = maxMotorTorque;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
        }

    }

    private void go()
    {
        if (des == false)
        {
            if (this.gameObject.tag == "drive")
            {
                freshLOOPING.incrementRew();
                des = true;
            }

        }

    }
    private void juststop()
    {

        if (currentNode == nodes.Count - 3 && u.currentMaterial.color.Equals(material2.color))
        {
            WheelFL.motorTorque = 0;
            WheelFR.motorTorque = 0;
            WheelFL.brakeTorque = maxBrakeTorque2;
            WheelFR.brakeTorque = maxBrakeTorque2;

        }
        else
        {
            WheelFL.motorTorque = maxMotorTorque;
            WheelFR.motorTorque = maxMotorTorque;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
        }
    }


    private void goo()
    {

        if (currentNode == nodes.Count - 2)
        {
            WheelFL.motorTorque = maxMotorTorque;
            WheelFR.motorTorque = maxMotorTorque;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
        }
    }

    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        WheelFL.steerAngle = newSteer;
        WheelFR.steerAngle = newSteer;
    }
    private void Drive(int numlaps)
    {
        currentSpeed = 2 * Mathf.PI * WheelFL.radius * WheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && lapCounter < numlaps)
        {
            WheelFL.motorTorque = maxMotorTorque;
            WheelFR.motorTorque = maxMotorTorque;
        }
        else
        {
            WheelFL.motorTorque = 0;
            WheelFR.motorTorque = 0;
            WheelFL.brakeTorque = maxBrakeTorque;
            WheelFR.brakeTorque = maxBrakeTorque;
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
            carCounterFACTORY3.decrementCarCount();

            //to get the generated car count
            incrementCountNumber.incrementcarC();

            k = Time.time - startTime;
            System.IO.File.AppendAllText("xFourjourneyTimeLatest.csv", k.ToString() + ",");
        }


    }


    private void Instantiate()
    {

        {

            {

            }
        }

    }

    private void LerpToSteerAngle()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }


}
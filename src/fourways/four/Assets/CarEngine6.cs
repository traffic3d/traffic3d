using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine6 : MonoBehaviour
{

    public Transform path;
    public GameObject trafficLight;
    public GameObject cameraObject;
    public GameObject CountCars;


    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 100f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public Rigidbody VEHICLE;
    public Vector3 spawnSpot = new Vector3(-37.83f, 11.28f, 14.96f);
    public Vector3 VehicleCurrentPosition;
    public Vector3 TrafficLightPosition;

    public float range1 = 2f;
    public float range2 = 12f;
    public Material material2;
    public materialchangeeee m = null;
    public TLaction2 p = null;
    public COUNTER n = null;
    public List<Transform> nodes;
    public List<Transform> MaterialChange;
    public int currentNode = 0;
    private int lapCounter = 0;
    private float targetSteerAngle = 0;

    public newCarCount carCount;

    public float k;
    public float startTime;



    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        trafficLight = GameObject.Find("TrafficLight2");
        path = GameObject.Find("mypath1").GetComponent<Transform>();

        m = trafficLight.GetComponent<materialchangeeee>();
        p = trafficLight.GetComponent<TLaction2>();
        startTime = Time.time;

        cameraObject = GameObject.Find("Main Camera");

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


    private void FixedUpdate()
    {

        ApplySteer();
        Drive(1);
        CheckWaypointDistance();
        Destroy();
        Instantiate();
        LerpToSteerAngle();
        Stop();

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
            freshLOOPING.incrementRew();

            newCarCount.decrementCarCount();


            incrementCountNumber.incrementcarC();

            k = (Time.time - startTime);
            System.IO.File.AppendAllText("journeyTimeLatest.csv", k.ToString() + "," + System.Environment.NewLine);
        }


    }


    private void Instantiate()
    {
        if (currentNode == nodes.Count - 1)

        {

        }

    }


    private void LerpToSteerAngle()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void Stop()
    {
        Vector3 a = GetComponent<Transform>().position;
        Vector3 b = trafficLight.GetComponent<Transform>().position;
        if ((p.CM.color.Equals(material2.color)) && (currentNode == nodes.Count - 3))
        {
            WheelFL.motorTorque = 0;
            WheelFR.motorTorque = 0;
            WheelFL.brakeTorque = maxBrakeTorque;
            WheelFR.brakeTorque = maxBrakeTorque;

        }

        else
        {
            WheelFL.motorTorque = maxMotorTorque;
            WheelFR.motorTorque = maxMotorTorque;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
        }
    }

    private void brakeCar()
    {
        GameObject[] cars;
        cars = GameObject.FindGameObjectsWithTag("car");
        foreach (GameObject car in cars)
        {

            if (car.gameObject != this.gameObject)
            {
                if (Mathf.Abs(this.transform.position.z - car.transform.position.z) < 0.02f && Mathf.Abs(this.transform.position.z - car.transform.position.z) != 0)
                {
                    Debug.Log(car.gameObject.name);
                    Debug.Log("-------------------------Breakkkkkkk-------------------------");
                    Debug.Log(Mathf.Abs(this.transform.position.z - car.transform.position.z) + "lesssss distance");

                }
            }

        }
    }


}



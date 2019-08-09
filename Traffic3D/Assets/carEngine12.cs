﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carEngine12 : MonoBehaviour
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
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public Rigidbody VEHICLE;

    public Material material2;
    public TrafficLightRed2 r = null;

    public COUNTER n = null;
    public List<Transform> nodes;
    public List<Transform> MaterialChange;
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

        trafficLight = GameObject.Find("SphereTL2");
        r = trafficLight.GetComponent<TrafficLightRed2>();

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
        Instantiate();
        LerpToSteerAngle();
        Stop();
        engineoff();
        go();

        keepgoing();

    }




    private void keepgoing()
    {
        {
            if (this.gameObject.tag == "rid")
            {

                WheelFL.motorTorque = maxMotorTorque;
                WheelFR.motorTorque = maxMotorTorque;
                WheelFL.brakeTorque = 0;
                WheelFR.brakeTorque = 0;
            }
        }

    }
    private void go()
    {
        if (!(r.currentMaterial.color.Equals(material2.color)))
        {
            WheelFL.motorTorque = maxMotorTorque;
            WheelFR.motorTorque = maxMotorTorque;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
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
            newCarCount.decrementCarCount();
            freshLOOPING.incrementRew();

            //to get the generated car count
            incrementCountNumber.incrementcarC();

            journeyTimeCARCOUNTER.incrementjourneyCARsCount();
            k = (Time.time - startTime);
            System.IO.File.AppendAllText("negjourneyTimeLatest1.csv", k.ToString() + ",");
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
        if ((r.currentMaterial.color.Equals(material2.color)) && (currentNode == nodes.Count - 4))
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



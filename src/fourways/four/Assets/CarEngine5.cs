﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine5 : MonoBehaviour {


    public Transform path;
    public GameObject trafficlight;
    public GameObject CAR4;

    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public float maxMotorTorque = 70f;
    public float maxBrakeTorque = 250f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public Rigidbody VEHICLE;
    // public Vector3 spawnSpot = new Vector3(-20.31f, 13.85f, 5.46f);
    public Vector3 VehicleCurrentPosition;
    public Vector3 TrafficLightPosition;
    public float range1 = 2f;
    public float range2 = 12f;
 //   public Material Material1;
   // public Material Material3;
    public materialchangeee m = null;

	public Material material2;

	public TLaction4 r = null;

    public List<Transform> nodes;
    public List<Transform> MaterialChange;
    public int currentNode = 0;
    private int lapCounter = 0;
    private float targetSteerAngle = 0;



    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

		path = GameObject.Find("mypath4").GetComponent<Transform>();
		trafficlight = GameObject.Find("TrafficLight4");
		r = trafficlight.GetComponent<TLaction4>();

       // TrafficLightPosition = TrafficLight.transform.position;
       // m = TrafficLight.GetComponent<materialchangeee>();


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
        //Sensors();

        // if (!(m.CM.color.Equals(Material1.color) && ((Vector3.Distance(transform.position, TrafficLightPosition) <= range1)))) //&& (Vector3.Distance(transform.position, TrafficLightPosition) >= range2))))

        ApplySteer();
        Drive(1);
        CheckWaypointDistance();
        Destroy();
        //Instantiate();
        LerpToSteerAngle();
        //Stop();
		juststop();



    }
	private void juststop()
	{
		if (r.CM.color.Equals(material2.color) && currentNode == nodes.Count - 3)

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


    /*private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + frontSensorPosition;
        sensorStartPos.z =+ frontSensorPosition; 

        // front center sensor
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
        }


        // front right sensor
        sensorStartPos.x += frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
        }


        // front right angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
        }


        // front left sensor
        sensorStartPos.x -= 2 * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
        }


        // front left angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {

        }
        Debug.DrawLine(sensorStartPos, hit.point);
    }
    */




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
        //print(Vector3.Distance(transform.position, nodes[currentNode].position));
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 3f)
        {
            //  print("updating current node from " + currentNode +"   " + nodes.Count);
            if (currentNode == nodes.Count - 1)
            {                                                    // print("updating current node from " + currentNode +"   " + nodes.Count);
                currentNode = 0;
                lapCounter++;
            }                                                           //currentNode = (currentNode + 1) % (nodes.Count);
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
			carCounterFactory4.decrementCarCount(); 

			freshLOOPING.incrementRew ();
        }


    }


    /* private void Instantiate()
     {

         {
             if (currentNode == nodes.Count - 1)


                 Instantiate(VEHICLE, spawnSpot, Quaternion.identity);

         }

     }  */

    private void LerpToSteerAngle()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void Stop()
    {
        //GameObject go = GameObject.FindWithTag("TrafficLight"); 
        Vector3 a = transform.position;
     //   Vector3 b = TrafficLight.transform.position;
        //if ( ( (m.CM.color.Equals(Material1.color)  && (Vector3.Distance(transform.position, TrafficLight.transform.position) < 8f) && (Vector3.Distance(transform.position, CAR4.transform.position) < 15f))))
		if((r.CM.color.Equals(material2.color)) && (currentNode == nodes.Count - 3))
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



}


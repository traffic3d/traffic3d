using System.Collections;
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
    public Vector3 VehicleCurrentPosition;
    public Vector3 TrafficLightPosition;
    public float range1 = 2f;
    public float range2 = 12f;
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
        LerpToSteerAngle();
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
			carCounterFactory4.decrementCarCount(); 

			freshLOOPING.incrementRew ();
        }


    }

    private void LerpToSteerAngle()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void Stop()
    {
        Vector3 a = transform.position;
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


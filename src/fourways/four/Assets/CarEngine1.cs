using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine1 : MonoBehaviour
{

    public Transform path;
    public GameObject trafficlight;
    public GameObject CAR1;

    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public float maxMotorTorque = 70f;
    public float maxBrakeTorque = 250f;
    public float currentSpeed;
    public float maxSpeed = 80f;
    public Vector3 centerOfMass;
    public Rigidbody VEHICLE1;
   public Vector3 spawnSpot = new Vector3(-17.38f, 11.28f, -13.73f);
    public Vector3 VehicleCurrentPosition;
    public Vector3 TrafficLightPosition;
    public float range1 = 2f;
    public float range2 = 12f;
    public Material Material1;
    public Material Material3;
    public Material Material5;

	public TLaction3 s = null;
	public Material material2;


    public List<Transform> nodes;
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
		trafficlight = GameObject.Find("TrafficLight3");
		s = trafficlight.GetComponent<TLaction3>();


		path = GameObject.Find("mypathx").GetComponent<Transform>();
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

		juststop ();

    }

	private void juststop()
	{
		if (currentNode == nodes.Count - 3 && s.CM.color.Equals(material2.color)) {
			WheelFL.motorTorque = 0;
			WheelFR.motorTorque = 0;
			WheelFL.brakeTorque = maxBrakeTorque;
			WheelFR.brakeTorque = maxBrakeTorque;
		
		} else {
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
			carCounterFACTORY3.decrementCarCount ();

			freshLOOPING.incrementRew ();
        }


    }


     private void Instantiate()
     {

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
		if((s.CM.color.Equals(material2.color)) && (currentNode == nodes.Count - 4))
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

  



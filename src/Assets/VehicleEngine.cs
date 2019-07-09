using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEngine : MonoBehaviour
{

    public Transform path;
    public GameObject TrafficLight;
    
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
    public Vector3 spawnSpot = new Vector3(-43.59f, 11.28f, 14.96f);
    public Vector3 VehicleCurrentPosition;
    public Vector3 TrafficLightPosition;
    public float range1 = 2f;
    public float range2 = 12f;
    public Material Material1;
    public Material Material3;
    public Material Material5;
    public MaterialChange m = null;
    public COUNTER n = null;
    


    public List<Transform> nodes;
    public List<Transform> MaterialChange;
    

    public int currentNode = 0;
    private int lapCounter = 0;
    public int count = 0;
    private float targetSteerAngle = 0;

    

    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        TrafficLightPosition = TrafficLight.transform.position;
        m = TrafficLight.GetComponent<MaterialChange>();
      
        


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
        if (currentNode == nodes.Count - 3)
        {
            Destroy(this.gameObject);
        }


    }


     private void Instantiate()
     {

         {
            if (currentNode == nodes.Count - 1)

            {
                Instantiate(VEHICLE, spawnSpot, Quaternion.identity);
            }
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
        Vector3 b = TrafficLight.transform.position;
        if ((((m.CM.color.Equals(Material1.color) && (Vector3.Distance(transform.position, TrafficLight.transform.position) < 8f)))))

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


  


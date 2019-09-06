using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceVehicleEngine : MonoBehaviour
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

    public Material material2;
    public Material material4;
    public TrafficLightRed1 m = null;

    public float range1 = 2f;
    public float range2 = 12f;

    public Counter n = null;
    public List<Transform> nodes;
    public List<Transform> nodes1;

    public int currentNode = 0;
    private int lapCounter = 0;
    private float targetSteerAngle = 0;

    public CarFactoryCounter1 carCount;

    public static float k;
    public float startTime;
    public bool des = false;


    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        trafficLight = GameObject.Find("TrafficLight1");
        path1 = GameObject.Find("newpath2").GetComponent<Transform>();
        path2 = GameObject.Find("newpath21").GetComponent<Transform>();     //turning path
        m = trafficLight.GetComponent<TrafficLightRed1>();

        startTime = Time.time;


        if (Random.value > 0.5)
        {

            path = path1;
        }
        else
        {

            path = path2;
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
        Stop();
        go();
        engineoff();
        keepgoing();
        got();


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
        if (!(m.currentMaterial.color.Equals(material2.color)))

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

    private void go()
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
            CarFactoryCounter1.DecrementCarCount();

            OverallCarCounter.IncrementOverallCarCount();    //to get generated car number

            k = (Time.time - startTime);
            System.IO.File.AppendAllText("xFourjourneyTimeLatest.csv", k.ToString() + ",");
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
        if ((m.currentMaterial.color.Equals(material2.color)) && (currentNode == nodes.Count - 3))
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

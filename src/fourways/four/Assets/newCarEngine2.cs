using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newCarEngine2 : MonoBehaviour {
    public Transform path;
    public GameObject trafficLight;
    //public GameObject Counter;
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

	//public Material material1;
	public Material material2;
	public TLaction1 q = null;

    public float range1 = 2f;
    public float range2 = 12f;
   // public Material Material1;
    //public Material Material3;
   // public Material Material5;
    public materialchangeeee m = null;
    public COUNTER n = null;
    public List<Transform> nodes;
    public List<Transform> MaterialChange;
    public int currentNode = 0;
    private int lapCounter = 0;
    private float targetSteerAngle = 0;

    public CarCounter carCount;

	public float k;
	public float startTime;



    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        //TrafficLightPosition = TrafficLight.transform.position;

       // trafficLight = GameObject.Find("TrafficLight (1)");
		trafficLight = GameObject.Find("TrafficLight1");
        path = GameObject.Find("newpath3").GetComponent<Transform>();

        //carCount = GameObject.Find("CarCount").GetComponent<CarCounter>();
		q = trafficLight.GetComponent<TLaction1>();
		m = trafficLight.GetComponent<materialchangeeee>();
        //n = Counter.GetComponent<COUNTER>();
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

        // if (CarCounter.getCarCount() < CarCounter.maxCarNumbers)
        //{
        //Debug.Log(CarCounter.getCarCount());
        //StartCoroutine(waitInstantiate());
        //CarCounter.incrementCarCount();
        //}
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

  /*  void OnBecameInvisible()
    {
        //enabled = false;
        transform.gameObject.tag = "Untagged";
        CarCounter.decrementCarCount();
        // Debug.Log("untagged");
    }
    void OnBecameVisible()
    {
        //enabled = true;
        transform.gameObject.tag = "car";
        //Debug.Log("Car");
    }   */


    private void FixedUpdate()
    {

        ApplySteer();
        Drive(1);
        CheckWaypointDistance();
        Destroy();
        Instantiate();
        LerpToSteerAngle();
        Stop();
        //brakeCar();


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
        //print(Vector3.Distance(transform.position, nodes[currentNode].position));
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 1.5f)
        {
            //print("updating current node from " + currentNode +"   " + nodes.Count);
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
			CarCounter.decrementCarCount();
			freshLOOPING.incrementRew ();

			incrementCountNumber.incrementcarC();

			k = (Time.time - startTime);
			System.IO.File.AppendAllText("journeyTimeLatest.csv", k.ToString() + "," + System.Environment.NewLine);

            //this.gameObject.SetActive(false);
            //Destroy(this.gameObject);
            //  COUNTER counterscript = (COUNTER)Counter.GetComponent(typeof(COUNTER));
            //  counterscript.decrement_counter();
            //print("Destroy");
        }


    }


    private void Instantiate()
    {
        if (currentNode == nodes.Count - 1)

        {
            //Invoke("noWaitInstantiate", 5);
            //StartCoroutine(waitInstantiate());
            //Instantiate(VEHICLE, spawnSpot, Quaternion.identity);
            //if (GameObject.Find("Camera") != null) GameObject.Find("Camera").SetActive(false);
            //Debug.Log("Instantiate");
            //COUNTER counterscript = (COUNTER)Counter.GetComponent(typeof(COUNTER));
            // counterscript.increment_counter();

            //counterscript.writeTextFile(string.Format("{0}/{1:D04} shot.txt", "ScreenshotMovieOutput", Time.frameCount), string.Format("{0:D04}", Time.frameCount));
            // ScreenImages screenImage = (ScreenImages)cameraObject.GetComponent(typeof(ScreenImages));

            //screenImage.takeScreenshot();

        }

    }

  /*  IEnumerator waitInstantiate()
    {
        //Debug.Log("Instantiate");
        yield return new WaitForSeconds(Random.Range(3, 6));
        //Debug.Log("After wait");
        Instantiate(VEHICLE, spawnSpot, Quaternion.identity);
        if (GameObject.Find("Camera") != null) GameObject.Find("Camera").SetActive(false);
        //CarCounter.incrementCarCount();
        //Destroy(this.gameObject);
    }

    void noWaitInstantiate()
    {
        Debug.Log("Instantiate");
        Instantiate(VEHICLE, spawnSpot, Quaternion.identity);
        if (GameObject.Find("Camera") != null) GameObject.Find("Camera").SetActive(false);
        Destroy(this.gameObject);

    } */


    private void LerpToSteerAngle()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void Stop()
    {
        //GameObject go = GameObject.FindWithTag("TrafficLight"); 
        Vector3 a = GetComponent<Transform>().position;
        //Debug.Log(trafficLight.GetComponent<Transform>().position);
        Vector3 b = trafficLight.GetComponent<Transform>().position;
        //Debug.Log(TrafficLight.transform.position);
        //Debug.Log((m.CM.color.Equals(Material1.color)));
        //Debug.Log(transform.position);
       // if ((((m.CM.color.Equals(Material1.color) && (currentNode == nodes.Count - 3)))))
        //(Vector3.Distance(a, TrafficLightPosition) < 5f)
	//	if((q.CM.color.Equals(material1)) && (q.CM.color.Equals(material2)) && (currentNode == nodes.Count - 3))

		if((q.CM.color.Equals(material2.color)) && (currentNode == nodes.Count - 3))
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
            //if (Vector3.Distance(car.transform.position, this.transform.position) < 10f){

            if (car.gameObject != this.gameObject)
            {
                //Debug.Log(Mathf.Abs(this.transform.position.z - car.transform.position.z) + " distance");
                if (Mathf.Abs(this.transform.position.z - car.transform.position.z) < 0.02f && Mathf.Abs(this.transform.position.z - car.transform.position.z) != 0)
                {
                    Debug.Log(car.gameObject.name);
                    Debug.Log("-------------------------Breakkkkkkk-------------------------");
                    Debug.Log(Mathf.Abs(this.transform.position.z - car.transform.position.z) + "lesssss distance");
                    //WheelFL.motorTorque = 0;
                    //WheelFR.motorTorque = 0;
                    //WheelFL.brakeTorque = maxBrakeTorque;
                    //WheelFR.brakeTorque = maxBrakeTorque;

                }
            }

        }
    }


}



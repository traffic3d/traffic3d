using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEngine : MonoBehaviour
{

    public Transform path;
	public Transform path1;
	public Transform path2;

    public GameObject trafficLight;
    //public GameObject Counter;
    
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
    //public Vector3 spawnSpot = new Vector3(-43.59f, 11.28f, 14.96f);
    public Vector3 VehicleCurrentPosition;
    public Vector3 TrafficLightPosition;
  //  public float range1 = 2f;
   // public float range2 = 12f;
   
	public COUNTER n = null;
    
	public Material material2;
	public Material material4;
	public TLaction3 u = null;


    public List<Transform> nodes;
    //public List<Transform> MaterialChange;
    

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
		path1 = GameObject.Find("mypathy").GetComponent<Transform>();
		path2 = GameObject.Find("mypathy1").GetComponent<Transform>();
		trafficLight = GameObject.Find("SphereTL3");
		u = trafficLight.GetComponent<TLaction3>();


		startTime = Time.time;
   
      
		if (Random.value > 0.5) {

			path = path1;
		} else {

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

	/*void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "car") {

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


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "car")
		{
			//print ("collision6 detected");
			WheelFL.motorTorque = 0;
			WheelFR.motorTorque = 0;
		//	WheelFL.brakeTorque = maxBrakeTorque2;
		//	WheelFR.brakeTorque = maxBrakeTorque2;
			maxSpeed = 0;
			currentSpeed = 0;
		}
	}    */


	public void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "car")
		{
			other.gameObject.tag = "hap";
		}
	}

/*	public void OnCollisionExit(Collision other)
	{
		if (other.gameObject.tag == "hap")
		{
			other.gameObject.tag = "unhap";
		}
	}   */


    private void FixedUpdate()
    {
   

        ApplySteer();
        Drive(1);
        CheckWaypointDistance();
        Destroy();
        Instantiate();
        LerpToSteerAngle();
       // Stop();
		juststop();
		//redtest ();
		//amberstop();
		//go();

	//	engineoff();
		keepgoing();
		//going();
	goo();
	//	got();
    }


	private void redtest()
	{
		if (u.CM.color.Equals(material2.color)) {
			print ("red1");
			WheelFL.motorTorque = 0;
			WheelFR.motorTorque = 0;
			WheelFL.brakeTorque = maxBrakeTorque;
			WheelFR.brakeTorque = maxBrakeTorque;
		}
	}     

	private void engineoff()
	{
		if(this.gameObject.tag == "hap")
		{
			WheelFL.motorTorque = 0;
			WheelFR.motorTorque = 0;
			WheelFL.brakeTorque = maxBrakeTorque;
			WheelFR.brakeTorque = maxBrakeTorque;
		}
	}


	private void keepgoing()
	{
		if(!(u.CM.color.Equals(material2.color)))

		{
			WheelFL.motorTorque = maxMotorTorque;
			WheelFR.motorTorque = maxMotorTorque;
			WheelFL.brakeTorque = 0;
			WheelFR.brakeTorque = 0; 	
		}

	}

	private void going()
	{
		if (this.gameObject.tag == "unhap") {
			WheelFL.motorTorque = maxMotorTorque;
			WheelFR.motorTorque = maxMotorTorque;
			WheelFL.brakeTorque = 0;
			WheelFR.brakeTorque = 0; 

		}
	}

	private void got()
	{
		if (this.gameObject.tag == "drive") {
			WheelFL.motorTorque = maxMotorTorque;
			WheelFR.motorTorque = maxMotorTorque;
			WheelFL.brakeTorque = 0;
			WheelFR.brakeTorque = 0; 
		}

	}

	private void go()
	{
if(des == false)		
{
if (this.gameObject.tag == "drive") {
			//WheelFL.motorTorque = maxMotorTorque;
		//	WheelFR.motorTorque = maxMotorTorque;
		//	WheelFL.brakeTorque = 0;
		//	WheelFR.brakeTorque = 0; 
freshLOOPING.incrementRew ();
des = true;
		}

	}

}
	private void juststop()
	{

		if (currentNode == nodes.Count - 3 && u.CM.color.Equals(material2.color))
		{
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


	private void goo()
	{

		if (currentNode == nodes.Count - 2 )
		 {
			WheelFL.motorTorque = maxMotorTorque;
			WheelFR.motorTorque = maxMotorTorque;
			WheelFL.brakeTorque = 0;
			WheelFR.brakeTorque = 0;
		}
	}

	/*private void amberstop()
	{
	
		if (currentNode == nodes.Count - 3 && u.CM.color.Equals(material4.color))
		{
			WheelFL.motorTorque = 0;
			WheelFR.motorTorque = 0;
			WheelFL.brakeTorque = maxBrakeTorque;
			WheelFR.brakeTorque = maxBrakeTorque;

		} 
		else {
			WheelFL.motorTorque = maxMotorTorque;
			WheelFR.motorTorque = maxMotorTorque;
			WheelFL.brakeTorque = 0;
			WheelFR.brakeTorque = 0;
		}  */
	


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
			carCounterFACTORY3.decrementCarCount ();

			//freshLOOPING.incrementRew ();
			incrementCountNumber.incrementcarC();   //to get the generated car count

			k = Time.time - startTime;
//System.IO.File.AppendAllText("AFourjourneyTimeLatest.csv", k.ToString() + ",");
System.IO.File.AppendAllText("xFourjourneyTimeLatest.csv", k.ToString() + ",");
        }


    }


     private void Instantiate()
     {

         {
          //  if (currentNode == nodes.Count - 1)

            {
                //COUNTER counterscript=(COUNTER)Counter.GetComponent(typeof(COUNTER));
                //counterscript.increment_counter();
               // Instantiate(VEHICLE, spawnSpot, Quaternion.identity);
            }
         }

     }  

    private void LerpToSteerAngle()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }


}


    //private void Stop()
    //{
        //GameObject go = GameObject.FindWithTag("TrafficLight"); 
       // Vector3 a = transform.position;
      //  Vector3 b = TrafficLight.transform.position;
      //  if ((((m.CM.color.Equals(Material1.color) && (Vector3.Distance(transform.position, TrafficLight.transform.position) < 8f)))))
//		if((t.CM.color.Equals(material2.color)) && (currentNode == nodes.Count - 3))
     /*   {
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
        } */
   
	//} 






    

                                             //   currentNode = (currentNode + 1) % (nodes.Count);

  


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class freshLOOPING : MonoBehaviour {

	private const int port = 13000;
	byte[] bytes = new byte[256];
	public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	public GameObject trafficlight1;
	public GameObject trafficlight2;

       public GameObject waitcars1;
       public GameObject waitcars2;
       public GameObject waitcars3;
	public TLaction1 m = null;
	public TLaction2 n = null;

	public bool waiting = false;

	public static int shot_count=0;
	public static int rewCount = 0;
       
         public static int finalrew=0;

	public bool yes = false;
        List<GameObject> mylist = new List<GameObject>();

       public static int densitycount1;
       public static double densityperkm;
       public static double averagespeed;
       public static double flow;
     
       public static List<double> speedlist = new List<double>();

	void Start () {

		trafficlight1 = GameObject.Find("TrafficLight1");
		m = trafficlight1.GetComponent<TLaction1> ();

		trafficlight2 = GameObject.Find("TrafficLight2");
		n = trafficlight2.GetComponent<TLaction2> ();

		socket.Connect ("localhost", port);


		StartCoroutine (looping ());
	
	}


	public IEnumerator looping()
	{
		
	while (true) 
		{
			yield return StartCoroutine (zero());
			yield return StartCoroutine (takeshot());
			yield return StartCoroutine (soc());
			yield return StartCoroutine (action());
			yield return StartCoroutine (another());
                        yield return StartCoroutine (densitycal());
			yield return StartCoroutine (rewards());
		

	}

	}

   public IEnumerator zero()
	{
		if (waiting == false) 
		{
			m.materialchangeRED1 ();
			n.materialchangeRED2 ();
			yield return new WaitForSeconds(20);
			Time.timeScale = 0;
			waiting = true;
		}
	}


	public IEnumerator takeshot()
	{
		shot_count += 1; 
		
ScreenCapture.CaptureScreenshot("/media/gargd/Fast/MFDimg/shot"+ shot_count + ".png");	

		yield return null;
	}

	public IEnumerator soc()
	{
		byte[] msg = Encoding.UTF8.GetBytes ("shot"+ shot_count + ".png");
		socket.Send(msg);
		yield return null;

	}

	public IEnumerator action()
	{
		socket.Receive (bytes);

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 0) {
n.materialchangeRED2();			
Time.timeScale = 1;
yield return new WaitForSeconds (6);
m.materialchangeGREEN1 ();

		} 

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 1) {

m.materialchangeRED1();
Time.timeScale = 1;			
yield return new WaitForSeconds (6);
n.materialchangeGREEN2 ();

}
yield return null;
}

	public IEnumerator another()
	{
		yield return new WaitForSeconds(10);  
	
	}


        public IEnumerator densitycal()
       {
         Time.timeScale = 0;
         getdensitycount1();
         densityperkm = (densitycount1 / 34.0);
         System.IO.File.AppendAllText("densityperkm.csv", densityperkm.ToString() + ",");

         averagespeed = (speedlist.Sum() / (densitycount1));

        flow = (densityperkm * averagespeed);
        System.IO.File.AppendAllText("flow.csv", flow.ToString() + ",");
              
        resetdensitycount1();
        speedlist.Clear();
        yield return null;
      }


	public IEnumerator rewards()
	{
		
		
		
getrewcount ();

GameObject[] waitcars1 = (GameObject.FindGameObjectsWithTag("hap"));
foreach(GameObject obj in waitcars1)
{
if(!mylist.Contains(obj))
{
mylist.Add(obj);
}
}

GameObject[] waitcars2 = (GameObject.FindGameObjectsWithTag("car"));
foreach(GameObject obje in waitcars2)
{
if(!mylist.Contains(obje))
{
mylist.Add(obje);
}
}         



                  finalrew = (rewCount - mylist.Count);

System.IO.File.AppendAllText("truerewards.csv", finalrew.ToString() + ",");
System.IO.File.AppendAllText("throughput.csv", rewCount.ToString() + ",");  

		byte[] msg1 = Encoding.UTF8.GetBytes (""+finalrew);
		socket.Send(msg1);
		resetRew();
              
                mylist.Clear();
		yield return null;
	}




	public static int getrewcount()
	{
		return rewCount;
	}


	public static void incrementRew()
	{
		rewCount++;
	}

	public static void resetRew()
	{
		rewCount = 0;
	}


	public static int getdensitycount1()
	{
		return densitycount1;
	}


	public static void incdensitycount1()
	{
		densitycount1++;
	}

	public static void decdensitycount1()
	{
		densitycount1--;
	}

	public static void resetdensitycount1()
	{
		densitycount1 = 0;
	}
      

	void Update ()

 {

	} 
}





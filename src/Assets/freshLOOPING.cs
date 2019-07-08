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

      // public materialchangeeee m = null;
	//public materialchangeeee n = null;

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
            // shot_count = 0;
		//print ("scene2 Loaded");

		//yes = false;

		trafficlight1 = GameObject.Find("TrafficLight1");
		m = trafficlight1.GetComponent<TLaction1> ();

		trafficlight2 = GameObject.Find("TrafficLight2");
		n = trafficlight2.GetComponent<TLaction2> ();


		/*if (trafficlight1 != null) {
			print ("traffic light1 found");
		}


		if (trafficlight2 != null) {
			print ("traffic light2 found");
		}   */

		socket.Connect ("localhost", port);
	//	print ("tcpSocket unity connected to python");


		StartCoroutine (looping ());
		

		//SceneManager.LoadScene("Scene1");
	
	}


	public IEnumerator looping()
	{
		
	     //print("starting looping");

		//if(yes == false)
		
	while (true) 
		//for(int i = 0; i < 200000; i++)
             //   for(int i = 0; i < 5; i++)
		{
			yield return StartCoroutine (zero());
			yield return StartCoroutine (takeshot());
			yield return StartCoroutine (soc());
			yield return StartCoroutine (action());
			yield return StartCoroutine (another());
                        yield return StartCoroutine (densitycal());
			yield return StartCoroutine (rewards());
                          //yield return StartCoroutine(red());
                            //   yield return StartCoroutine(wait1());
                            //   yield return StartCoroutine(green());
                             //  yield return StartCoroutine(wait2());
		

		//	yes = true;
	}
		//yes = true;
	}

	 /*  
         public IEnumerator red()
        {
           m.materialchangeGREEN1 ();
           n.materialchangeRED2();
           yield return null;
        }
         
         public IEnumerator wait1()
        {
      print(Time.time);
           yield return new WaitForSeconds(20);
     print(Time.time);
        }
        
          public IEnumerator green()
        {
           n.materialchangeGREEN2 ();
           m.materialchangeRED1();
           yield return null;
        }
        public IEnumerator wait2()
        {
           yield return new WaitForSeconds(20);
        }   

*/



   public IEnumerator zero()
	{
		//print("inside zero");
		if (waiting == false) 
		{
			//m.defaultmaterial();
			m.materialchangeRED1 ();
		//	print("TL1 default light set");
			//n.defaultmaterial();
			n.materialchangeRED2 ();
			//print("TL1 default light set");
			yield return new WaitForSeconds(20);
			Time.timeScale = 0;
			waiting = true;
		}
	}


	public IEnumerator takeshot()
	{
		//print("inside takeshot");
		shot_count += 1; 
		//ScreenCapture.CaptureScreenshot("images/shot"+ shot_count + ".png");	
		
ScreenCapture.CaptureScreenshot("/media/gargd/Fast/MFDimg/shot"+ shot_count + ".png");	
//print("shot taken");		
//print("shot"+ shot_count + ".png");

		yield return null;
	}

	public IEnumerator soc()
	{
		//print("inside soc");
		byte[] msg = Encoding.UTF8.GetBytes ("shot"+ shot_count + ".png");
		socket.Send(msg);
              //  print("shot"+ shot_count + ".png" + "sent");
		//globalsoc.socket.Send (msg);
		//print("image number sent:" + "shot"+ shot_count + ".png");
		//socket.Receive(bytes);
		yield return null;

	}

	public IEnumerator action()
	{
		//print("inside action");
		//globalsoc.socket.Receive(bytes);
		socket.Receive (bytes);
		//print("action:" + Encoding.UTF8.GetString(bytes));

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 0) {
			//m.materialchangeAMBER ();
		    //print("amber1 set");
			//m.amberwait();
			//yield return new WaitForSeconds (2);


			//StartCoroutine(amberwaiting1());


			//print("about to set green1");
n.materialchangeRED2();			
Time.timeScale = 1;
yield return new WaitForSeconds (6);
m.materialchangeGREEN1 ();
			//print("green1set");
			


		//	n.materialchangeRED2();
			//print("red2 set");
		} 

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 1) {

m.materialchangeRED1();
Time.timeScale = 1;			
yield return new WaitForSeconds (6);
n.materialchangeGREEN2 ();

}
yield return null;
}
			//n.materialchangeAMBER ();
		//	n.amberwait2 ();
			////yield return new WaitForSeconds (2);
			//print("amber2 set");
			//yield return StartCoroutine(amberwaiting());
			//yield return new WaitForSeconds (2);


			//print("about to set green2");
		//	n.materialchangeGREEN2 ();
			//print ("green2 set");

		//	m.materialchangeRED1();
		
			//print("red1 set");   */

			//StartCoroutine(amberwaiting2());

		//} 


/*else {
			m.defaultmaterial ();
			n.defaultmaterial ();
		
		//}   */
	
		//Time.timeScale = 1;
		//yield return new WaitForSeconds(20);
		//yield return null;
	//}

	public IEnumerator another()
	{
		//print("inside another");
	//	Time.timeScale = 1;
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


             //    finalrew = (mylist.Count - rewCount);
                  finalrew = (rewCount - mylist.Count);
                 //print("finalrew:" + finalrew);

		//System.IO.File.AppendAllText("REWARDS.csv", rewCount.ToString() + ",");  // save the rewards in a csv file

//System.IO.File.AppendAllText("moreREWARDS.csv", rewCount.ToString() + ",");  // save the more rewards in a csv file to know if PG has converged or not

//System.IO.File.AppendAllText("REWARDSAGAIN.csv", rewCount.ToString() + ",");
//System.IO.File.AppendAllText("REWARDSAGAINAGAIN.csv", rewCount.ToString() + ",");

System.IO.File.AppendAllText("truerewards.csv", finalrew.ToString() + ",");
System.IO.File.AppendAllText("throughput.csv", rewCount.ToString() + ",");  

		byte[] msg1 = Encoding.UTF8.GetBytes (""+finalrew);
		socket.Send(msg1);
		//globalsoc.socket.Send(msg1);
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
		/*if (yes == true) {
			CarCounter.resetCarCount ();        //car counter = 0
			newCarCount.resetCarCount ();
			SceneManager.LoadScene ("Scene3");
			//print ("DONEEEE");

		}*/

	} 
}





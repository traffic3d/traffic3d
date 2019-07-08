using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class nightScene2LOOP : MonoBehaviour {


	//byte[] bytes = new byte[256];
	//public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	public GameObject trafficlight1;
	public GameObject trafficlight2;

	public TLaction1 m = null;
	public TLaction2 n = null;

	public bool waiting = false;

	//public static int shot_count;
	public static int rewCount = 0;

	public bool yes = false;


	void Start () {


		trafficlight1 = GameObject.Find("TrafficLight1");
		m = trafficlight1.GetComponent<TLaction1> ();

		trafficlight2 = GameObject.Find("TrafficLight2");
		n = trafficlight2.GetComponent<TLaction2> ();

		StartCoroutine (loopsing ());
	}


	public IEnumerator loopsing()
	{

		//print("starting looping2");

		if(yes == false)
		{
			//while (true) 
			for(int i = 0; i < 50000; i++)
			{
				//yield return StartCoroutine (zero());
				//yield return StartCoroutine (takeshot());
				//yield return StartCoroutine (soc());
				//yield return StartCoroutine (action());
				//yield return StartCoroutine (another());
				//yield return StartCoroutine (rewards());
                            yield return StartCoroutine(red());
                               yield return StartCoroutine(wait1());
                               yield return StartCoroutine(green());
                               yield return StartCoroutine(wait2());
                                
			}

			yes = true;
		}
		//yes = true;
	}

          public IEnumerator red()
        {
           m.materialchangeGREEN1 ();
           n.materialchangeRED2();
           yield return null;
        }
         
         public IEnumerator wait1()
        {
           yield return new WaitForSeconds(20);
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

	/*public IEnumerator zero()
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
	}*/

	//public IEnumerator takeshot()
	//{
		//print("inside takeshot");
		//freshLOOPING.shot_count += 1;
		//shot_count += 1; 
		//ScreenCapture.CaptureScreenshot("images/shot"+ shot_count + ".png");	
		//ScreenCapture.CaptureScreenshot("/media/gargd/Data/trafficVARY2/unityCommonProject/JunctionImages/shot"+ freshLOOPING.shot_count + ".png");	
		//print("shot"+ freshLOOPING.shot_count + ".png");

		//yield return null;
	//}

	//public IEnumerator soc()
	//{
		//print("inside soc");
		//byte[] msg = Encoding.UTF8.GetBytes ("shot"+ freshLOOPING.shot_count + ".png");
		//socket.Send(msg);
		//globalsoc.socket.Send (msg);
		//print("image number sent:" + "shot"+ freshLOOPING.shot_count + ".png");
		//socket.Receive(bytes);
		//yield return null;

	//}

	//public IEnumerator action()
	//{
		//print("inside action");
	//	globalsoc.socket.Receive(bytes);
		//socket.Receive (bytes);
		//print("action:" + Encoding.UTF8.GetString(bytes));

		//if (int.Parse (Encoding.UTF8.GetString (bytes)) == 0) {
			//m.materialchangeAMBER ();
			//print("amber1 set");
			//m.amberwait();
			//yield return new WaitForSeconds (2);


			//StartCoroutine(amberwaiting1());


			//print("about to set green1");
			//m.materialchangeGREEN1 ();
			//print("green1set");



			//n.materialchangeRED2();
			//print("red2 set");
		//} 

		//if (int.Parse (Encoding.UTF8.GetString (bytes)) == 1) {
			//n.materialchangeAMBER ();
			//	n.amberwait2 ();
			////yield return new WaitForSeconds (2);
			//print("amber2 set");
			//yield return StartCoroutine(amberwaiting());
			//yield return new WaitForSeconds (2);


			//print("about to set green2");
		//	n.materialchangeGREEN2 ();
			//print ("green2 set");

			//m.materialchangeRED1();

			//print("red1 set");   */

			//StartCoroutine(amberwaiting2());

		//} 
                   /*else {
			m.defaultmaterial ();
			n.defaultmaterial ();
		
		}   */

		//Time.timeScale = 1;
		//yield return new WaitForSeconds(20);
		//yield return null;
	//}

	/*public IEnumerator another()
	{
		//print("inside another");
		Time.timeScale = 1;
		yield return new WaitForSeconds(20);  

	}*/


	//public IEnumerator rewards()
	//{
		//print ("inside rewards");
	//	Time.timeScale = 0;
		//m.defaultmaterial();
		//n.defaultmaterial();

	//	freshLOOPING.getrewcount ();

	//	System.IO.File.AppendAllText("rewards.csv", freshLOOPING.rewCount.ToString() + ",");  // save the rewards in a csv file
		//byte[] msg1 = Encoding.UTF8.GetBytes (""+freshLOOPING.rewCount);
		//socket.Send(msg1);
		//globalsoc.socket.Send(msg1);
		//freshLOOPING.resetRew();
		//yield return null;
	//}




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





	void Update () {
		if (yes == true) {
			//carCounterScene3.resetCarCount ();
			//CARcOUNTER2sCEne3.resetCarCount ();


			CarCounter.resetCarCount ();
			newCarCount.resetCarCount ();
			//SceneManager.LoadScene ("nightscene 1");
			print("done with FINAL TRAINING");
                        Time.timeScale = 0;
                        print("unity paused after final training");
                        
		}

	} 
}

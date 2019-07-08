using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Net;
//using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using UnityEngine.SceneManagement;



public class scene3Looping : MonoBehaviour {


	//private const int port = 14069;
	//byte[] bytes = new byte[256];
	//public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	public GameObject trafficlight1;
	public GameObject trafficlight2;

	public TLaction1 m = null;
	public TLaction2 n = null;

	public bool waiting = false;

	//private int shot_count = 2;
	public static int rewCount = 0;

	public bool yes;


	void Start () {

		print("scene3 loaded");

		yes = false;

		trafficlight1 = GameObject.Find("TrafficLight1");
		m = trafficlight1.GetComponent<TLaction1> ();

		trafficlight2 = GameObject.Find("TrafficLight2");
		n = trafficlight2.GetComponent<TLaction2> ();


		//	socket.Connect ("localhost", port);
		//	print ("tcpSocket unity connected to python");


		//StartCoroutine (looping ());


		//SceneManager.LoadScene("Scene1");

	}


	//public IEnumerator looping()
	//{

		//print("inside looping");

		//if(yes == false)
		//{
			//while (true) 
			//for(int i = 1; i < 3; i++)
			//{
				//yield return StartCoroutine (zero());
				//yield return StartCoroutine (takeshot());
				//yield return StartCoroutine (soc());
				//yield return StartCoroutine (action());
				//yield return StartCoroutine (another());
				//yield return StartCoroutine (rewards());
			//}
		//}
		//yes = true;
	//}
	
     /*public IEnumerator zero()
	{
		print("inside zero");
		if (waiting == false) 
		{
			//m.defaultmaterial();
			m.materialchangeRED1 ();
			print("TL1 default light set");
			//n.defaultmaterial();
			n.materialchangeRED2 ();
			print("TL1 default light set");
			yield return new WaitForSeconds(20);
			Time.timeScale = 0;
			waiting = true;
		}
	}  */


	/*public IEnumerator takeshot()
	{
		print("inside takeshot");
		//shot_count +=1;
		freshLOOPING.shot_count += 1;
		//ScreenCapture.CaptureScreenshot("images/shot"+ freshLOOPING.shot_count + ".png");

		ScreenCapture.CaptureScreenshot("images/shot"+ freshLOOPING.shot_count + ".png");
		print("shot"+ freshLOOPING.shot_count + ".png");
		yield return null;

	}

	public IEnumerator soc()
	{
		print("inside soc");
		byte[] msg = Encoding.UTF8.GetBytes ("shot"+ freshLOOPING.shot_count + ".png");
		//socket.Send(msg);
		globalsoc.socket.Send (msg);

		//socket.Receive(bytes);
		yield return null;

	}    */

	//public IEnumerator action()
	//{
	//	print("inside action");
	//	globalsoc.socket.Receive(bytes);
		//socket.Receive (bytes);
		//print(Encoding.UTF8.GetString(bytes));

		//if (int.Parse (Encoding.UTF8.GetString (bytes)) == 0) {
			//m.materialchangeAMBER ();
			//print("amber1 set");
			//m.amberwait();
			//yield return new WaitForSeconds (2);


			//StartCoroutine(amberwaiting1());


			//print("about to set green1");
		//	m.materialchangeGREEN1 ();
			//print("green1set");



		//	n.materialchangeRED2();
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
			//n.materialchangeGREEN2 ();
			//print ("green2 set");

			//m.materialchangeRED1();

			//print("red1 set");   */

			//StartCoroutine(amberwaiting2());

		//} /*else {
			//m.defaultmaterial ();
		//	n.defaultmaterial ();
		
		//}   */

		//Time.timeScale = 1;
		//yield return new WaitForSeconds(20);
		//yield return null;
	//}

	/*public IEnumerator another()
	{
		print("inside another");
		Time.timeScale = 1;
		yield return new WaitForSeconds(20);  

	} */


	/*public IEnumerator rewards()
	{
		print ("inside rewards");
		Time.timeScale = 0;
		//m.defaultmaterial();
		//n.defaultmaterial();

		//getrewcountScene3 ();
		System.IO.File.AppendAllText("rewards.csv", rewCount.ToString() + ",");  // save the rewards in a csv file
		byte[] msg1 = Encoding.UTF8.GetBytes (""+rewCount);
		//socket.Send(msg1);
		globalsoc.socket.Send(msg1);
		resetRew();
		yield return null;
	}   


	public IEnumerator amberwaiting1()
	{
		m.materialchangeAMBER ();
		print("amber1 set");

		m.amberwait();

		 yield return new WaitForSeconds(2);
		print ("amber1 waiting done");

		print("about to set green1");
		m.materialchangeGREEN1 ();
		print("green1set");



		n.materialchangeRED2();
		print("red2 set");

		yield return null;
		//yield return new WaitForSeconds(2);
	}

	public IEnumerator amberwaiting2()
	{
		n.materialchangeAMBER ();
		n.amberwait2 ();

		//yield return new WaitForSeconds (2);
		/*print("amber2 set");
		//yield return StartCoroutine(amberwaiting());
		yield return new WaitForSeconds (2);
		print ("amber2 waiting done");

		print("about to set green2");
		n.materialchangeGREEN2 ();
		print ("green2 set"); 

		m.materialchangeRED1();

		print("red1 set");
		yield return null;
	} 
*/

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
			SceneManager.LoadSceneAsync("testscene");
		}

	}
}

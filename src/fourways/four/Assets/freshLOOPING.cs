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


public class freshLOOPING : MonoBehaviour {

	private const int port = 16004;  
	byte[] bytes = new byte[256];
	public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	public GameObject trafficlight1;
	public GameObject trafficlight2;
	public GameObject trafficlight3;
	public GameObject trafficlight4;
	//public GameObject trafficlight11;

	public TLaction1 m = null;
	public TLaction2 n = null;
	public TLaction3 s = null;
	public TLaction4 r = null;

	public bool waiting = false;

	private int shot_count;
	public static int rewCount = 0;


	void Start () {
		
		print("start LOOPING");
		trafficlight1 = GameObject.Find("TrafficLight1");
		m = trafficlight1.GetComponent<TLaction1> ();


		trafficlight2 = GameObject.Find("TrafficLight2");
		n = trafficlight2.GetComponent<TLaction2> ();


		trafficlight3 = GameObject.Find("TrafficLight11");
		s = trafficlight3.GetComponent<TLaction1>();
		//if (trafficlight3 != null) {
		//	print("trafficlight3 found");
		//}



		trafficlight4 = GameObject.Find("TrafficLight4");
		r = trafficlight4.GetComponent<TLaction4>();
	

		socket.Connect("localhost", port);                                                 //(IPAddress.Parse("172.16.114.1"), port); for connecting to vmware
		print ("tcpSocket unity connected to python");

		StartCoroutine(looping ());
	                  
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
			yield return StartCoroutine (rewards());
		}
	}

	public IEnumerator zero()
	{
		if (waiting == false) 
		{
			//m.defaultmaterial();
			m.materialchangeRED1 ();
			//print("TL1 default light set");
			//n.defaultmaterial();
			n.materialchangeRED2 ();
		//	print("TL1 default light set");
			s.materialchangeRED1();
			n.
			//s.materialchangeRED1 ();
			r.materialchangeRED4 ();

			yield return new WaitForSeconds(20);
			Time.timeScale = 0;
			print("paused");
			waiting = true;
		}
	}

	public IEnumerator takeshot()
	{
		shot_count +=1; 
		//try{                     /media/gargd/Data
		ScreenCapture.CaptureScreenshot("/media/gargd/Data/crossjunctionshots/shot"+ shot_count + ".png");
			//print("NO ERROR SO FAR");
		//	print(shot_count);
		//}
		//catch (IOException e)
	//	{
		//	print ("ERROR WITH PICTURE");
	//	}
		//if((ScreenCapture.CaptureScreenshot ("Z:/sharedImages/UNITYshots/shot" + shot_count + ".png")) == true) {
		//	Time.timeScale = 0;
		//	print ("unity PAUSED due to screenshot error");


		yield return null;
	}

	public IEnumerator soc()
	{
		byte[] msg = Encoding.UTF8.GetBytes ("shot" + shot_count + ".png");
		socket.Send(msg);
		print ("image number sent");
		//socket.Receive(bytes);
		yield return null;

	}

	public IEnumerator action()
	{
		socket.Receive (bytes);
		print(Encoding.UTF8.GetString(bytes));

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 0) {
			//m.materialchangeAMBER ();
		    //print("amber1 set");
			//m.amberwait();
			//yield return new WaitForSeconds (2);


			//StartCoroutine(amberwaiting1());


			//print("about to set green1");
			m.materialchangeGREEN1 ();
			//print("green1set");
			


			n.materialchangeRED2();
			s.materialchangeRED3 ();
			r.materialchangeRED4 ();
			//print("red2 set");
		} 

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 1) {
			//n.materialchangeAMBER ();
		//	n.amberwait2 ();
			////yield return new WaitForSeconds (2);
			//print("amber2 set");
			//yield return StartCoroutine(amberwaiting());
			//yield return new WaitForSeconds (2);


			//print("about to set green2");
			n.materialchangeGREEN2 ();
			//print ("green2 set");

			m.materialchangeRED1();
			s.materialchangeRED3 ();
			r.materialchangeRED4 ();
			//print("red1 set");   */

			//StartCoroutine(amberwaiting2());

		} /*else {
			m.defaultmaterial ();
			n.defaultmaterial ();
		
		}   */
	
		//Time.timeScale = 1;
		//yield return new WaitForSeconds(20);

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 2) {
			//n.materialchangeAMBER ();
			//	n.amberwait2 ();
			////yield return new WaitForSeconds (2);
			//print("amber2 set");
			//yield return StartCoroutine(amberwaiting());
			//yield return new WaitForSeconds (2);


			//print("about to set green2");
			//n.materialchangeGREEN2 ();
			s.materialchangeGREEN3 ();
			n.materialchangeRED2();
            m.materialchangeRED1();
			r.materialchangeRED4 ();

			//print ("green2 set");

		//	m.materialchangeRED1();
			} 


		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 3) {
			//n.materialchangeAMBER ();
			//	n.amberwait2 ();
			////yield return new WaitForSeconds (2);
			//print("amber2 set");
			//yield return StartCoroutine(amberwaiting());
			//yield return new WaitForSeconds (2);


			//print("about to set green2");
			//n.materialchangeGREEN2 ();
			//s.materialchangeGREEN3 ();
			r.materialchangeGREEN4 ();
			n.materialchangeRED2();
			m.materialchangeRED1();
			s.materialchangeRED3 ();

			//print ("green2 set");

			//	m.materialchangeRED1();
		} 



		yield return null;
	}

	public IEnumerator another()
	{
		Time.timeScale = 1;
		yield return new WaitForSeconds(20);  
	
	}


	public IEnumerator rewards()
	{
		Time.timeScale = 0;
		//m.defaultmaterial();
		//n.defaultmaterial();

		getrewcount ();
		System.IO.File.AppendAllText ("rewardsfourway.csv", rewCount.ToString () + ",");   // + System.Environment.NewLine);  // save the rewards in a csv file
		byte[] msg1 = Encoding.UTF8.GetBytes (""+rewCount);
		socket.Send(msg1);

		resetRew();
		yield return null;
	}


	public IEnumerator amberwaiting1()
	{
		m.materialchangeAMBER ();
		print("amber1 set");

		m.amberwait();

		/* yield return new WaitForSeconds(2);
		print ("amber1 waiting done");

		print("about to set green1");
		m.materialchangeGREEN1 ();
		print("green1set"); */

  

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
		print ("green2 set"); */

		m.materialchangeRED1();

		print("red1 set");
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




	void Update () {
		
	}
}

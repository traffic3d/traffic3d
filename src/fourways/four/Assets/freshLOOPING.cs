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



		trafficlight4 = GameObject.Find("TrafficLight4");
		r = trafficlight4.GetComponent<TLaction4>();
	

		socket.Connect("localhost", port);
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
			m.materialchangeRED1 ();
			n.materialchangeRED2 ();
			s.materialchangeRED1();
			n.
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
		ScreenCapture.CaptureScreenshot("/media/gargd/Data/crossjunctionshots/shot"+ shot_count + ".png");


		yield return null;
	}

	public IEnumerator soc()
	{
		byte[] msg = Encoding.UTF8.GetBytes ("shot" + shot_count + ".png");
		socket.Send(msg);
		print ("image number sent");
		yield return null;

	}

	public IEnumerator action()
	{
		socket.Receive (bytes);
		print(Encoding.UTF8.GetString(bytes));

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 0) {

			m.materialchangeGREEN1 ();
			


			n.materialchangeRED2();
			s.materialchangeRED3 ();
			r.materialchangeRED4 ();
		} 

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 1) {

			n.materialchangeGREEN2 ();

			m.materialchangeRED1();
			s.materialchangeRED3 ();
			r.materialchangeRED4 ();

		}

		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 2) {

			s.materialchangeGREEN3 ();
			n.materialchangeRED2();
            m.materialchangeRED1();
			r.materialchangeRED4 ();

			} 


		if (int.Parse (Encoding.UTF8.GetString (bytes)) == 3) {

			r.materialchangeGREEN4 ();
			n.materialchangeRED2();
			m.materialchangeRED1();
			s.materialchangeRED3 ();

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

		getrewcount ();
		System.IO.File.AppendAllText ("rewardsfourway.csv", rewCount.ToString () + ","); // save the rewards in a csv file
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

		n.materialchangeRED2();
		print("red2 set");

		yield return null;
	}

	public IEnumerator amberwaiting2()
	{
		n.materialchangeAMBER ();
		n.amberwait2 ();

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

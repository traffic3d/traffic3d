using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;

public class redlight : MonoBehaviour {

	public GameObject trafficlight1;
	public GameObject trafficlight11;
	public GameObject trafficlight2;
	public GameObject trafficlight21;
	public GameObject trafficlight3;
	public GameObject trafficlight31;
	public GameObject trafficlight4;
	public GameObject trafficlight41;

	public TLaction1 m = null;
	public TLaction11 m1 = null;
	public TLaction2 n = null;
	public TLaction21 n1 = null;
	public TLaction3 u = null;
	public TLaction31 u1 = null;
	public TLaction4 v = null;
	public TLaction41 v1 = null;




	void Start () {

		print("start LOOPING");
		trafficlight1 = GameObject.Find("SphereTL1");   //TrafficLight1
		m = trafficlight1.GetComponent<TLaction1> ();

		trafficlight11 = GameObject.Find("SphereTL11");   //TrafficLight1
		m1 = trafficlight11.GetComponent<TLaction11> ();
		//trafficlight11 = GameObject.Find("SphereTL11");   //TrafficLight1
		//m = trafficlight1.GetComponent<TLaction1> ();

		trafficlight2 = GameObject.Find("SphereTL2");  //TrafficLight2
		n = trafficlight2.GetComponent<TLaction2> ();

		trafficlight21 = GameObject.Find("SphereTL21");  //TrafficLight2
		n1 = trafficlight21.GetComponent<TLaction21> ();

		trafficlight3 = GameObject.Find("SphereTL3");
		u = trafficlight3.GetComponent<TLaction3>();

		trafficlight31 = GameObject.Find("SphereTL31");
		u1 = trafficlight31.GetComponent<TLaction31>();

		trafficlight4 = GameObject.Find("SphereTL4");
		v = trafficlight4.GetComponent<TLaction4>();
		trafficlight41 = GameObject.Find("SphereTL41");
		v1 = trafficlight41.GetComponent<TLaction41>();



		StartCoroutine(looping ());

	}
	

	public IEnumerator looping()
	{
		while (true) {
			yield return StartCoroutine (one());
			yield return StartCoroutine (two());
			yield return StartCoroutine (third());
			yield return StartCoroutine (four());
			yield return StartCoroutine (five());
		}
	
	
	}

	public IEnumerator one()
	{
		m.materialchangeRED1 ();
		m1.materialchangeblack ();
		v.materialchangeRED4 ();
		v1.materialchangeblack ();
		n.materialchangeRED2 ();
		n1.materialchangeblack ();

		u.materialchangeRED3 ();
		u1.materialchangeblack ();
//		v.materialchangeRED4 ();
		yield return new WaitForSeconds(200);
	

	}

	public IEnumerator two()
	{
		//n.materialchangeRED2();
		u.materialchangeRED3();
		u1.materialchangeblack ();
		//v.materialchangeRED4();
		m.materialchangeblack();
		m1.materialchangeGREEN1 ();
		v.materialchangeRED4 ();
		v1.materialchangeblack ();

		n.materialchangeRED2 ();
		n1.materialchangeblack ();
		//Time.timeScale = 1;
		yield return new WaitForSeconds(19);


		//yield return null;

	}

	public IEnumerator third()
	{
		v.materialchangeRED4 ();
		v1.materialchangeblack ();
		m.materialchangeRED1();
		m1.materialchangeblack ();
		//u.materialchangeRED3 ();
		u.materialchangeblack();
		u1.materialchangeGREEN3 ();

		n.materialchangeRED2 ();
		n1.materialchangeblack ();
		//v.materialchangeRED4 ();
		//n.materialchangeGREEN2 ();
	
		yield return new WaitForSeconds (19);

	}

	public IEnumerator four()
	{
		m.materialchangeRED1();
		m1.materialchangeblack();
		v.materialchangeRED4();
		v1.materialchangeblack ();
		//n.materialchangeRED2();
		//v.materialchangeRED4();
		u.materialchangeRED3();
		u1.materialchangeblack ();

		n.materialchangeblack ();
		n1.materialchangeGREEN2 ();

		yield return new WaitForSeconds(19);


	}


	public IEnumerator five()
	{
		v1.materialchangeGREEN4 ();
		v.materialchangeblack ();
		m.materialchangeRED1();
		m1.materialchangeblack ();
		u.materialchangeRED3();
		u1.materialchangeblack ();
		//n.materialchangeRED2();
	//	u.materialchangeRED3();
		n.materialchangeRED2 ();
		n1.materialchangeblack ();
		//v.materialchangeGREEN4();
		yield return new WaitForSeconds(19);

	
	}

	// Update is called once per frame
	void Update () {
		
	}
}

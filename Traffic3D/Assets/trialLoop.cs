using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trialLoop : MonoBehaviour {
	public GameObject trafficlight1;
	public GameObject trafficlight11;
	// Use this for initialization

	public TLaction1 m = null;
	public TLaction11 m1 = null;
	void Start () {
		trafficlight1 = GameObject.Find("Sphere");   //TrafficLight1
		m = trafficlight1.GetComponent<TLaction1> ();

		trafficlight11 = GameObject.Find("Sphere1");   //TrafficLight1
		m1 = trafficlight11.GetComponent<TLaction11> ();
		StartCoroutine(looping ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public IEnumerator looping()
	{
		while (true) {
			yield return StartCoroutine (one());
			yield return StartCoroutine (two());
			//yield return StartCoroutine (third());
			//yield return StartCoroutine (four());
			//yield return StartCoroutine (five());
		}


	}
	public IEnumerator one()
	{
		m.materialchangeRED1 ();
		m1.materialchangeblack ();
		//n.materialchangeRED2 ();

	//	u.materialchangeRED3 ();

		//v.materialchangeRED4 ();
		yield return new WaitForSeconds(5);


	}

	public IEnumerator two()
	{
	//	m.materialchangeRED1 ();
	//	m1.materialchangeblack ();
		m1.materialchangeGREEN1 ();
		m.materialchangeblack ();
	//	n.materialchangeRED2 ();

	//	u.materialchangeRED3 ();

	//	v.materialchangeRED4 ();
		yield return new WaitForSeconds(5);


	}
}


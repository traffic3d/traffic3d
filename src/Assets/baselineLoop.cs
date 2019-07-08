using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baselineLoop : MonoBehaviour {

	public GameObject trafficlight1;
	public GameObject trafficlight2;

	public TLaction1 m = null;
	public TLaction2 n = null;



	void Start () {
trafficlight1 = GameObject.Find("TrafficLight1");
		m = trafficlight1.GetComponent<TLaction1> ();

		trafficlight2 = GameObject.Find("TrafficLight2");
		n = trafficlight2.GetComponent<TLaction2> ();
	
             StartCoroutine (looping ());
	}
	

public IEnumerator looping()
{

while(true)
{
                        yield return StartCoroutine (one());
                        yield return StartCoroutine (wait1());
                      //  yield return StartCoroutine (amber1());
			yield return StartCoroutine (two());
			yield return StartCoroutine (wait2());
                       // yield return StartCoroutine (amber2());
}
}
 public IEnumerator one()
{
n.materialchangeRED2();	
yield return new WaitForSeconds (6);
m.materialchangeGREEN1 ();
 yield return null;
}

public IEnumerator wait1()
{
yield return new WaitForSeconds(5);
}

//public IEnumerator amber1()
//{
//yield return new WaitForSeconds(6);
//}

public IEnumerator two()
{
m.materialchangeRED1();
yield return new WaitForSeconds (6);
n.materialchangeGREEN2 ();
 yield return null;
}

public IEnumerator wait2()
{
yield return new WaitForSeconds(5);
}
	
	void Update () {
		
	}
}

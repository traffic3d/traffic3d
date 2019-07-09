using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class NIGHTsceneLOOP : MonoBehaviour {


	byte[] bytes = new byte[256];

	public GameObject trafficlight1;
	public GameObject trafficlight2;

	public TLaction1 m = null;
	public TLaction2 n = null;
            
	public bool waiting = false;

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

		if(yes == false)
		{
			for(int i = 0; i < 200000; i++)
			{
                               yield return StartCoroutine(red());
                               yield return StartCoroutine(wait1());
                               yield return StartCoroutine(green());
                               yield return StartCoroutine(wait2());
			}

			yes = true;
		}
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


			CarCounter.resetCarCount ();
			newCarCount.resetCarCount ();
			SceneManager.LoadScene ("nightscene 1");

		}

	} 
}


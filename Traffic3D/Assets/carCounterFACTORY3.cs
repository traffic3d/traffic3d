using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carCounterFACTORY3 : MonoBehaviour {

	public static int carCount;

	public static int maxCarNumbers = 5;

	//public static int maxcars = Random.Range(2,4);



	void Start () {
		//print ("carcounterFAC3 started");
		carCount = 0;
	}
	

	void Update () {
		
	}


	public static int getCarCount()
	{

		return carCount;

	}

	/*  public static int maxCarNumbers()
    {
        Random.Range(1, 8);
    }  */


	public static void incrementCarCount()
	{

		carCount++;
		//print ("infac3:" + carCount);

	}


	public static void decrementCarCount()
	{

		carCount--;

	}
}

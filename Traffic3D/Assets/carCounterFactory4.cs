using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carCounterFactory4 : MonoBehaviour {

	public static int carCount;

	public static int maxCarNumbers = 8;

	void Start () {
	//	print ("carcounterFAC4 started");
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
	//	print ("INfac4" + carCount);

	}

	public static void decrementCarCount()
	{

		carCount--;

	}

}

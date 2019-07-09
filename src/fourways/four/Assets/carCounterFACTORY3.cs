using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carCounterFACTORY3 : MonoBehaviour {

	private static int carCount = 0;

	public static int maxCarNumbers = 8;

	void Start () {
		
	}
	

	void Update () {
		
	}

	public static int getCarCount()
	{

		return carCount;

	}

	public static void incrementCarCount()
	{

		carCount++;

	}

	public static void decrementCarCount()
	{

		carCount--;

	}
}

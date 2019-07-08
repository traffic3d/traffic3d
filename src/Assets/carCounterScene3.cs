using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carCounterScene3 : MonoBehaviour {

	private static int carCount = 0;

	public static int maxCarNumbers = 10;

	// Use this for initialization
	void Start () {
		// int maxCarNumbers = Random.Range(8, 10);
	}

	// Update is called once per frame
	void Update () {

	}

	public static int getCarCount() {

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

	public static void resetCarCount()
	{

		carCount = 0;

	}



}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NScounter2 : MonoBehaviour {

	private static int carCount = 0;

	public static int maxCarNumbers = 8;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

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
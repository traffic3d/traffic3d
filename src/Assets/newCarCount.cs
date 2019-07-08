using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newCarCount : MonoBehaviour {

    public static int carCount = 0;

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

  /*  public static int maxCarNumbers()
    {
        Random.Range(1, 8);
    }  */

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

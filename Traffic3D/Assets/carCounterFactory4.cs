using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carCounterFactory4 : MonoBehaviour
{

    public static int carCount;

    public static int maxCarNumbers = 8;

    void Start()
    {
        carCount = 0;
    }




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

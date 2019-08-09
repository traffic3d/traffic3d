using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactoryCounter4 : MonoBehaviour
{

    public static int carCount = 0;

    public static int maxCarCount = 8;

    public static int GetCarCount()
    {
        return carCount;
    }

    public static void IncrementCarCount()
    {
        carCount++;
    }

    public static void DecrementCarCount()
    {
        carCount--;
    }

}

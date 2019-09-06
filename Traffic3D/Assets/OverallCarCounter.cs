using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallCarCounter : MonoBehaviour
{
    public static int overallCarCount = 0;

    public static int GetOverallCarCount()
    {
        return overallCarCount;
    }

    public static void IncrementOverallCarCount()
    {
        overallCarCount++;
    }
}

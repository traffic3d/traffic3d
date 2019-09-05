using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyCarCounter : MonoBehaviour
{

    public static int journeyCarCount = 0;


    void Start()
    {

    }


    void Update()
    {

    }

    public static int GetJourneyCarCount()
    {
        return journeyCarCount;

    }

    public static void IncrementJourneyCarCount()
    {
        journeyCarCount++;
    }

}

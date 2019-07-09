using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rewardcounter : MonoBehaviour
{

    public static int rewardcount = 0;

    void Start()
    {

    }


    void Update()
    {

    }

    public static int getrewardcount()
    {
        return rewardcount;
    }

    public static void incrementrewardcount()
    {
        rewardcount++;
    }
}

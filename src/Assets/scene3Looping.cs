using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using UnityEngine.SceneManagement;



public class scene3Looping : MonoBehaviour
{

    public GameObject trafficlight1;
    public GameObject trafficlight2;

    public TLaction1 m = null;
    public TLaction2 n = null;

    public bool waiting = false;

    public static int rewCount = 0;

    public bool yes;


    void Start()
    {

        print("scene3 loaded");

        yes = false;

        trafficlight1 = GameObject.Find("TrafficLight1");
        m = trafficlight1.GetComponent<TLaction1>();

        trafficlight2 = GameObject.Find("TrafficLight2");
        n = trafficlight2.GetComponent<TLaction2>();

    }

    public static int getrewcount()
    {
        return rewCount;
    }


    public static void incrementRew()
    {
        rewCount++;
    }

    public static void resetRew()
    {
        rewCount = 0;
    }




    void Update()
    {

        if (yes == true)
        {
            SceneManager.LoadSceneAsync("testscene");
        }

    }
}

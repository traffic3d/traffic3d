using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TrafficLightManagerWithAI : MonoBehaviour
{

    private const int port = 13000;
    byte[] bytes = new byte[256];
    public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public GameObject waitcars1;
    public GameObject waitcars2;
    public GameObject waitcars3;

    public TrafficLightRed1 trafficLightRed1 = null;
    public TrafficLightGreen1 trafficLightGreen1 = null;
    public TrafficLightRed2 trafficLightRed2 = null;
    public TrafficLightGreen2 trafficLightGreen2 = null;
    public TrafficLightRed3 trafficLightRed3 = null;
    public TrafficLightGreen3 trafficLightGreen3 = null;
    public TrafficLightRed4 trafficLightRed4 = null;
    public TrafficLightGreen4 trafficLightGreen4 = null;

    public bool waiting = false;

    public static int shotCount = 0;
    public static int rewCount = 0;

    public static int finalrew = 0;

    public bool yes = false;
    List<GameObject> mylist = new List<GameObject>();

    public static int densityCount1;
    public static double densityPerkm;
    public static double averageSpeed;
    public static double flow;

    public static List<double> speedlist = new List<double>();

    void Start()
    {

        trafficLightRed1 = GameObject.Find("SphereTL1").GetComponent<TrafficLightRed1>();

        trafficLightGreen1 = GameObject.Find("SphereTL11").GetComponent<TrafficLightGreen1>();

        trafficLightRed2 = GameObject.Find("SphereTL2").GetComponent<TrafficLightRed2>();

        trafficLightGreen2 = GameObject.Find("SphereTL21").GetComponent<TrafficLightGreen2>();

        trafficLightRed3 = GameObject.Find("SphereTL3").GetComponent<TrafficLightRed3>();

        trafficLightGreen3 = GameObject.Find("SphereTL31").GetComponent<TrafficLightGreen3>();

        trafficLightRed4 = GameObject.Find("SphereTL4").GetComponent<TrafficLightRed4>();

        trafficLightGreen4 = GameObject.Find("SphereTL41").GetComponent<TrafficLightGreen4>();

        socket.Connect("localhost", port);


        StartCoroutine(MainLoop());

    }


    public IEnumerator MainLoop()
    {

        while (true)
        {
            yield return StartCoroutine(Reset());
            yield return StartCoroutine(TakeScreenshot());
            yield return StartCoroutine(SendScreenshot());
            yield return StartCoroutine(GetAction());
            yield return StartCoroutine(WaitTenSeconds());
            yield return StartCoroutine(CalculateDensity());
            yield return StartCoroutine(SendRewards());


        }

    }

    public IEnumerator Reset()
    {
        if (waiting == false)
        {
            trafficLightRed1.SetToRedMaterial();

            trafficLightRed2.SetToRedMaterial();

            trafficLightRed3.SetToRedMaterial();

            trafficLightRed4.SetToRedMaterial();

            yield return new WaitForSeconds(20);
            Time.timeScale = 0;
            waiting = true;
        }
    }


    public IEnumerator TakeScreenshot()
    {
        shotCount += 1;

        string screenshotPath = Application.dataPath + "/Screenshots";
        if (!Directory.Exists(screenshotPath))
        {
            Directory.CreateDirectory(screenshotPath);
        }

        ScreenCapture.CaptureScreenshot(screenshotPath + "/shot" + shotCount + ".png");

        yield return null;
    }

    public IEnumerator SendScreenshot()
    {
        byte[] msg = Encoding.UTF8.GetBytes("shot" + shotCount + ".png");
        socket.Send(msg);
        yield return null;

    }

    public IEnumerator GetAction()
    {
        socket.Receive(bytes);

        if (int.Parse(Encoding.UTF8.GetString(bytes)) == 0)
        {
            trafficLightGreen2.SetToBlackMaterial();
            trafficLightRed2.SetToRedMaterial();
            trafficLightGreen3.SetToBlackMaterial();
            trafficLightRed3.SetToRedMaterial();
            trafficLightGreen4.SetToBlackMaterial();
            trafficLightRed4.SetToRedMaterial();
            Time.timeScale = 1;
            yield return new WaitForSeconds(6);
            trafficLightGreen1.SetToGreenMaterial();
            trafficLightRed1.SetToBlackMaterial();

        }

        if (int.Parse(Encoding.UTF8.GetString(bytes)) == 1)
        {
            trafficLightGreen1.SetToBlackMaterial();
            trafficLightRed1.SetToRedMaterial();
            trafficLightGreen3.SetToBlackMaterial();
            trafficLightRed3.SetToRedMaterial();
            trafficLightGreen4.SetToBlackMaterial();
            trafficLightRed4.SetToRedMaterial();
            Time.timeScale = 1;
            yield return new WaitForSeconds(6);
            trafficLightGreen2.SetToGreenMaterial();
            trafficLightRed2.SetToBlackMaterial();

        }

        if (int.Parse(Encoding.UTF8.GetString(bytes)) == 2)
        {
            trafficLightGreen1.SetToBlackMaterial();
            trafficLightRed1.SetToRedMaterial();
            trafficLightGreen2.SetToBlackMaterial();
            trafficLightRed2.SetToRedMaterial();
            trafficLightGreen4.SetToBlackMaterial();
            trafficLightRed4.SetToRedMaterial();

            Time.timeScale = 1;
            yield return new WaitForSeconds(5);
            trafficLightGreen3.SetToGreenMaterial();
            trafficLightRed3.SetToBlackMaterial();
            
        }


        if (int.Parse(Encoding.UTF8.GetString(bytes)) == 3)
        {
            trafficLightGreen1.SetToBlackMaterial();
            trafficLightRed1.SetToRedMaterial();
            trafficLightGreen2.SetToBlackMaterial();
            trafficLightRed2.SetToRedMaterial();
            trafficLightGreen3.SetToBlackMaterial();
            trafficLightRed3.SetToRedMaterial();

            Time.timeScale = 1;
            yield return new WaitForSeconds(5);
            trafficLightGreen2.SetToGreenMaterial();
            trafficLightRed4.SetToBlackMaterial();

        }

        yield return null;
    }

    public IEnumerator WaitTenSeconds()
    {
        yield return new WaitForSeconds(10);

    }

    public IEnumerator CalculateDensity()
    {
        Time.timeScale = 0;
        GetDensityCount1();
        densityPerkm = (densityCount1 / 34.0);
        System.IO.File.AppendAllText("densityperkm.csv", densityPerkm.ToString() + ",");

        averageSpeed = (speedlist.Sum() / (densityCount1));

        flow = (densityPerkm * averageSpeed);
        System.IO.File.AppendAllText("flow.csv", flow.ToString() + ",");

        ResetDensityCount1();
        speedlist.Clear();
        yield return null;
    }

    public IEnumerator SendRewards()
    {



        GetRewardCount();

        GameObject[] waitcars1 = (GameObject.FindGameObjectsWithTag("hap"));
        foreach (GameObject obj in waitcars1)
        {
            if (!mylist.Contains(obj))
            {
                mylist.Add(obj);
            }
        }

        GameObject[] waitcars2 = (GameObject.FindGameObjectsWithTag("car"));
        foreach (GameObject obje in waitcars2)
        {
            if (!mylist.Contains(obje))
            {
                mylist.Add(obje);
            }
        }



        finalrew = (rewCount - mylist.Count);

        System.IO.File.AppendAllText("truerewards.csv", finalrew.ToString() + ",");
        System.IO.File.AppendAllText("throughput.csv", rewCount.ToString() + ",");

        byte[] msg1 = Encoding.UTF8.GetBytes("" + finalrew);
        socket.Send(msg1);
        ResetRewardCount();

        mylist.Clear();
        yield return null;
    }

    public static int GetRewardCount()
    {
        return rewCount;
    }


    public static void IncrementRewardCount()
    {
        rewCount++;
    }

    public static void ResetRewardCount()
    {
        rewCount = 0;
    }


    public static int GetDensityCount1()
    {
        return densityCount1;
    }


    public static void IncrementDensityCount1()
    {
        densityCount1++;
    }

    public static void DecrementDensityCount1()
    {
        densityCount1--;
    }

    public static void ResetDensityCount1()
    {
        densityCount1 = 0;
    }

}

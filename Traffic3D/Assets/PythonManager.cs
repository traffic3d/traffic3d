using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class PythonManager : MonoBehaviour
{

    public static int shotCount = 0;
    public static int rewardCount = 0;

    public static int finalReward = 0;

    public static int densityCount1;
    public static double densityPerkm;
    public static double averageSpeed;
    public static double flow;

    public static List<double> speedlist = new List<double>();

    void Start()
    {
        if (SocketManager.GetInstance().Connect())
        {
            StartCoroutine(MainLoop());
        }
        else
        {
            TrafficLightManager.GetInstance().RunDemo();
        }
    }

    public IEnumerator MainLoop()
    {
        yield return StartCoroutine(Reset());
        while (true)
        {
            yield return StartCoroutine(TakeScreenshot());
            yield return StartCoroutine(SendScreenshot());
            yield return StartCoroutine(GetAction());
            yield return StartCoroutine(Wait20Seconds());
            yield return StartCoroutine(CalculateDensity());
            yield return StartCoroutine(SendRewards());
        }
    }

    public IEnumerator Reset()
    {
        yield return new WaitForSeconds(1);
        TrafficLightManager.GetInstance().SetAllToRed();
        yield return new WaitForSeconds(20);
        Time.timeScale = 0;
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
        SocketManager.GetInstance().Send(msg);
        yield return null;

    }

    public IEnumerator GetAction()
    {
        byte[] bytes = new byte[125];

        SocketManager.GetInstance().Receive(bytes);

        int trafficLightId = int.Parse(Encoding.UTF8.GetString(bytes)) + 1;

        TrafficLightManager.GetInstance().SetAllToRed();
        Time.timeScale = 1;
        yield return new WaitForSeconds(10);
        TrafficLightManager.GetInstance().SetTrafficLightToGreen(trafficLightId);

        yield return null;
    }

    public IEnumerator Wait20Seconds()
    {
        yield return new WaitForSeconds(20);

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

        List<GameObject> waitingCars = new List<GameObject>();

        GameObject[] waitcars1 = (GameObject.FindGameObjectsWithTag("hap"));
        foreach (GameObject obj in waitcars1)
        {
            if (!waitingCars.Contains(obj))
            {
                waitingCars.Add(obj);
            }
        }

        GameObject[] waitcars2 = (GameObject.FindGameObjectsWithTag("car"));
        foreach (GameObject obje in waitcars2)
        {
            if (!waitingCars.Contains(obje))
            {
                waitingCars.Add(obje);
            }
        }

        finalReward = (rewardCount - waitingCars.Count);

        System.IO.File.AppendAllText("truerewards.csv", finalReward.ToString() + ",");
        System.IO.File.AppendAllText("throughput.csv", rewardCount.ToString() + ",");

        byte[] msg1 = Encoding.UTF8.GetBytes("" + finalReward);
        SocketManager.GetInstance().Send(msg1);
        ResetRewardCount();

        waitingCars.Clear();
        yield return null;
    }

    public static int GetRewardCount()
    {
        return rewardCount;
    }

    public static void IncrementRewardCount()
    {
        rewardCount++;
    }

    public static void ResetRewardCount()
    {
        rewardCount = 0;
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

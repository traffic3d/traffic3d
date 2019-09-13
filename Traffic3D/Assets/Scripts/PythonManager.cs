using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PythonManager : MonoBehaviour
{
    public static PythonManager instance;

    public static PythonManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    public int shotCount = 0;
    public int rewardCount = 0;
    public int densityCount;
    public List<double> speedList = new List<double>();

    void Start()
    {
        if (SocketManager.GetInstance().Connect())
        {
            StartCoroutine(MainLoop());
        }
        else
        {
            Debug.Log("Unable to connect to the Python Script. Running the demo instead.");
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
            yield return new WaitForSeconds(20);
            yield return StartCoroutine(CalculateDensity());
            yield return StartCoroutine(SendRewards());
        }
    }

    public IEnumerator Reset()
    {
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
        SocketManager.GetInstance().Send("shot" + shotCount + ".png");
        yield return null;
    }

    public IEnumerator GetAction()
    {
        int trafficLightId = SocketManager.GetInstance().ReceiveInt() + 1;
        TrafficLightManager.GetInstance().SetAllToRed();
        Time.timeScale = 1;
        yield return new WaitForSeconds(10);
        TrafficLightManager.GetInstance().SetTrafficLightToGreen(trafficLightId);
        yield return null;
    }

    public IEnumerator CalculateDensity()
    {
        Time.timeScale = 0;
        double densityPerkm = (densityCount / 34.0);
        System.IO.File.AppendAllText("densityperkm.csv", densityPerkm.ToString() + ",");
        double averageSpeed = (speedList.Sum() / (densityCount));
        double flow = (densityPerkm * averageSpeed);
        System.IO.File.AppendAllText("flow.csv", flow.ToString() + ",");
        ResetDensityCount();
        speedList.Clear();
        yield return null;
    }

    public IEnumerator SendRewards()
    {
        int finalReward = rewardCount - (GameObject.FindGameObjectsWithTag("car").Length + GameObject.FindGameObjectsWithTag("hap").Length);
        System.IO.File.AppendAllText("truerewards.csv", finalReward.ToString() + ",");
        System.IO.File.AppendAllText("throughput.csv", rewardCount.ToString() + ",");
        SocketManager.GetInstance().Send("" + finalReward);
        ResetRewardCount();
        yield return null;
    }

    public int GetRewardCount()
    {
        return rewardCount;
    }

    public void IncrementRewardCount()
    {
        rewardCount++;
    }

    public void ResetRewardCount()
    {
        rewardCount = 0;
    }

    public int GetDensityCount()
    {
        return densityCount;
    }

    public void IncrementDensityCount()
    {
        densityCount++;
    }

    public void DecrementDensityCount()
    {
        densityCount--;
    }

    public void ResetDensityCount()
    {
        densityCount = 0;
    }
}

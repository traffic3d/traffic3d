using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PythonManager : MonoBehaviour
{
    private static bool headlessMode = false;
    public static PythonManager instance;

    public static PythonManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    private Camera camera;
    private bool hasScreenshot = false;
    private string screenshotPath;
    public int shotCount = 0;
    public int rewardCount = 0;
    public int densityCount;
    public List<double> speedList = new List<double>();

    /// <summary>
    /// If connected, get the screenshot file path from the python script over the socket and remove any invalid characters from the path.
    /// </summary>
    /// /// <param name="vehicle">The vehicle to create.</param>
    void Start()
    {
        if (IsHeadlessMode())
        {
            camera = GameObject.FindObjectOfType<Camera>();
            camera.enabled = false;
        }
        if (SocketManager.GetInstance().Connect())
        {
            screenshotPath = string.Concat(SocketManager.GetInstance().ReceiveString().Replace('\\', '/').Split(System.IO.Path.GetInvalidPathChars()));
            StartCoroutine(MainLoop());
        }
        else
        {
            Debug.Log("Unable to connect to the Python Script. Running the demo instead.");
            TrafficLightManager.GetInstance().RunDemo();
        }
    }

    /// <summary>
    /// The main loop for the Python script.
    /// </summary>
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

    /// <summary>
    /// Resets all the traffic lights to red and then waits.
    /// </summary>
    public IEnumerator Reset()
    {
        TrafficLightManager.GetInstance().SetAllToRed();
        yield return new WaitForSeconds(20);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Takes a screen shot and then stores it in within the screenshots folder.
    /// </summary>
    public IEnumerator TakeScreenshot()
    {
        shotCount += 1;
        if (!IsHeadlessMode())
        {
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(screenshotPath, "shot" + shotCount + ".png"));
        }
        else
        {
            camera.Render();
        }

        yield return null;
    }

    public void SetScreenshot(Texture2D screenshot)
    {
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(System.IO.Path.Combine(screenshotPath, "shot" + shotCount + ".png"), bytes);
        hasScreenshot = true;
    }

    /// <summary>
    /// Sends the file path as a string to the socket using the SocketManager.
    /// </summary>
    public IEnumerator SendScreenshot()
    {
        if (headlessMode)
        {
            while (!hasScreenshot)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        SocketManager.GetInstance().Send("shot" + shotCount + ".png");
        hasScreenshot = false;
        yield return null;
    }

    /// <summary>
    /// Gets the action which is an int sent by the python script.
    /// It uses the int by converting it into a traffic light id.
    /// All traffic lights are then set to red and it waits.
    /// Once the wait is over that traffic light with the specified ID is then changed to green.
    /// </summary>
    public IEnumerator GetAction()
    {
        int trafficLightId = SocketManager.GetInstance().ReceiveInt() + 1;
        TrafficLightManager.GetInstance().SetAllToRed();
        Time.timeScale = 1;
        yield return new WaitForSeconds(10);
        TrafficLightManager.GetInstance().SetTrafficLightToGreen(trafficLightId);
        yield return null;
    }

    /// <summary>
    /// Calculates the density of the traffic and the flow.
    /// </summary>
    public IEnumerator CalculateDensity()
    {
        Time.timeScale = 0;
        double densityPerkm = (densityCount / 34.0);
        double averageSpeed = (speedList.Sum() / (densityCount));
        double flow = (densityPerkm * averageSpeed);
        ResetDensityCount();
        speedList.Clear();
        yield return null;
    }

    /// <summary>
    /// Sends the rewards calculated by Unity to the python script using the SocketManager.
    /// </summary>
    public IEnumerator SendRewards()
    {
        // Both "car" and "hap" tags are cars that are waiting.
        int finalReward = rewardCount - (GameObject.FindGameObjectsWithTag("car").Length + GameObject.FindGameObjectsWithTag("hap").Length);
        SocketManager.GetInstance().Send("" + finalReward);
        ResetRewardCount();
        yield return null;
    }

    public static bool IsHeadlessMode()
    {
        return headlessMode;
    }

    public static void SetHeadlessMode(bool isHeadlessMode)
    {
        headlessMode = isHeadlessMode;
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

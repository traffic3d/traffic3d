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

    private string screenshotPath;
    private double densityLengthConstant;
    public int shotCount = 0;
    public Dictionary<Junction, int> junctionShotCount = new Dictionary<Junction, int>();
    public int rewardCount = 0;
    public int densityCount;
    public List<double> speedList = new List<double>();

    /// <summary>
    /// If connected, get the screenshot file path from the python script over the socket and remove any invalid characters from the path.
    /// </summary>
    /// /// <param name="vehicle">The vehicle to create.</param>
    void Start()
    {
        if (Settings.IsHeadlessMode())
        {
            foreach (Camera camera in GameObject.FindObjectsOfType<Camera>())
            {
                camera.enabled = false;
            }
        }
        if (SocketManager.GetInstance().Connect())
        {
            screenshotPath = string.Concat(SocketManager.GetInstance().ReceiveString().Replace('\\', '/').Split(System.IO.Path.GetInvalidPathChars()));
            StartCoroutine(MainLoop());
        }
        else
        {
            Debug.Log("Unable to connect to the Python Script. Running the demo instead.");
            if (SumoManager.GetInstance() == null || !SumoManager.GetInstance().IsConnected() || !SumoManager.GetInstance().IsControlledBySumo(SumoLinkControlPoint.TRAFFIC_LIGHTS))
            {
                TrafficLightManager.GetInstance().RunDemo();
            }
        }
        densityLengthConstant = FindObjectsOfType<Path>().Select(path => path.GetDistanceUntilDensityMeasurePointInKM()).Sum();
    }

    /// <summary>
    /// The main loop for the Python script.
    /// </summary>
    public IEnumerator MainLoop()
    {
        yield return StartCoroutine(Reset());
        while (true)
        {
            yield return StartCoroutine(TakeScreenshots());
            yield return StartCoroutine(SendScreenshots());
            yield return StartCoroutine(GetActions());
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
    public IEnumerator TakeScreenshots()
    {
        foreach (Junction junction in TrafficLightManager.GetInstance().GetJunctions())
        {
            Camera camera = junction.GetJunctionCamera();
            if (camera == null)
            {
                throw new System.Exception("Unable to get camera for junction: " + junction.junctionId);
            }
            junctionShotCount[junction] = GetJunctionScreenshotCount(junction) + 1;
            CameraManager cameraManager = camera.GetComponent<CameraManager>();
            cameraManager.SetRenderScreenshot();
            camera.Render();
        }

        yield return null;
    }

    public void SetScreenshot(Camera camera, Texture2D screenshot)
    {
        byte[] bytes = screenshot.EncodeToPNG();
        Junction junction = TrafficLightManager.GetInstance().GetJunctions().ToList().Find(j => j.GetJunctionCamera() == camera);
        File.WriteAllBytes(GetScreenshotFilePath(screenshotPath, junction.junctionId, GetJunctionScreenshotCount(junction)), bytes);
    }

    /// <summary>
    /// Sends the file path as a string to the socket using the SocketManager.
    /// </summary>
    public IEnumerator SendScreenshots()
    {
        List<PythonScreenshot> screenshots = new List<PythonScreenshot>();
        foreach (Junction junction in TrafficLightManager.GetInstance().GetJunctions())
        {
            screenshots.Add(new PythonScreenshot(junction.junctionId, GetScreenshotFilePath("", junction.junctionId, GetJunctionScreenshotCount(junction))));
        }
        PythonScreenshots pythonScreenshots = new PythonScreenshots(screenshots);
        SocketManager.GetInstance().Send(JsonUtility.ToJson(pythonScreenshots));
        yield return null;
    }

    /// <summary>
    /// Gets the action which is an int sent by the python script.
    /// It uses the int by converting it into a traffic light id.
    /// All traffic lights are then set to red and it waits.
    /// Once the wait is over that traffic light with the specified ID is then changed to green.
    /// </summary>
    public IEnumerator GetActions()
    {
        string dataString = SocketManager.GetInstance().ReceiveString();
        PythonActions pythonActions = JsonUtility.FromJson<PythonActions>(dataString);
        TrafficLightManager.GetInstance().SetAllToRed();
        Time.timeScale = 1;
        yield return new WaitForSeconds(10);
        foreach (PythonAction pythonAction in pythonActions.actions)
        {
            TrafficLightManager.GetInstance().GetJunction(pythonAction.junctionId).SetJunctionState(pythonAction.action + 1);
        }
        yield return null;
    }

    /// <summary>
    /// Calculates the density of the traffic and the flow.
    /// </summary>
    public IEnumerator CalculateDensity()
    {
        Time.timeScale = 0;
        double densityPerkm = (densityCount / densityLengthConstant);
        Utils.AppendAllTextToResults("DensityPerKm.csv", densityPerkm.ToString() + ",");
        double averageSpeed = (speedList.Sum() / (densityCount));
        double flow = (densityPerkm * averageSpeed);
        Utils.AppendAllTextToResults("Flow.csv", flow.ToString() + ",");
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
        Utils.AppendAllTextToResults("TrueRewards.csv", finalReward.ToString() + ",");
        Utils.AppendAllTextToResults("Throughput.csv", rewardCount.ToString() + ",");
        SocketManager.GetInstance().Send("" + finalReward);
        ResetRewardCount();
        yield return null;
    }

    public string GetScreenshotFilePath(string screenshotFilePath, string junctionId, int screenshotNumber)
    {
        return System.IO.Path.Combine(screenshotFilePath, junctionId + "_shot" + screenshotNumber + ".png");
    }

    public int GetJunctionScreenshotCount(Junction junction)
    {
        if (junctionShotCount.ContainsKey(junction))
        {
            return junctionShotCount[junction];
        }
        else
        {
            return 0;
        }
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

    /// <summary>
    /// A list of screenshots sent to the Python Script.
    /// </summary>
    [System.Serializable]
    public class PythonScreenshots
    {
        public List<PythonScreenshot> screenshots;

        public PythonScreenshots(List<PythonScreenshot> screenshots)
        {
            this.screenshots = screenshots;
        }
    }

    /// <summary>
    /// A single screenshot item containing the junction id and the screenshot file path.
    /// </summary>
    [System.Serializable]
    public class PythonScreenshot
    {
        public string junctionId;
        public string screenshotPath;

        public PythonScreenshot(string junctionId, string screenshotPath)
        {
            this.junctionId = junctionId;
            this.screenshotPath = screenshotPath;
        }
    }

    /// <summary>
    /// A List of Actions that have been sent from the Python Script to Traffic3D (see PythonAction).
    /// </summary>
    [System.Serializable]
    public class PythonActions
    {
        public PythonAction[] actions;
    }

    /// <summary>
    /// A single Python Action that contains the junction id of the junction to change and the action being the state
    /// of which the junction should be.
    /// </summary>
    [System.Serializable]
    public class PythonAction
    {
        public string junctionId;
        public int action;

        public PythonAction(string junctionId, int action)
        {
            this.junctionId = junctionId;
            this.action = action;
        }
    }

}

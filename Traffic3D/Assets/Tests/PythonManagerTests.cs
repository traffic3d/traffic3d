using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class PythonManagerTests : CommonSceneTest
{
    [TearDown]
    public void TearDown()
    {
        PythonManager.GetInstance().rewardCount = 0;
        PythonManager.GetInstance().densityCount = 0;
    }

    [UnityTest]
    public IEnumerator PythonManagerRewardCountTest()
    {
        int currentCount = PythonManager.GetInstance().GetRewardCount();
        currentCount++;
        PythonManager.GetInstance().IncrementRewardCount();
        Assert.AreEqual(PythonManager.GetInstance().GetRewardCount(), currentCount);
        currentCount = 0;
        PythonManager.GetInstance().ResetRewardCount();
        Assert.AreEqual(PythonManager.GetInstance().GetRewardCount(), currentCount);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PythonManagerDensityCountTest()
    {
        int currentCount = PythonManager.GetInstance().GetDensityCount();
        currentCount++;
        PythonManager.GetInstance().IncrementDensityCount();
        Assert.AreEqual(PythonManager.GetInstance().GetDensityCount(), currentCount);
        currentCount--;
        PythonManager.GetInstance().DecrementDensityCount();
        Assert.AreEqual(PythonManager.GetInstance().GetDensityCount(), currentCount);
        currentCount++;
        PythonManager.GetInstance().IncrementDensityCount();
        currentCount = 0;
        PythonManager.GetInstance().ResetDensityCount();
        Assert.AreEqual(PythonManager.GetInstance().GetDensityCount(), currentCount);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PythonManagerGetScreenshotFilePathTest()
    {
        string path = PythonManager.GetInstance().GetScreenshotFilePath("screenshots/", "junction1", 5);
        Assert.AreEqual("screenshots/junction1_shot5.png", path);
        yield return null;
    }

    [UnityTest]
    public IEnumerator FrameRateTest()
    {
        yield return null;
        Assert.AreEqual(Time.captureFramerate, PythonManager.GetInstance().frameRate);
    }

}

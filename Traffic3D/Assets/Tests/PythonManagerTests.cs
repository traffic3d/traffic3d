using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Category("Tests")]
public class PythonManagerTests
{
    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SocketManager.GetInstance().SetSocket(new MockSocket());
            SceneManager.LoadScene(0);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

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
        string path = PythonManager.GetInstance().GetScreenshotFilePath("screenshots/", 5);
        Assert.AreEqual("screenshots/shot5.png", path);
        yield return null;
    }

}

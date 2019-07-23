using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using System;

public class LatestPrefabTest
{

    [SetUp]
    public void SetUpTest()
    {
        // Load Scene before test
        try
        {
            SceneManager.LoadScene(0);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    [TearDown]
    public void TearDownTest()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
    }

    [UnityTest]
    public IEnumerator CameraFrameRateTest()
    {

        yield return null;

        cameraFrameRate cameraFrameRate = (cameraFrameRate)UnityEngine.Object.FindObjectOfType(typeof(cameraFrameRate));

        // Check capture framerate is correct
        Assert.AreEqual(Time.captureFramerate, cameraFrameRate.frameRate);

    }

    [UnityTest]
    public IEnumerator JourneyTimeCarCounterTest()
    {

        yield return null;

        // Incremental Test for journeyTimeCARCOUNTER
        int count = journeyTimeCARCOUNTER.getjourneyCARsCount();

        count++;
        journeyTimeCARCOUNTER.incrementjourneyCARsCount();

        // Get test for journeyTimeCARCOUNTER
        Assert.AreEqual(journeyTimeCARCOUNTER.getjourneyCARsCount(), count);

    }

}

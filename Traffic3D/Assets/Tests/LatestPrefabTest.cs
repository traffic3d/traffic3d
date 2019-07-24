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

        Assert.AreEqual(Time.captureFramerate, cameraFrameRate.frameRate);

    }

    [UnityTest]
    public IEnumerator JourneyTimeCarCounterTest()
    {

        yield return null;

        int count = journeyTimeCARCOUNTER.getjourneyCARsCount();

        count++;
        journeyTimeCARCOUNTER.incrementjourneyCARsCount();

        Assert.AreEqual(journeyTimeCARCOUNTER.getjourneyCARsCount(), count);

    }

}

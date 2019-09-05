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
        JourneyCarCounter.journeyCarCount = 0;
    }

    [UnityTest]
    public IEnumerator FrameRateTest()
    {

        yield return null;

        FrameRate frameRate = (FrameRate)UnityEngine.Object.FindObjectOfType(typeof(FrameRate));

        Assert.AreEqual(Time.captureFramerate, frameRate.frameRate);

    }

    [UnityTest]
    public IEnumerator JourneyCarCounterTest()
    {

        yield return null;

        int count = JourneyCarCounter.GetJourneyCarCount();

        count++;
        JourneyCarCounter.IncrementJourneyCarCount();

        Assert.AreEqual(JourneyCarCounter.GetJourneyCarCount(), count);

    }

}

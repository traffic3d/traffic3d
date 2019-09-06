using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class TrafficLightManagerWithAITests
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
    public void TearDown()
    {

        TrafficLightManagerWithAI.rewCount = 0;
        TrafficLightManagerWithAI.densityCount1 = 0;

    }

    [UnityTest]
    public IEnumerator TrafficLightManagerWithAIRewCountTest()
    {

        int currentCount = TrafficLightManagerWithAI.GetRewardCount();

        currentCount++;
        TrafficLightManagerWithAI.IncrementRewardCount();

        Assert.AreEqual(TrafficLightManagerWithAI.GetRewardCount(), currentCount);

        currentCount = 0;
        TrafficLightManagerWithAI.ResetRewardCount();

        Assert.AreEqual(TrafficLightManagerWithAI.GetRewardCount(), currentCount);

        yield return null;

    }

    [UnityTest]
    public IEnumerator TrafficLightManagerWithAIDensityCountTest()
    {

        int currentCount = TrafficLightManagerWithAI.GetDensityCount1();

        currentCount++;
        TrafficLightManagerWithAI.IncrementDensityCount1();

        Assert.AreEqual(TrafficLightManagerWithAI.GetDensityCount1(), currentCount);

        currentCount--;
        TrafficLightManagerWithAI.DecrementDensityCount1();

        Assert.AreEqual(TrafficLightManagerWithAI.GetDensityCount1(), currentCount);

        currentCount++;
        TrafficLightManagerWithAI.IncrementDensityCount1();

        currentCount = 0;
        TrafficLightManagerWithAI.ResetDensityCount1();

        Assert.AreEqual(TrafficLightManagerWithAI.GetDensityCount1(), currentCount);

        yield return null;

    }

}

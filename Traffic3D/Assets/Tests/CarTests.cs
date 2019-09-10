using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class CarTests
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

        OverallCarCounter.overallCarCount = 0;

    }

    // OverallCarCounter class test
    [UnityTest]
    public IEnumerator OverallCarCounterTest()
    {

        int currentCount = OverallCarCounter.GetOverallCarCount();

        currentCount++;
        OverallCarCounter.IncrementOverallCarCount();

        Assert.AreEqual(OverallCarCounter.GetOverallCarCount(), currentCount);

        yield return null;

    }

}

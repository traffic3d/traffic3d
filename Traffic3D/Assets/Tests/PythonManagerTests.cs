using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class PythonManagerTests
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

        PythonManager.rewardCount = 0;
        PythonManager.densityCount1 = 0;

    }

    [UnityTest]
    public IEnumerator PythonManagerRewardCountTest()
    {

        int currentCount = PythonManager.GetRewardCount();

        currentCount++;
        PythonManager.IncrementRewardCount();

        Assert.AreEqual(PythonManager.GetRewardCount(), currentCount);

        currentCount = 0;
        PythonManager.ResetRewardCount();

        Assert.AreEqual(PythonManager.GetRewardCount(), currentCount);

        yield return null;

    }

    [UnityTest]
    public IEnumerator PythonManagerDensityCountTest()
    {

        int currentCount = PythonManager.GetDensityCount1();

        currentCount++;
        PythonManager.IncrementDensityCount1();

        Assert.AreEqual(PythonManager.GetDensityCount1(), currentCount);

        currentCount--;
        PythonManager.DecrementDensityCount1();

        Assert.AreEqual(PythonManager.GetDensityCount1(), currentCount);

        currentCount++;
        PythonManager.IncrementDensityCount1();

        currentCount = 0;
        PythonManager.ResetDensityCount1();

        Assert.AreEqual(PythonManager.GetDensityCount1(), currentCount);

        yield return null;

    }

}

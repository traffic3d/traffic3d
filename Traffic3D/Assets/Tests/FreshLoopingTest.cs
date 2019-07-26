using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class FreshLoopingTest {

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

    [UnityTest]
    public IEnumerator FreshLoopingRewCountTest()
    {

        int currentCount = freshLOOPING.getrewcount();

        currentCount++;
        freshLOOPING.incrementRew();

        Assert.AreEqual(freshLOOPING.getrewcount(), currentCount);

        currentCount = 0;
        freshLOOPING.resetRew();

        Assert.AreEqual(freshLOOPING.getrewcount(), currentCount);

        yield return null;

    }

    [UnityTest]
    public IEnumerator FreshLoopingDensityCountTest()
    {

        int currentCount = freshLOOPING.getdensitycount1();

        currentCount++;
        freshLOOPING.incdensitycount1();

        Assert.AreEqual(freshLOOPING.getdensitycount1(), currentCount);

        currentCount--;
        freshLOOPING.decdensitycount1();

        Assert.AreEqual(freshLOOPING.getdensitycount1(), currentCount);

        currentCount++;
        freshLOOPING.incdensitycount1();

        currentCount = 0;
        freshLOOPING.resetdensitycount1();

        Assert.AreEqual(freshLOOPING.getdensitycount1(), currentCount);

        yield return null;

    }

    [UnityTest]
    public IEnumerator FreshLoopingScreenshotTest()
    {

        freshLOOPING freshLooping = (freshLOOPING)GameObject.FindObjectOfType(typeof(freshLOOPING));

        int shotCount = freshLOOPING.shot_count;

        shotCount++;
        yield return freshLooping.takeshot();

        Assert.AreEqual(shotCount, freshLOOPING.shot_count);

        string screenshotPath = Application.dataPath + "/Screenshots";
        FileAssert.Exists(screenshotPath + "/shot" + shotCount + ".png");

        yield return null;

    }

    [UnityTest]
    public IEnumerator FreshLoopingZeroTest()
    {

        freshLOOPING freshLooping = (freshLOOPING)GameObject.FindObjectOfType(typeof(freshLOOPING));

        freshLooping.waiting = false;

        yield return freshLooping.zero();

        Assert.IsTrue(freshLooping.n.CM == freshLooping.n.material2);
        Assert.IsTrue(freshLooping.m.CM == freshLooping.m.material2);
        Assert.AreEqual(Time.timeScale, 0);
        Assert.IsTrue(freshLooping.waiting);

        yield return null;

    }

}

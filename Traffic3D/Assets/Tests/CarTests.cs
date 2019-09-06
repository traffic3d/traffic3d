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

        CarFactoryCounter1.carCount = 0;
        OverallCarCounter.overallCarCount = 0;
        CarFactoryCounter2.carCount = 0;
        CarFactoryCounter3.carCount = 0;
        CarFactoryCounter4.carCount = 0;

    }

    // CarFactoryCounter1 class test
    [UnityTest]
    public IEnumerator CarFactoryCounter1Test()
    {

        int currentCarCount = CarFactoryCounter1.GetCarCount();

        currentCarCount++;
        CarFactoryCounter1.IncrementCarCount();

        Assert.AreEqual(CarFactoryCounter1.GetCarCount(), currentCarCount);

        currentCarCount--;
        CarFactoryCounter1.DecrementCarCount();

        Assert.AreEqual(CarFactoryCounter1.GetCarCount(), currentCarCount);

        currentCarCount++;
        CarFactoryCounter1.IncrementCarCount();

        currentCarCount = 0;
        CarFactoryCounter1.ResetCarCount();

        Assert.AreEqual(CarFactoryCounter1.GetCarCount(), currentCarCount);

        yield return null;

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

    // CarFactoryCounter2 class test
    [UnityTest]
    public IEnumerator CarFactoryCounter2Test()
    {

        int currentCarCount = CarFactoryCounter2.GetCarCount();

        currentCarCount++;
        CarFactoryCounter2.IncrementCarCount();

        Assert.AreEqual(CarFactoryCounter2.GetCarCount(), currentCarCount);

        currentCarCount--;
        CarFactoryCounter2.DecrementCarCount();

        Assert.AreEqual(CarFactoryCounter2.GetCarCount(), currentCarCount);

        currentCarCount++;
        CarFactoryCounter2.IncrementCarCount();

        currentCarCount = 0;
        CarFactoryCounter2.ResetCarCount();

        Assert.AreEqual(CarFactoryCounter2.GetCarCount(), currentCarCount);

        yield return null;

    }

    // CarFactoryCounter3 class test
    [UnityTest]
    public IEnumerator CarFactoryCounter3Test()
    {

        int currentCarCount = CarFactoryCounter3.GetCarCount();

        currentCarCount++;
        CarFactoryCounter3.IncrementCarCount();

        Assert.AreEqual(CarFactoryCounter3.GetCarCount(), currentCarCount);

        currentCarCount--;
        CarFactoryCounter3.DecrementCarCount();

        Assert.AreEqual(CarFactoryCounter3.GetCarCount(), currentCarCount);

        yield return null;

    }

    // CarFactoryCounter4 class test
    [UnityTest]
    public IEnumerator CarFactoryCounter4Test()
    {

        int currentCarCount = CarFactoryCounter4.GetCarCount();

        currentCarCount++;
        CarFactoryCounter4.IncrementCarCount();

        Assert.AreEqual(CarFactoryCounter4.GetCarCount(), currentCarCount);

        currentCarCount--;
        CarFactoryCounter4.DecrementCarCount();

        Assert.AreEqual(CarFactoryCounter4.GetCarCount(), currentCarCount);

        yield return null;

    }

}

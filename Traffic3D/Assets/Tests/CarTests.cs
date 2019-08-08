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

        CarCounter.carCount = 0;
        incrementCountNumber.CarC = 0;
        newCarCount.carCount = 0;
        carCounterFACTORY3.carCount = 0;
        carCounterFactory4.carCount = 0;

    }

    // CarCounter class test
    [UnityTest]
    public IEnumerator CarCounterTest()
    {

        int currentCarCount = CarCounter.getCarCount();

        currentCarCount++;
        CarCounter.incrementCarCount();

        Assert.AreEqual(CarCounter.getCarCount(), currentCarCount);

        currentCarCount--;
        CarCounter.decrementCarCount();

        Assert.AreEqual(CarCounter.getCarCount(), currentCarCount);

        currentCarCount++;
        CarCounter.incrementCarCount();

        currentCarCount = 0;
        CarCounter.resetCarCount();

        Assert.AreEqual(CarCounter.getCarCount(), currentCarCount);

        yield return null;

    }

    // incrementCountNumber class test
    [UnityTest]
    public IEnumerator IncrementCountNumberTest()
    {

        int currentCount = incrementCountNumber.getcarC();

        currentCount++;
        incrementCountNumber.incrementcarC();

        Assert.AreEqual(incrementCountNumber.getcarC(), currentCount);

        yield return null;

    }

    // newCarCount class test
    [UnityTest]
    public IEnumerator NewCarCountTest()
    {

        int currentCarCount = newCarCount.getCarCount();

        currentCarCount++;
        newCarCount.incrementCarCount();

        Assert.AreEqual(newCarCount.getCarCount(), currentCarCount);

        currentCarCount--;
        newCarCount.decrementCarCount();

        Assert.AreEqual(newCarCount.getCarCount(), currentCarCount);

        currentCarCount++;
        newCarCount.incrementCarCount();

        currentCarCount = 0;
        newCarCount.resetCarCount();

        Assert.AreEqual(newCarCount.getCarCount(), currentCarCount);

        yield return null;

    }

    // CarCounter class test
    [UnityTest]
    public IEnumerator CarCounterFactory3Test()
    {

        int currentCarCount = carCounterFACTORY3.getCarCount();

        currentCarCount++;
        carCounterFACTORY3.incrementCarCount();

        Assert.AreEqual(carCounterFACTORY3.getCarCount(), currentCarCount);

        currentCarCount--;
        carCounterFACTORY3.decrementCarCount();

        Assert.AreEqual(carCounterFACTORY3.getCarCount(), currentCarCount);

        yield return null;

    }

    // CarCounter class test
    [UnityTest]
    public IEnumerator CarCounterFactory4Test()
    {

        int currentCarCount = carCounterFactory4.getCarCount();

        currentCarCount++;
        carCounterFactory4.incrementCarCount();

        Assert.AreEqual(carCounterFactory4.getCarCount(), currentCarCount);

        currentCarCount--;
        carCounterFactory4.decrementCarCount();

        Assert.AreEqual(carCounterFactory4.getCarCount(), currentCarCount);

        yield return null;

    }

}

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class CarTests {

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
	public IEnumerator CarCounterTest() {

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

}

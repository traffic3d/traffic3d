using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class CarFactoryTests {

    public const int TEST_TIME = 20;

    [SetUp]
    public void SetUpTest()
    {

        try
        {
            SceneManager.LoadScene(0);
        }catch(Exception e)
        {
            Debug.Log(e);
        }

    }

	[UnityTest]
	public IEnumerator CarFactorySpawnTest() {

		yield return new WaitForSeconds(TEST_TIME);

        // No cars have been spawned at all from Factory 1 or 2
        Assert.AreNotEqual(CarCounter.getCarCount(), 0);
        Assert.AreNotEqual(newCarCount.getCarCount(), 0);

        List<Type> engineTypeList = new List<Type>();
        engineTypeList.Add(typeof(carEngine12));
        engineTypeList.Add(typeof(CarEngine2));
        engineTypeList.Add(typeof(CarEngine6));
        engineTypeList.Add(typeof(newCarEngine2));

        List<Object> carList = new List<Object>();

        foreach (Type engineType in engineTypeList)
        {
            carList.AddRange(GameObject.FindObjectsOfType(engineType));
        }

        // There are currently cars that have been spawned
        Assert.AreNotEqual(carList.Count, 0);

	}
}

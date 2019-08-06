using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class CarFactoryTests
{

    public const int TEST_TIME = 50;

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
    [Timeout((TEST_TIME * 1000 * 2))]
    public IEnumerator CarFactorySpawnTest()
    {

        // No cars have been spawned at all from Factory 1, 2, 3 or 4
        Assert.AreEqual(CarCounter.getCarCount(), 0);
        Assert.AreEqual(newCarCount.getCarCount(), 0);
        Assert.AreEqual(carCounterFACTORY3.getCarCount(), 0);
        Assert.AreEqual(carCounterFactory4.getCarCount(), 0);

        List<Type> engineTypeList = new List<Type>();
        engineTypeList.Add(typeof(carEngine12));
        engineTypeList.Add(typeof(CarEngine2));
        engineTypeList.Add(typeof(CarEngine4));
        engineTypeList.Add(typeof(CarEngine5));
        engineTypeList.Add(typeof(CarEngine6));
        engineTypeList.Add(typeof(newCarEngine2));
        engineTypeList.Add(typeof(VehicleEngine));
        engineTypeList.Add(typeof(VehicleEngine1));
        engineTypeList.Add(typeof(VehicleEngine3));

        bool carSpawned = false;
        for (int i = 0; i < TEST_TIME; i++)
        {

            yield return new WaitForSeconds(1);

            List<Object> carList = new List<Object>();

            foreach (Type engineType in engineTypeList)
            {
                carList.AddRange(GameObject.FindObjectsOfType(engineType));
            }

            // Check if a car has spawned
            if (carList.Count != 0)
            {
                carSpawned = true;
                break;
            }

        }

        Assert.True(carSpawned);

    }
}

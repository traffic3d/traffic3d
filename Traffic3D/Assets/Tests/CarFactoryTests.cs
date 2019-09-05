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

    public const int TEST_TIME = 300;

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

        // Optimize time by removing unneeded particles
        foreach (ParticleSystem particleSystem in GameObject.FindObjectsOfType<ParticleSystem>())
        {
            particleSystem.Stop();
        }

        List<Type> engineTypeList = new List<Type>();
        engineTypeList.Add(typeof(VehicleEngine8));
        engineTypeList.Add(typeof(VehicleEngine3));
        engineTypeList.Add(typeof(VehicleEngine4));
        engineTypeList.Add(typeof(VehicleEngine5));
        engineTypeList.Add(typeof(VehicleEngine6));
        engineTypeList.Add(typeof(VehicleEngine7));
        engineTypeList.Add(typeof(VehicleEngine2));
        engineTypeList.Add(typeof(VehicleEngine1));

        // Ensure there are no cars currently spawned in
        foreach (Type engineType in engineTypeList)
        {
            Assert.AreEqual(0, GameObject.FindObjectsOfType(engineType).Length);
        }

        bool allCarsSpawned = false;
        for (int i = 0; i < TEST_TIME; i++)
        {

            yield return new WaitForSeconds(1);

            engineTypeList.RemoveAll(engineType => GameObject.FindObjectsOfType(engineType).Length != 0);

            if (engineTypeList.Count == 0)
            {
                allCarsSpawned = true;
                break;
            }

        }

        Assert.True(allCarsSpawned);

    }

}

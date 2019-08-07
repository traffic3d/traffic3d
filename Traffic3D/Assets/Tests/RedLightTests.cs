using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RedLightTests
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

    [UnityTest]
    public IEnumerator RedLightOneTest()
    {

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));

        yield return null;

    }

    private void DisableLoops()
    {

        // Optimize time by removing unneeded particles
        foreach (ParticleSystem particleSystem in GameObject.FindObjectsOfType<ParticleSystem>())
        {
            particleSystem.Stop();
        }

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));
        redlight.StopAllCoroutines();

        /*
        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        carFactory2 carFactory2 = (carFactory2)GameObject.FindObjectOfType(typeof(carFactory2));
        carfactory3 carfactory3 = (carfactory3)GameObject.FindObjectOfType(typeof(carfactory3));
        carfactory4 carfactory4 = (carfactory4)GameObject.FindObjectOfType(typeof(carfactory4));
        carFactory.StopAllCoroutines();
        carFactory2.StopAllCoroutines();
        carfactory3.StopAllCoroutines();
        carfactory4.StopAllCoroutines();
        */
    }

}

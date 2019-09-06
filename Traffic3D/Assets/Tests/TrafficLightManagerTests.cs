using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TrafficLightManagerTests
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
    public IEnumerator TrafficLightFirstEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.FirstEvent());

        yield return null;

        Assert.AreEqual(trafficLightManager.trafficLightRed1.redMaterial, trafficLightManager.trafficLightRed1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen1.blackMaterial, trafficLightManager.trafficLightGreen1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed2.redMaterial, trafficLightManager.trafficLightRed2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen2.blackMaterial, trafficLightManager.trafficLightGreen2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed3.redMaterial, trafficLightManager.trafficLightRed3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen3.blackMaterial, trafficLightManager.trafficLightGreen3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed4.redMaterial, trafficLightManager.trafficLightRed4.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen4.blackMaterial, trafficLightManager.trafficLightGreen4.currentMaterial);

        trafficLightManager.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator TrafficLightSecondEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.SecondEvent());

        yield return null;

        Assert.AreEqual(trafficLightManager.trafficLightRed1.blackMaterial, trafficLightManager.trafficLightRed1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen1.greenMaterial, trafficLightManager.trafficLightGreen1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed2.redMaterial, trafficLightManager.trafficLightRed2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen2.blackMaterial, trafficLightManager.trafficLightGreen2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed3.redMaterial, trafficLightManager.trafficLightRed3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen3.blackMaterial, trafficLightManager.trafficLightGreen3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed4.redMaterial, trafficLightManager.trafficLightRed4.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen4.blackMaterial, trafficLightManager.trafficLightGreen4.currentMaterial);

        trafficLightManager.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator TrafficLightThirdEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.ThirdEvent());

        yield return null;

        Assert.AreEqual(trafficLightManager.trafficLightRed1.redMaterial, trafficLightManager.trafficLightRed1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen1.blackMaterial, trafficLightManager.trafficLightGreen1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed2.redMaterial, trafficLightManager.trafficLightRed2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen2.blackMaterial, trafficLightManager.trafficLightGreen2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed3.blackMaterial, trafficLightManager.trafficLightRed3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen3.greenMaterial, trafficLightManager.trafficLightGreen3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed4.redMaterial, trafficLightManager.trafficLightRed4.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen4.blackMaterial, trafficLightManager.trafficLightGreen4.currentMaterial);

        trafficLightManager.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator TrafficLightFourthEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.FourthEvent());

        yield return null;

        Assert.AreEqual(trafficLightManager.trafficLightRed1.redMaterial, trafficLightManager.trafficLightRed1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen1.blackMaterial, trafficLightManager.trafficLightGreen1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed2.blackMaterial, trafficLightManager.trafficLightRed2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen2.greenMaterial, trafficLightManager.trafficLightGreen2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed3.redMaterial, trafficLightManager.trafficLightRed3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen3.blackMaterial, trafficLightManager.trafficLightGreen3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed4.redMaterial, trafficLightManager.trafficLightRed4.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen4.blackMaterial, trafficLightManager.trafficLightGreen4.currentMaterial);

        trafficLightManager.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator TrafficLightFifthEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.FifthEvent());

        yield return null;

        Assert.AreEqual(trafficLightManager.trafficLightRed1.redMaterial, trafficLightManager.trafficLightRed1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen1.blackMaterial, trafficLightManager.trafficLightGreen1.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed2.redMaterial, trafficLightManager.trafficLightRed2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen2.blackMaterial, trafficLightManager.trafficLightGreen2.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed3.redMaterial, trafficLightManager.trafficLightRed3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen3.blackMaterial, trafficLightManager.trafficLightGreen3.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightRed4.blackMaterial, trafficLightManager.trafficLightRed4.currentMaterial);
        Assert.AreEqual(trafficLightManager.trafficLightGreen4.greenMaterial, trafficLightManager.trafficLightGreen4.currentMaterial);

        trafficLightManager.StopAllCoroutines();

    }

    private void DisableLoops()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));
        trafficLightManager.StopAllCoroutines();

    }

}

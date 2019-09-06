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

        foreach(TrafficLight trafficLight in trafficLightManager.GetTrafficLights())
        {
            Assert.AreEqual(TrafficLight.LightColour.RED, trafficLight.GetCurrentLightColour());
        }

        trafficLightManager.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator TrafficLightSecondEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.SecondEvent());

        yield return null;

        CheckTrafficLightIsGreen(1);

        trafficLightManager.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator TrafficLightThirdEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.ThirdEvent());

        yield return null;

        CheckTrafficLightIsGreen(3);

        trafficLightManager.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator TrafficLightFourthEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.FourthEvent());

        yield return null;

        CheckTrafficLightIsGreen(2);

        trafficLightManager.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator TrafficLightFifthEventTest()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));

        trafficLightManager.StartCoroutine(trafficLightManager.FifthEvent());

        yield return null;

        CheckTrafficLightIsGreen(4);

        trafficLightManager.StopAllCoroutines();

    }

    private void DisableLoops()
    {

        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));
        trafficLightManager.StopAllCoroutines();

    }

    private void CheckTrafficLightIsGreen(int id)
    {
        foreach (TrafficLight trafficLight in TrafficLightManager.GetInstance().GetTrafficLights())
        {
            if (trafficLight.GetTrafficLightId() == id)
            {
                Assert.AreEqual(TrafficLight.LightColour.GREEN, trafficLight.GetCurrentLightColour());
            }
            else
            {
                Assert.AreEqual(TrafficLight.LightColour.RED, trafficLight.GetCurrentLightColour());
            }
        }
    }

}

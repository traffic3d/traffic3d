using NUnit.Framework;
using System;
using System.Collections;
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
        foreach (TrafficLight trafficLight in trafficLightManager.GetTrafficLights())
        {
            Assert.AreEqual(TrafficLight.LightColour.RED, trafficLight.GetCurrentLightColour());
        }
        trafficLightManager.StopAllCoroutines();
    }

    [UnityTest]
    public IEnumerator TrafficLightFireEventTest()
    {
        DisableLoops();
        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));
        foreach (int i in trafficLightManager.demoOrder)
        {
            trafficLightManager.StartCoroutine(trafficLightManager.FireEvent(i));
            yield return null;
            CheckTrafficLightIsGreen(i);
        }
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

﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class TrafficLightTest
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
    public IEnumerator TrafficLightChangeColours()
    {

        GameObject trafficLightObject1 = GameObject.Find("TrafficLight1");
        GameObject trafficLightObject2 = GameObject.Find("TrafficLight2");
        TLaction1 trafficLightScript1 = trafficLightObject1.GetComponent<TLaction1>();
        TLaction2 trafficLightScript2 = trafficLightObject2.GetComponent<TLaction2>();

        // First Script
        // Set default (red)
        trafficLightScript1.defaultmaterial();

        Assert.IsTrue(CheckTrafficLightColour(trafficLightObject1, trafficLightScript1, 1, trafficLightScript1.material1));

        trafficLightScript1.materialchangeGREEN1();

        Assert.IsTrue(CheckTrafficLightColour(trafficLightObject1, trafficLightScript1, 1, trafficLightScript1.material3));

        trafficLightScript1.materialchangeRED1();

        Assert.IsTrue(CheckTrafficLightColour(trafficLightObject1, trafficLightScript1, 1, trafficLightScript1.material2));

        trafficLightScript1.materialchangeAMBER();

        Assert.IsTrue(CheckTrafficLightColour(trafficLightObject1, trafficLightScript1, 1, trafficLightScript1.material4));


        // Second Script
        // Set default (red)
        trafficLightScript2.defaultmaterial();

        Assert.IsTrue(CheckTrafficLightColour(trafficLightObject2, trafficLightScript2, 2, trafficLightScript2.material1));

        trafficLightScript2.materialchangeGREEN2();

        Assert.IsTrue(CheckTrafficLightColour(trafficLightObject2, trafficLightScript2, 2, trafficLightScript2.material3));

        trafficLightScript2.materialchangeRED2();

        Assert.IsTrue(CheckTrafficLightColour(trafficLightObject2, trafficLightScript2, 2, trafficLightScript2.material2));

        trafficLightScript2.materialchangeAMBER();

        Assert.IsTrue(CheckTrafficLightColour(trafficLightObject2, trafficLightScript2, 2, trafficLightScript2.material4));

        yield return null;

    }

    private bool CheckTrafficLightColour(GameObject trafficLight, MonoBehaviour trafficLightScript, int scriptNumber, Material expectedColour)
    {

        // Check CM (current colour) is correct colour and check Render Settings has the correct material 
        if (scriptNumber == 1)
        {
            return ((TLaction1)trafficLightScript).CM == expectedColour && trafficLight.GetComponent<Renderer>().sharedMaterials[0] == expectedColour;
        }
        else
        {
            return ((TLaction2)trafficLightScript).CM == expectedColour && trafficLight.GetComponent<Renderer>().sharedMaterials[0] == expectedColour;
        }

    }

}

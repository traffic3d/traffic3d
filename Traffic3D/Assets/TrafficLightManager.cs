using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{

    public static TrafficLightManager instance;

    public static TrafficLightManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    public TrafficLight[] trafficLights;

    void Start()
    {

        trafficLights = GameObject.FindObjectsOfType<TrafficLight>();

        StartCoroutine(MainLoop());

    }

    public IEnumerator MainLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FirstEvent());
            yield return StartCoroutine(SecondEvent());
            yield return StartCoroutine(ThirdEvent());
            yield return StartCoroutine(FourthEvent());
            yield return StartCoroutine(FifthEvent());
        }
    }

    public IEnumerator FirstEvent()
    {
        SetAllToRed();
        yield return new WaitForSeconds(200);
    }

    public IEnumerator SecondEvent()
    {
        SetTrafficLightToGreen(1);
        yield return new WaitForSeconds(19);
    }

    public IEnumerator ThirdEvent()
    {
        SetTrafficLightToGreen(3);
        yield return new WaitForSeconds(19);
    }

    public IEnumerator FourthEvent()
    {
        SetTrafficLightToGreen(2);
        yield return new WaitForSeconds(19);
    }

    public IEnumerator FifthEvent()
    {
        SetTrafficLightToGreen(4);
        yield return new WaitForSeconds(19);
    }

    public TrafficLight[] GetTrafficLights()
    {
        return trafficLights;
    }

    public TrafficLight GetTrafficLight(int id)
    {
        foreach (TrafficLight trafficLight in trafficLights)
        {
            if (trafficLight.GetTrafficLightId() == id)
            {
                return trafficLight;
            }
        }
        return null;
    }

    public void SetTrafficLightToGreen(int id)
    {
        foreach (TrafficLight trafficLight in trafficLights)
        {
            if (trafficLight.GetTrafficLightId() == id)
            {
                trafficLight.SetColour(TrafficLight.LightColour.GREEN);
            }
            else
            {
                trafficLight.SetColour(TrafficLight.LightColour.RED);
            }
        }
    }

    public void SetAllToRed()
    {
        foreach (TrafficLight trafficLight in trafficLights)
        {
            trafficLight.SetColour(TrafficLight.LightColour.RED);
        }
    }

}

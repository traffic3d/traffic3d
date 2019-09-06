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

    public int[] demoOrder;

    void Start()
    {
        trafficLights = GameObject.FindObjectsOfType<TrafficLight>();
        demoOrder = new int[4] { 1, 3, 2, 4 };
    }

    public void RunDemo()
    {
        StartCoroutine(DemoLoop());
    }

    public IEnumerator DemoLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FirstEvent());
            foreach(int i in demoOrder)
            {
                yield return StartCoroutine(FireEvent(i));
            }
        }
    }

    public IEnumerator FirstEvent()
    {
        yield return new WaitForSeconds(1);
        SetAllToRed();
        yield return new WaitForSeconds(200);
    }

    public IEnumerator FireEvent(int trafficLightId)
    {
        SetTrafficLightToGreen(trafficLightId);
        yield return new WaitForSeconds(19);
    }

    public TrafficLight[] GetTrafficLights()
    {
        return trafficLights;
    }

    public TrafficLight GetTrafficLightFromStopNode(Transform node)
    {
        foreach (TrafficLight trafficLight in trafficLights)
        {
            if (trafficLight.HasStopNode(node))
            {
                return trafficLight;
            }
        }
        return null;
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

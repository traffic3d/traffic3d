﻿using System.Collections;
using System.Linq;
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
        trafficLights = GameObject.FindObjectsOfType<TrafficLight>();
        instance = this;
    }

    public TrafficLight[] trafficLights;
    public int[] demoOrder;

    void Start()
    {
        demoOrder = new int[4] { 1, 3, 2, 4 };
    }

    /// <summary>
    /// Run the demo for the traffic lights which just changes four traffic lights in order using the demoOrder int array. 
    /// Normally used when a connection cannot be made with the python script.
    /// </summary>
    public void RunDemo()
    {
        StartCoroutine(DemoLoop());
    }

    public IEnumerator DemoLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FirstEvent());
            foreach (int i in demoOrder)
            {
                yield return StartCoroutine(FireEvent(i));
            }
        }
    }

    public IEnumerator FirstEvent()
    {
        SetAllToRed();
        yield return new WaitForSeconds(200);
    }

    /// <summary>
    /// Changes the colour of the inputted traffic light ID to green and waits.
    /// </summary>
    /// <param name="trafficLightId">The traffic light int ID which needs changing.</param>
    public IEnumerator FireEvent(int trafficLightId)
    {
        SetTrafficLightToGreen(trafficLightId);
        yield return new WaitForSeconds(19);
    }

    public TrafficLight[] GetTrafficLights()
    {
        return trafficLights;
    }

    /// <summary>
    /// Gets the traffic light from the certain path node, also known as stop node for the traffic light.
    /// </summary>
    /// <param name="node">The path node to get the traffic light from.</param>
    /// <returns>The traffic light of that certain node, but is null if no traffic light can be found.</returns>
    public TrafficLight GetTrafficLightFromStopNode(Transform node)
    {
        return trafficLights.ToList().Find(trafficLight => trafficLight.HasStopNode(node));
    }

    /// <summary>
    /// Get traffic light from the inputted ID.
    /// </summary>
    /// <param name="id">The ID of the traffic light needed.</param>
    /// <returns>The traffic light of the ID inputted.</returns>
    public TrafficLight GetTrafficLight(int id)
    {
        return trafficLights.ToList().Find(trafficLight => trafficLight.GetTrafficLightId() == id);
    }

    /// <summary>
    /// Set the traffic light to green and set all others to red.
    /// </summary>
    /// <param name="id">The ID of the traffic light to turn green.</param>
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

    /// <summary>
    /// Sets all traffic lights to red.
    /// </summary>
    public void SetAllToRed()
    {
        trafficLights.ToList().ForEach(trafficLight => trafficLight.SetColour(TrafficLight.LightColour.RED));
    }
}

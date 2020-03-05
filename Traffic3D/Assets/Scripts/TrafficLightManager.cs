﻿using System.Collections;
using System.Collections.Generic;
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
        RefreshTrafficLights();
        instance = this;
    }

    public TrafficLight[] trafficLights;
    public int[] demoOrder;

    public event TrafficLightChangeEvent trafficLightChangeEvent;

    public delegate void TrafficLightChangeEvent(object sender, TrafficLight.TrafficLightChangeEventArgs e);

    void Start()
    {
        demoOrder = new int[4] { 0, 1, 2, 3 };
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
        yield return StartCoroutine(FirstEvent());
        while (true)
        {
            foreach (int i in demoOrder)
            {
                yield return StartCoroutine(FireEvent(i));
            }
        }
    }

    public void RefreshTrafficLights()
    {
        trafficLights = GameObject.FindObjectsOfType<TrafficLight>();
        foreach (TrafficLight trafficLight in trafficLights)
        {
            trafficLight.trafficLightChangeEvent -= TrafficLightChange;
            trafficLight.trafficLightChangeEvent += TrafficLightChange;
        }
    }

    private void TrafficLightChange(object sender, TrafficLight.TrafficLightChangeEventArgs e)
    {
        if (trafficLightChangeEvent != null)
        {
            trafficLightChangeEvent.Invoke(sender, e);
        }
    }

    public IEnumerator FirstEvent()
    {
        SetAllToRed();
        yield return new WaitForSeconds(20);
    }

    /// <summary>
    /// Sets all to red, waits 5 seconds then changes the colour of the inputted traffic light ID to green and waits.
    /// </summary>
    /// <param name="trafficLightId">The traffic light int ID which needs changing.</param>
    public IEnumerator FireEvent(int eventNumber)
    {
        SetAllToRed();
        yield return new WaitForSeconds(5);
        SetTrafficLightsToGreen(trafficLights.Where((x, i) => i % demoOrder.Length == eventNumber).Select(t => t.trafficLightId).ToList());
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
    /// Is the node a stop node.
    /// </summary>
    /// <param name="node">The path node to check.</param>
    /// <returns>true if its a stop node.</returns>
    public bool IsStopNode(Transform node)
    {
        return GetTrafficLightFromStopNode(node) != null;
    }

    /// <summary>
    /// Get traffic light from the inputted ID.
    /// </summary>
    /// <param name="id">The ID of the traffic light needed.</param>
    /// <returns>The traffic light of the ID inputted.</returns>
    public TrafficLight GetTrafficLight(string id)
    {
        return trafficLights.ToList().Find(trafficLight => trafficLight.GetTrafficLightId().Equals(id));
    }

    /// <summary>
    /// Set the traffic light to green and set all others to red.
    /// </summary>
    /// <param name="id">The ID of the traffic light to turn green.</param>
    public void SetTrafficLightToGreen(string id)
    {
        SetTrafficLightsToGreen(new List<string>() { id });
    }

    /// <summary>
    /// Set all specified traffic lights to green and set the rest to red.
    /// </summary>
    /// <param name="id">The ID of the traffic light to turn green.</param>
    public void SetTrafficLightsToGreen(List<string> ids)
    {
        foreach (TrafficLight trafficLight in trafficLights)
        {
            if (ids.Contains(trafficLight.GetTrafficLightId()))
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

using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public string trafficLightId;
    public Material redMaterial;
    public Material amberMaterial;
    public Material greenMaterial;
    public Material blackMaterial;
    public GameObject redLightObject;
    public GameObject amberLightObject;
    public GameObject greenLightObject;
    public List<Transform> stopNodes;
    public LightColour currentLightColour = LightColour.RED;
    private Dictionary<LightColour, GameObject> lightObjects;
    private Dictionary<LightColour, Material> lightMaterials;

    public event TrafficLightChangeEvent trafficLightChangeEvent;

    public delegate void TrafficLightChangeEvent(object sender, TrafficLightChangeEventArgs e);

    void Awake()
    {
        lightObjects = new Dictionary<LightColour, GameObject>();
        lightObjects.Add(LightColour.RED, redLightObject);
        lightObjects.Add(LightColour.AMBER, amberLightObject);
        lightObjects.Add(LightColour.GREEN, greenLightObject);
        lightMaterials = new Dictionary<LightColour, Material>();
        lightMaterials.Add(LightColour.RED, redMaterial);
        lightMaterials.Add(LightColour.AMBER, amberMaterial);
        lightMaterials.Add(LightColour.GREEN, greenMaterial);
    }

    /// <summary>
    /// Sets the current colour of the traffic light.
    /// </summary>
    /// <param name="lightColour">The traffic light colour as an enum.</param>
    public void SetColour(LightColour lightColour)
    {
        currentLightColour = lightColour;
        foreach (KeyValuePair<LightColour, GameObject> lightObjectEntry in lightObjects)
        {
            if (lightColour == lightObjectEntry.Key)
            {
                ChangeMaterial(lightObjectEntry.Value, lightMaterials[lightObjectEntry.Key]);
            }
            else
            {
                ChangeMaterial(lightObjectEntry.Value, blackMaterial);
            }
        }
        if (trafficLightChangeEvent != null)
        {
            trafficLightChangeEvent.Invoke(this, new TrafficLightChangeEventArgs(this, currentLightColour));
        }
    }

    /// <summary>
    /// Changes the material of the inputted object.
    /// </summary>
    /// <param name="lightObject">The game object to have the material changed.</param>
    /// <param name="material">The material to change to.</param>
    private void ChangeMaterial(GameObject lightObject, Material material)
    {
        lightObject.GetComponent<Renderer>().material = material;
    }

    /// <summary>
    /// Returns the ID of the traffic light.
    /// </summary>
    /// <returns>The int ID of the traffic light.</returns>
    public string GetTrafficLightId()
    {
        return trafficLightId;
    }

    /// <summary>
    /// Checks if this traffic light has a certain path node also known as the stop node to the traffic light.
    /// </summary>
    /// <param name="node">The path node to check.</param>
    /// <returns>True if the traffic light has this node.</returns>
    public bool HasStopNode(Transform node)
    {
        return stopNodes.Contains(node);
    }

    /// <summary>
    /// Gets all stop nodes also known as path nodes that are related to this traffic light.
    /// </summary>
    /// <returns>List of nodes (Transforms).</returns>
    public List<Transform> GetStopNodes()
    {
        return stopNodes;
    }

    /// <summary>
    /// Gets the current light colour of the traffic light.
    /// </summary>
    /// <returns>The current LightColour enum.</returns>
    public LightColour GetCurrentLightColour()
    {
        return currentLightColour;
    }

    /// <summary>
    /// Checks to see if the inputted light colour is the current light colour.
    /// </summary>
    /// <param name="lightColour">The light colour to compare.</param>
    /// <returns>True if the inputted light colour is the current light colour.</returns>
    public bool IsCurrentLightColour(LightColour lightColour)
    {
        return currentLightColour == lightColour;
    }

    public enum LightColour
    {
        RED,
        AMBER,
        GREEN
    }

    public class TrafficLightChangeEventArgs : EventArgs
    {
        public TrafficLight trafficLight;
        public LightColour lightColour;

        public TrafficLightChangeEventArgs(TrafficLight trafficLight, LightColour lightColour)
        {
            this.trafficLight = trafficLight;
            this.lightColour = lightColour;
        }
    }
}

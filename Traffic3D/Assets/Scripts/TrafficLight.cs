using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public int trafficLightId;
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
    }

    private void ChangeMaterial(GameObject lightObject, Material material)
    {
        lightObject.GetComponent<Renderer>().material = material;
    }

    public int GetTrafficLightId()
    {
        return trafficLightId;
    }

    public bool HasStopNode(Transform node)
    {
        return stopNodes.Contains(node);
    }

    public List<Transform> GetStopNodes()
    {
        return stopNodes;
    }

    public LightColour GetCurrentLightColour()
    {
        return currentLightColour;
    }

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
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentSettings : MonoBehaviour
{
    public List<EnvironmentStateType> activeEnvironmentStateTypes;
    public float ambientIntensity = 1F;
    public Material rainSkyBox;
    public Material snowSkyBox;
    public Material nightSkyBox;
    public Light mainLight;
    public Color nightLightColour = new Color(0.8392157F, 0.8405378F, 1F);
    public Material snowMaterial;
    public PhysicMaterial normalSurfaceMaterial;
    public PhysicMaterial rainSurfaceMaterial;
    public PhysicMaterial snowSurfaceMaterial;

    private List<IEnvironmentState> registeredEnvironmentStates;

    void Awake()
    {
        registeredEnvironmentStates = new List<IEnvironmentState>();
        registeredEnvironmentStates.Add(new RainEnvironmentState());
        registeredEnvironmentStates.Add(new SnowEnvironmentState());
        registeredEnvironmentStates.Add(new NightEnvironmentState());
    }

    void Start()
    {
        // Set the normal material to all surfaces at start to be overridden if needed.
        SetSurfaceMaterial(normalSurfaceMaterial);
        RenderSettings.ambientIntensity = ambientIntensity;
        foreach (IEnvironmentState environmentState in registeredEnvironmentStates.OrderBy(s => s.GetPriority()))
        {
            if (activeEnvironmentStateTypes.Contains(environmentState.GetEnvironmentStateType()))
            {
                environmentState.EnableEnvironmentState(this);
            }
        }
    }

    /// <summary>
    /// Sets the PhysicMaterial for each surface road and pathway.
    /// </summary>
    /// <param name="surfaceMaterial">The physics material to apply</param>
    public void SetSurfaceMaterial(PhysicMaterial surfaceMaterial)
    {
        // For each roadway and pathway check to see if it has a collider and apply the material.
        foreach (GameObject roadway in GameObject.FindGameObjectsWithTag("roadway"))
        {
            if (roadway.GetComponent<Collider>() != null)
            {
                roadway.GetComponent<Collider>().material = surfaceMaterial;
            }
        }
        foreach (GameObject pathway in GameObject.FindGameObjectsWithTag("pathway"))
        {
            if (pathway.GetComponent<Collider>() != null)
            {
                pathway.GetComponent<Collider>().material = surfaceMaterial;
            }
        }
    }

}

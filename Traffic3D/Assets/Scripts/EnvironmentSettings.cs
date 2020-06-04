using DigitalRuby.RainMaker;
using UnityEngine;

public class EnvironmentSettings : MonoBehaviour
{
    public bool rain;
    public bool snow;
    public bool night;
    public float ambientIntensity = 1F;
    public Material rainSkyBox;
    public Material snowSkyBox;
    public Material nightSkyBox;
    public Light mainLight;
    public Color nightLightColour = new Color(0.8392157F, 0.8405378F, 1F);
    public Material snowMaterial;

    private const int snowHeight = 50;
    private const int snowLengthForward = 40;
    private const float nightLightIntensityConstant = 0.1F;

    void Start()
    {
        RenderSettings.ambientIntensity = ambientIntensity;
        if (rain)
        {
            GameObject rainPrefab = Resources.Load<GameObject>("Models/Rain");
            foreach (Camera cam in FindObjectsOfType<Camera>())
            {
                GameObject rainPrefabInstance = GameObject.Instantiate(rainPrefab, cam.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                rainPrefabInstance.GetComponent<RainScript>().Camera = cam;
            }
            RenderSettings.skybox = rainSkyBox;
        }
        if (snow)
        {
            GameObject snowPrefab = Resources.Load<GameObject>("Models/Snow");
            foreach (Camera cam in FindObjectsOfType<Camera>())
            {
                Vector3 camPosition = cam.transform.position;
                GameObject snowPrefabInstance = GameObject.Instantiate(snowPrefab, new Vector3(camPosition.x, camPosition.y + snowHeight, camPosition.z) + cam.transform.forward * snowLengthForward, Quaternion.Euler(new Vector3(0, 0, 0)));
            }
            foreach (Renderer renderer in GameObject.FindObjectsOfType<Renderer>())
            {
                if (renderer.material.name.ToLower().Contains("grass"))
                {
                    renderer.material = snowMaterial;
                }
            }
            RenderSettings.skybox = snowSkyBox;
        }
        if (night)
        {
            RenderSettings.skybox = nightSkyBox;
            if(mainLight != null)
            {
                mainLight.intensity = mainLight.intensity * nightLightIntensityConstant;
                mainLight.color = nightLightColour;
            }
        }
    }

}

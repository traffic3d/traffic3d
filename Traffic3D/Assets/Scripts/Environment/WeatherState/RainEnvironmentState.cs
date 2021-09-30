using DigitalRuby.RainMaker;
using UnityEngine;

public class RainEnvironmentState : IEnvironmentState
{
    public void EnableEnvironmentState(EnvironmentSettings environmentSettings)
    {
        environmentSettings.SetSurfaceMaterial(environmentSettings.rainSurfaceMaterial);
        GameObject rainPrefab = Resources.Load<GameObject>("Models/Rain");
        foreach (Camera cam in GameObject.FindObjectsOfType<Camera>())
        {
            GameObject rainPrefabInstance = GameObject.Instantiate(rainPrefab, cam.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            rainPrefabInstance.GetComponent<RainScript>().Camera = cam;
        }
        RenderSettings.skybox = environmentSettings.rainSkyBox;
    }

    public EnvironmentStateType GetEnvironmentStateType()
    {
        return EnvironmentStateType.RAIN;
    }

    public Priority GetPriority()
    {
        return Priority.LOW;
    }
}
